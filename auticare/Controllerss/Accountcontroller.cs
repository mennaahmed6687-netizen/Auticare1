using auticare.core;
using auticare.core.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
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

        public AccountController(UserManager<Parent> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // ---------------- REGISTER ----------------
        [HttpPost("Register")]
        public async Task<IActionResult> RegisterNewUser([FromBody] DtoRegister user)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            Parent parent = new Parent
            {
                Name = user.Name,
                UserName = user.Name,   // مهم جدًا لـ Identity
                Email = user.Email,
                Phone = user.Phone
            };

            var result = await _userManager.CreateAsync(parent, user.Password1);

            if (result.Succeeded)
                return Ok("Success");

            foreach (var item in result.Errors)
                ModelState.AddModelError("", item.Description);

            return BadRequest(ModelState);
        }

        // ---------------- LOGIN ----------------
        [HttpPost("Login")]
        public async Task<IActionResult> Login([FromBody] DtoLogin login)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            // البحث باستخدام UserName (وهو Name عندك)
            var parent = await _userManager.FindByNameAsync(login.UserName);

            if (parent == null)
                return Unauthorized("Invalid User");

            if (!await _userManager.CheckPasswordAsync(parent, login.Password))
                return Unauthorized("Invalid Password");

            // ---------------- CLAIMS ----------------
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, parent.UserName),
                new Claim(ClaimTypes.NameIdentifier, parent.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

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

            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiration = token.ValidTo
            });
        }
    }
}