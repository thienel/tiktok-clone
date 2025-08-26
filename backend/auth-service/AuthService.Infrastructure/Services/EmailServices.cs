using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AuthService.Interfaces.Services;

namespace AuthService.Infrastructure.Services
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

        public async Task<bool> SendPasswordResetEmailAsync(string email, string verificationCode, string name)
        {
            try
            {
                var subject = $"{verificationCode} is your verification code";
                var body = GenerateChangePasswordEmailBody(verificationCode, name);

                var mailMessage = new MailMessage
                {
                    From = new MailAddress(_configuration["EmailSettings:FromEmail"]!, _configuration["EmailSettings:FromName"]),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                mailMessage.To.Add(email);
                await _smtpClient.SendMailAsync(mailMessage);
                _logger.LogInformation("Change password email sent successfully to {Email}", email);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public string GenerateVerificationEmailBody(string code)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                </head>
                <body>
                    <div style='display: flex'>
                        <div style='width: 100%; max-width: 440px; padding: 0px 20px'>
                        <div style='width: 100%; padding: 40px 7px'>
                            <img width=150px src='https://ci3.googleusercontent.com/meips/ADKq_NYK420UkUAFUsKIvzZlahWkg41X9bA1QHf8TAGuZ6ym8m5XYTWJ2Ee90ro6vYswm83-RT1a0AIsN9w5idF4Z_lvhqDOa86bd0GwD60T3s6Dn0-spVP2AbIsrT452YMV-LIR2a1b2k_SiU-YxQ=s0-d-e1-ft#http://p16-tiktokcdn-com.akamaized.net/obj/tiktok-obj/f70f9d0dc867d6c71ce2cd684a0ffff0' />
                        </div>
                        <div style='max-width:100%;background-color:#f1f1f1;padding:20px 16px;font-weight:bold;font-size:20px;color:rgb(22,24,35)'>
                        Verification Code
                        </div>
                        <div style='max-width:100%;background-color:#f8f8f8;padding:24px 16px;font-size:17px;color:rgba(22,24,35,0.75);line-height:20px'>
                        <p style='margin-bottom:20px'>To verify your account, enter this code in TikTok:</p>
                        <p style='margin-bottom:20px;color:rgb(22,24,35);font-weight:bold'>{code}</p>
                        <p style='margin-bottom:20px'>Verification codes expire after 48 hours.</p>
                        <p style='margin-bottom:20px'>If you didn't request this code, you can ignore this message.</p>

                        <p>TikTok Support Team</p>
                        <p style='word-break:break-all'>
                            TikTok Help Center:
                            <a style='color:rgb(0,91,158)' href='https://support.tiktok.com/' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://support.tiktok.com/&amp;source=gmail&amp;ust=1748939493036000&amp;usg=AOvVaw1wDuidsFuxN9LEUUZ2tcYu'>https://support.tiktok.com/</a>
                        </p>
                        </div>
                        <div style='max-width:100%;padding:40px 16px 20px;font-size:15px;color:rgba(22,24,35,0.5);line-height:18px'>
                        <div>Have a question?</div>
                        <div style='margin-bottom:20px'>Check out our help center or contact us in the app using
                            <span style='color:rgb(0,91,158);font-weight:bold'>Settings &gt; Report a Problem.</span></div>
                        <div>This is an automatically generated email. Replies to this email address aren't monitored.</div>
                        </div>
                        <div style='color:rgba(22,24,35,0.5);margin:20px 16px 40px 16px;font-size:12px;line-height:18px'>
                        <div style='word-break:break-all'>
                            <a style='color:rgba(22,24,35,0.5);text-decoration:underline' href='https://www.tiktok.com/en/privacy-policy?lang=en' target='_blank' data-saferedirecturl='https://www.google.com/url?q=https://www.tiktok.com/en/privacy-policy?lang%3Den&amp;source=gmail&amp;ust=1748939493036000&amp;usg=AOvVaw0hFhnlGXiRqHVjOsXJqeZU'>Privacy Policy
                            </a>
                        </div>
                        <div>TikTok Clone by Thien Le, FPT University, HCMC</div>
                        </div>
                        </div>
                    </div>
                    </body>
                </html>";
        }
        public string GenerateChangePasswordEmailBody(string code, string name)
        {
            return $@"
                <!DOCTYPE html>
                <html>
                <head>
                </head>
                <body>
                    <div style='display: flex'>
                        <div style='width: 100%; max-width: 440px; padding: 0px 20px'>
                            <div style='width: 100%; padding: 40px 7px'>
                                <img width='150px' src='https://ci3.googleusercontent.com/meips/ADKq_NYK420UkUAFUsKIvzZlahWkg41X9bA1QHf8TAGuZ6ym8m5XYTWJ2Ee90ro6vYswm83-RT1a0AIsN9w5idF4Z_lvhqDOa86bd0GwD60T3s6Dn0-spVP2AbIsrT452YMV-LIR2a1b2k_SiU-YxQ=s0-d-e1-ft#http://p16-tiktokcdn-com.akamaized.net/obj/tiktok-obj/f70f9d0dc867d6c71ce2cd684a0ffff0' />
                            </div>

                            <div style='max-width: 100%; background-color: #f1f1f1; padding: 20px 16px; font-weight: bold; font-size: 20px; color: rgb(22,24,35)'>
                                Change your password
                            </div>

                            <div style='max-width: 100%; background-color: #f8f8f8; padding: 24px 16px; font-size: 17px; color: rgba(22,24,35,0.75); line-height: 20px'>
                                <p style='margin-bottom: 20px; color: rgba(22,24,35,0.5)'>
                                    This is an automatically generated email, please do not reply.
                                </p>
                                <p style='margin-bottom: 20px'>Hi {name},</p>
                                <p style='margin-bottom: 20px'>To change your password, enter this verification code in the TikTok app:</p>
                                <p style='margin-bottom: 20px; color: rgb(22,24,35); font-weight: bold; font-size: 20px; line-height: 24px'>{code}</p>
                                <p style='margin-bottom: 20px'>Verification codes expire after 48 hours.</p>
                                <p style='margin-bottom: 20px'>If you didn't request this change, ignore this message and your password will remain the same.</p>
                                <p>Best,</p>
                                <p>TikTok Support Team</p>
                                <p style='word-break: break-all'>
                                    TikTok Help Center:
                                    <a style='color: rgb(0,91,158)' href='https://support.tiktok.com/' target='_blank'>
                                        https://support.tiktok.com/
                                    </a>
                                </p>
                            </div>

                            <div style='max-width: 100%; padding: 40px 16px 20px; font-size: 15px; color: rgba(22,24,35,0.5); line-height: 18px'>
                                <div>Have a question?</div>
                                <div style='margin-bottom: 20px'>
                                    Check out our help center or contact us in the app using
                                    <span style='color: rgb(0,91,158); font-weight: bold'>Settings &gt; Report a Problem.</span>
                                </div>
                            </div>

                            <div style='border: 0; background-color: rgba(0,0,0,0.12); height: 1px; margin-bottom: 16px'></div>

                            <div style='color: rgba(22,24,35,0.5); margin: 20px 16px 40px 16px; font-size: 12px; line-height: 18px'>
                                <div>This email was generated for {name}.</div>
                                <div style='word-break: break-all'>
                                    <a style='color: rgba(22,24,35,0.5); text-decoration: underline' href='https://www.tiktok.com/en/privacy-policy?lang=en' target='_blank'>
                                        Privacy Policy
                                    </a>
                                </div>
                                <div>TikTok Clone by Thien Le, FPT University, HCMC</div>
                            </div>
                        </div>
                    </div>
                </body>
                </html>";
        }
    }
}
