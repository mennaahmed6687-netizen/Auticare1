using auticare.core;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace auticare.Controllerss
{
    [Route("api/auth")]
    [ApiController]
    public class GoogleController : ControllerBase
    {
        private readonly UserManager<Parent> _userManager;
        private readonly IConfiguration _configuration;

        public GoogleController(UserManager<Parent> userManager, IConfiguration configuration)
        {
            _userManager = userManager;
            _configuration = configuration;
        }

        // ================= LOGIN GOOGLE =================
        [HttpGet("login-google")]
        public IActionResult LoginGoogle()
        {
            // Backend callback (Google يرجع هنا)
            var redirectUrl = Url.Action("GoogleResponse", "Google");

            var properties = new AuthenticationProperties
            {
                RedirectUri = redirectUrl
            };

            return Challenge(properties, "Google");
        }

        // ================= GOOGLE CALLBACK =================
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            if (!result.Succeeded)
                return Redirect("https://auticare1jj.netlify.app/index.html?error=google_failed");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (string.IsNullOrEmpty(email))
                return Redirect("https://auticare1jj.netlify.app/index.html?error=no_email");

            // ================= CHECK USER =================
            var user = await _userManager.FindByEmailAsync(email);

            if (user == null)
            {
                user = new Parent
                {
                    UserName = email,
                    Email = email,
                    Name = name ?? email,
                    Phone = "0000000000"
                };

                var createResult = await _userManager.CreateAsync(user);

                if (!createResult.Succeeded)
                    return Redirect("https://auticare1jj.netlify.app/index.html?error=create_failed");
            }

            // ================= JWT TOKEN =================
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? ""),
                new Claim(ClaimTypes.Email, user.Email ?? "")
            };

            var key = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_configuration["JWT:SecretKey"]!)
            );

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var jwt = new JwtSecurityToken(
                issuer: _configuration["JWT:Issuer"],
                audience: _configuration["JWT:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(1),
                signingCredentials: creds
            );

            var token = new JwtSecurityTokenHandler().WriteToken(jwt);

            // ================= FINAL REDIRECT TO FRONTEND =================
            return Redirect(
                $"https://auticare1jj.netlify.app/index.html" +
                $"?token={token}&username={user.UserName}&parentId={user.Id}"
            );
        }
    }
}


