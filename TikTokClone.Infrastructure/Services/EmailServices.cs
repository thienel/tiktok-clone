using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using TikTokClone.Application.Interfaces.Services;

namespace TikTokClone.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private ILogger<EmailService> _logger;
        private readonly SmtpClient _smtpClient;

        public EmailService(IConfiguration configuration, ILogger<EmailService> logger)
        {
            _configuration = configuration;
            _logger = logger;
            _smtpClient = new SmtpClient(_configuration["EmailSettings:SmtpServer"])
            {
                Port = int.Parse(_configuration["EmailSettings:Port"] ?? "587"),
                Credentials = new NetworkCredential(
                    _configuration["EmailSettings:Username"],
                    _configuration["EmailSettings:Password"]
                ),
                EnableSsl = bool.Parse(configuration["EmailSettings:EnableSsl"] ?? "true")
            };
        }

        public async Task<bool> SendEmailVerificationCodeAsync(string email, string verificationCode)
        {
            try
            {
                var subject = $"{verificationCode} is your verification code";
                var body = GenerateVerificationEmailBody(verificationCode);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["EmailSettings:FromEmail"]!, _configuration["EmailSettings:FromName"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                await _smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Verification email sent successfully to {Email}", email);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public Task<bool> SendPasswordResetEmailAsync(string email, string resetToken)
        {
            throw new NotImplementedException();
        }

        public string GenerateVerificationEmailBody(string code)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                </head>
                <body>
                    <h1>{code}</h1>
                </body>
                </html>";
        }
    }
}
