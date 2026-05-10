using auticare.core;
using auticare.core.DTO;
using auticare.Data;
using Auticare.core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Asn1.Ocsp;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace auticare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<Parent> _userManager;
        private readonly IConfiguration _configuration;
        private readonly AuticareDbContext _context;

        public AccountController(UserManager<Parent> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }


            // ================= REGISTER =================
            [HttpPost("Register")]
            public async Task<IActionResult> RegisterNewUser([FromBody] DtoRegister user)
            {
                if (!ModelState.IsValid)
                {
                    var modelErrors = ModelState.Values
                        .SelectMany(v => v.Errors)
                        .Select(e => e.ErrorMessage)
                        .ToList();

                    return BadRequest(new
                    {
                        message = "بيانات غير صحيحة",
                        errors = modelErrors
                    });
                }

                var parent = new Parent
                {
                    Name = user.Name,
                    UserName = user.Email,
                    Email = user.Email,
                    Phone = user.Phone
                };

                var result = await _userManager.CreateAsync(parent, user.Password1);

                if (result.Succeeded)
                {
                    return Ok(new
                    {
                        message = "تم إنشاء الحساب بنجاح"
                    });
                }

                var errors = result.Errors
                    .Select(e => TranslateError(e.Description))
                    .ToList();

                return BadRequest(new
                {
                    message = "فشل إنشاء الحساب",
                    errors
                });
            }

            // ================= ERROR TRANSLATION =================
            private string TranslateError(string error)
            {
                return error switch
                {
                    var e when e.Contains("Password") && e.Contains("non alphanumeric") =>
                        "كلمة المرور يجب أن تحتوي على رمز مثل @ أو _",

                    var e when e.Contains("digit") =>
                        "كلمة المرور يجب أن تحتوي على رقم",

                    var e when e.Contains("uppercase") =>
                        "كلمة المرور يجب أن تحتوي على حرف كبير",

                    var e when e.Contains("6 characters") =>
                        "كلمة المرور يجب أن تكون 6 أحرف على الأقل",

                    var e when e.Contains("Email") && e.Contains("already") =>
                        "هذا البريد الإلكتروني مستخدم بالفعل",

                    var e when e.Contains("User name") =>
                        "اسم المستخدم مستخدم بالفعل",

                    _ => error
                };
            }
        
    
    [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] DtoLogin login)
        {


            
                if (!ModelState.IsValid)
                    return BadRequest(new { message = "بيانات غير صحيحه" });

                var parent = await _userManager.FindByEmailAsync(login.Email);

                if (parent == null)
                    return Unauthorized(new { message = "هذا البريد الالكتروني غير موجود" });

                var isPasswordValid = await _userManager.CheckPasswordAsync(parent, login.Password);

                if (!isPasswordValid)
                    return Unauthorized(new { message = "كلمة السر غير صحيحه" });

            // ✅ IMPORTANT FIX
      

           

           


            // ---------------- CLAIMS ----------------
            var claims = new List<Claim>
    {
        new Claim(ClaimTypes.NameIdentifier, parent.Id),
        new Claim(ClaimTypes.Name, parent.UserName ?? ""),
        new Claim(ClaimTypes.Email, parent.Email ?? ""),
        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
       

        // ✅ التصليح هنا فقط

    };
            // 🔐 Roles
            var roles = await _userManager.GetRolesAsync(parent);
            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // ---------------- KEY ----------------
            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"])
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            // ---------------- TOKEN ----------------
            var token = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            // ---------------- RESPONSE ----------------
            return Ok(new
            {
                token = jwt,
                expiration = token.ValidTo,
                email = parent.Email,
                username = parent.UserName,
                role = roles.Contains("Admin") ? "Admin" : "Parent",
                isAdmin = roles.Contains("Admin"),
                message = "Login successful",
                parentId=parent.Id,
            });
        }
    }
}
