using Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System.Net.Mail;
using System.Net;

namespace Infrastructure.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }

        public async Task SendVerificationEmailAsync(string toEmail, string verificationLink)
        {
            var subject = "Email Authentication - CommercialNews";

            // Load HTML template
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "VerificationEmailTemplate.html");
            var htmlBody = await File.ReadAllTextAsync(templatePath);

            // Replace placeholder
            htmlBody = htmlBody.Replace("{{VerificationLink}}", verificationLink);

            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        public async Task SendResetPasswordEmailAsync(string toEmail, string resetLink)
        {
            var subject = "Reset Your Password - CommercialNews";

            // Load HTML template
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "ResetPasswordEmailTemplate.html");
            var htmlBody = await File.ReadAllTextAsync(templatePath);

            // Replace placeholder
            htmlBody = htmlBody.Replace("{{ResetLink}}", resetLink);

            await SendEmailAsync(toEmail, subject, htmlBody);
        }

        // ✅ Gửi email dùng MailKit
        public async Task SendEmailAsync(string toEmail, string subject, string htmlBody)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["Email:From"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlBody
            };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();
            await smtp.ConnectAsync(
                _config["Email:SmtpHost"],
                int.Parse(_config["Email:SmtpPort"] ?? "587"),
                SecureSocketOptions.StartTls
            );

            await smtp.AuthenticateAsync(
                _config["Email:SmtpUser"],
                _config["Email:SmtpPass"]
            );

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }

}
