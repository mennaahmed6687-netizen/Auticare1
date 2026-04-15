using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using auticare.core;

namespace auticare.Controllerss
{
    [Route("api/auth")]
    [ApiController]
    public class GoogleController : Controller
    {
        private readonly UserManager<Parent> _userManager;
        private readonly SignInManager<Parent> _signInManager;

        public GoogleController(
            UserManager<Parent> userManager,
            SignInManager<Parent> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // ================= Google Login =================
        [HttpGet("login-google")]
        public IActionResult LoginGoogle()
        {
            return Challenge(new AuthenticationProperties
            {
                RedirectUri = "http://127.0.0.1:5500/Grad_project-main%20(1)/Grad_project-main/index.html"
            }, "Google");
        }

        // ================= Google Response =================
        [HttpGet("google-response")]
        public async Task<IActionResult> GoogleResponse()
        {
            var result = await HttpContext.AuthenticateAsync(IdentityConstants.ExternalScheme);

            if (!result.Succeeded)
                return BadRequest("Login Failed");

            var email = result.Principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = result.Principal.FindFirst(ClaimTypes.Name)?.Value;

            if (email == null)
                return BadRequest("Email not found");

            // ================= Check User =================
            var user = await _userManager.FindByEmailAsync(email);

            // ================= Create User =================
            if (user == null)
            {
                user = new Parent
                {
                    UserName = email,
                    Email = email
                };

                await _userManager.CreateAsync(user);
            }

            // ================= Sign In =================
            await _signInManager.SignInAsync(user, false);

            return Ok(new
            {
                Message = "Login Success",
                Email = email,
                Name = name
            });
        }
    }
}
