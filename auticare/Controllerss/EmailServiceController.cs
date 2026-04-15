using auticare.core;
using auticare.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace auticare.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly UserManager<Parent> _userManager;
        private readonly EmailService _emailService;

        public AuthController(UserManager<Parent> userManager, EmailService emailService)
        {
            _userManager = userManager;
            _emailService = emailService;
        }

        // =========================
        // 🔐 FORGOT PASSWORD
        // =========================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Ok("If email exists, reset link will be sent.");

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            var encodedToken = Uri.EscapeDataString(token);
            var resetLink = $"http://127.0.0.1:5500/Grad_project-main%20(1)/Grad_project-main/html/resetpassword.html?email={Uri.EscapeDataString(model.Email)}&token={encodedToken}";

            await _emailService.SendEmailAsync(
                model.Email,
                "Reset Password",
                $"Click here to reset your password: <a href='{resetLink}'>Reset Password</a>"
            );

            return Ok("Reset link sent to email");
        }

        // =========================
        // 🔁 RESET PASSWORD
        // =========================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return BadRequest("Invalid request");

            var decodedToken = Uri.UnescapeDataString(model.Token);

            var result = await _userManager.ResetPasswordAsync(
                user,
                decodedToken,
                model.NewPassword
            );

            if (result.Succeeded)
                return Ok("Password reset successfully");

            return BadRequest(result.Errors);
        }
    }

    // =========================
    // 📦 DTOs (Correct Version)
    // =========================

    public class ForgotPasswordDto
    {
        public string Email { get; set; }
    }

    public class ResetPasswordDto
    {
        public string Email { get; set; }
        public string Token { get; set; }
        public string NewPassword { get; set; }
    }
}