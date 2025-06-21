using Application.Interfaces;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
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

            await SendEmailInternalAsync(toEmail, subject, htmlBody);
        }

        public async Task SendResetPasswordEmailAsync(string toEmail, string resetLink)
        {
            var subject = "Reset Your Password - CommercialNews";

            // Load HTML template
            var templatePath = Path.Combine(Directory.GetCurrentDirectory(), "EmailTemplates", "ResetPasswordEmailTemplate.html");
            var htmlBody = await File.ReadAllTextAsync(templatePath);

            // Replace placeholder
            htmlBody = htmlBody.Replace("{{ResetLink}}", resetLink);

            await SendEmailInternalAsync(toEmail, subject, htmlBody);
        }

        // ✅ Helper dùng chung
        private async Task SendEmailInternalAsync(string toEmail, string subject, string htmlBody)
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_config["Email:From"]));
            email.To.Add(MailboxAddress.Parse(toEmail));
            email.Subject = subject;

            email.Body = new TextPart(MimeKit.Text.TextFormat.Html)
            {
                Text = htmlBody
            };

            using var smtp = new SmtpClient();
            await smtp.ConnectAsync(
                _config["Email:SmtpHost"],
                int.Parse(_config["Email:SmtpPort"]!),
                SecureSocketOptions.StartTls);

            await smtp.AuthenticateAsync(
                _config["Email:SmtpUser"],
                _config["Email:SmtpPass"]);

            await smtp.SendAsync(email);
            await smtp.DisconnectAsync(true);
        }
    }

}
