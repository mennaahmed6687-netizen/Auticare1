using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using auticare.core;
using Auticare.Core.Models.Admin;

namespace Auticare.Controllerss
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Parent> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IConfiguration _configuration;

        public AuthController(
            UserManager<Parent> userManager,
            RoleManager<IdentityRole> roleManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _configuration = configuration;
        }

        [HttpPost("login")]
        public async Task<ActionResult<ApiResponse<LoginResponse>>> Login([FromBody] LoginRequest request)
        {
            try
            {
                // Find user by email
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    return BadRequest(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "البريد الإلكتروني أو كلمة المرور غير صحيحة"
                    });
                }

                // Check password
                var isPasswordValid = await _userManager.CheckPasswordAsync(user, request.Password);
                if (!isPasswordValid)
                {
                    return BadRequest(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "البريد الإلكتروني أو كلمة المرور غير صحيحة"
                    });
                }

                // Check if user is Admin
                var isAdmin = await _userManager.IsInRoleAsync(user, "Admin");
                if (!isAdmin)
                {
                    return BadRequest(new ApiResponse<LoginResponse>
                    {
                        Success = false,
                        Message = "غير مصرح لك بالوصول إلى لوحة التحكم"
                    });
                }

                // Generate JWT Token
                var token = await GenerateJwtToken(user);

                var response = new LoginResponse
                {
                    Token = token,
                    Email = user.Email,
                    Name = user.Name,
                    Role = "Admin"
                };

                return Ok(new ApiResponse<LoginResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "تم تسجيل الدخول بنجاح"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<LoginResponse>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء تسجيل الدخول",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("admin/register")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<RegisterResponse>>> RegisterAdmin([FromBody] RegisterRequest request)
        {
            try
            {
                // Check if user already exists
                var existingUser = await _userManager.FindByEmailAsync(request.Email);
                if (existingUser != null)
                {
                    return BadRequest(new ApiResponse<RegisterResponse>
                    {
                        Success = false,
                        Message = "البريد الإلكتروني مستخدم بالفعل"
                    });
                }

                // Create Admin role if it doesn't exist
                if (!await _roleManager.RoleExistsAsync("Admin"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                // Create new user
                var user = new Parent
                {
                    UserName = request.Email,
                    Email = request.Email,
                    Name = request.Name,
                    EmailConfirmed = true,
                    Created = DateTime.UtcNow
                };

                var result = await _userManager.CreateAsync(user, request.Password);

                if (!result.Succeeded)
                {
                    return BadRequest(new ApiResponse<RegisterResponse>
                    {
                        Success = false,
                        Message = "فشل إنشاء المستخدم",
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }

                // Add user to Admin role
                await _userManager.AddToRoleAsync(user, "Admin");

                // Generate JWT Token
                var token = await GenerateJwtToken(user);

                var response = new RegisterResponse
                {
                    Token = token,
                    Email = user.Email,
                    Name = user.Name,
                    Role = "Admin"
                };

                return Ok(new ApiResponse<RegisterResponse>
                {
                    Success = true,
                    Data = response,
                    Message = "تم إنشاء حساب الأدمن بنجاح"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<RegisterResponse>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء إنشاء الحساب",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        [HttpPost("logout")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> Logout()
        {
            // For JWT, logout is typically handled client-side by removing the token
            // But we can add token invalidation logic here if needed
            
            return Ok(new ApiResponse<bool>
            {
                Success = true,
                Data = true,
                Message = "تم تسجيل الخروج بنجاح"
            });
        }

        [HttpGet("me")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<UserProfile>>> GetCurrentUser()
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    return NotFound(new ApiResponse<UserProfile>
                    {
                        Success = false,
                        Message = "المستخدم غير موجود"
                    });
                }

                var profile = new UserProfile
                {
                    Id = user.Id,
                    Email = user.Email,
                    Name = user.Name,
                    Role = "Admin",
                    CreatedAt = user.Created
                };

                return Ok(new ApiResponse<UserProfile>
                {
                    Success = true,
                    Data = profile,
                    Message = "تم جلب بيانات المستخدم بنجاح"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new ApiResponse<UserProfile>
                {
                    Success = false,
                    Message = "حدث خطأ أثناء جلب بيانات المستخدم",
                    Errors = new List<string> { ex.Message }
                });
            }
        }

        private async Task<string> GenerateJwtToken(Parent user)
        {
            var jwtSettings = _configuration.GetSection("JWT");
            var secretKey = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("JWT SecretKey is not configured");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email),
                new Claim(JwtRegisteredClaimNames.Name, user.Name),
                new Claim(ClaimTypes.Role, "Admin"),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtSettings["Issuer"],
                audience: jwtSettings["Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(24),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }

    // DTOs for Authentication
    public class LoginRequest
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class RegisterRequest
    {
        public string Name { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }

    public class LoginResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class RegisterResponse
    {
        public string Token { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
    }

    public class UserProfile
    {
        public string Id { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}
