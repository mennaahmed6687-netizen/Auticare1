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

        // ================= FORGOT PASSWORD =================
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordDto model)
        {
            if (string.IsNullOrWhiteSpace(model?.Email))
                return BadRequest(new { message = "البريد الإلكتروني مطلوب ❌" });

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return Ok(new { message = "تم إرسال الرابط إذا كان البريد موجودًا ✉️" });

            var token = await _userManager.GeneratePasswordResetTokenAsync(user);

            // ⚠️ مهم: لا تعتمد double encoding
            var resetLink =
                $"http://127.0.0.1:5500/html/resetpassword.html" +
                $"?email={model.Email}&token={Uri.EscapeDataString(token)}";

            await _emailService.SendEmailAsync(
                model.Email,
                "إعادة تعيين كلمة المرور",
                $"اضغط هنا: <a href='{resetLink}'>Reset Password</a>"
            );

            return Ok(new { message = "تم إرسال رابط إعادة التعيين ✉️" });
        }

        // ================= RESET PASSWORD =================
        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto model)
        {
            if (string.IsNullOrWhiteSpace(model?.Email) ||
                string.IsNullOrWhiteSpace(model?.Token))
            {
                return BadRequest(new { message = "بيانات غير صحيحة ❌" });
            }

            var user = await _userManager.FindByEmailAsync(model.Email);

            if (user == null)
                return BadRequest(new { message = "المستخدم غير موجود ❌" });

            var result = await _userManager.ResetPasswordAsync(
                user,
                model.Token, // 👈 بدون Unescape (مهم جدًا)
                model.NewPassword
            );

            if (result.Succeeded)
            {
                return Ok(new { message = "تم تغيير كلمة المرور بنجاح ✅" });
            }

            return BadRequest(new
            {
                message = "فشل تغيير كلمة المرور ❌",
                errors = result.Errors.Select(e => e.Description)
            });
        }
    }

    // ================= DTOs =================
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