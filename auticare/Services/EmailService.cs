using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;

namespace auticare.Services
{
    public class EmailService
    {
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var email = new MimeMessage();

            // المرسل
            email.From.Add(new MailboxAddress("Auticare App", "Mennaahmed6687@gmail.com"));

            // المستقبل
            email.To.Add(MailboxAddress.Parse(to));

            // الموضوع
            email.Subject = subject;

            // المحتوى (HTML)
            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = body
            };

            using var smtp = new SmtpClient();

            try
            {
                // ⚠️ حل مشكلة SSL (للتجربة + بعض الشبكات)
                smtp.ServerCertificateValidationCallback = (s, c, h, e) => true;

                // الاتصال بـ Gmail
                await smtp.ConnectAsync(
                    "smtp.gmail.com",
                    587,
                    SecureSocketOptions.StartTlsWhenAvailable
                );

                // تسجيل الدخول (App Password)
                await smtp.AuthenticateAsync(
                    "Mennaahmed6687@gmail.com",
                    "fnmt vthn lchl mtyw"
                );

                // إرسال الإيميل
                await smtp.SendAsync(email);

                await smtp.DisconnectAsync(true);
            }
            catch (Exception ex)
            {
                Console.WriteLine("Email Error: " + ex.Message);
                throw;
            }
        }
    }
}
