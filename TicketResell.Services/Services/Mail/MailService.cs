using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Repositories.Config;
using StackExchange.Redis;
using TicketResell.Repositories.Logger;

namespace TicketResell.Services.Services.Mail
{
    public class MailService : IMailService
    {
        private readonly AppConfig _config;
        private readonly IAppLogger _logger;
        private readonly string _smtpHost;
        private readonly int _smtpPort;
        private readonly string _smtpUsername;
        private readonly string _smtpPassword;
        private readonly string _fromEmail;
        private readonly string _fromDisplayName;
        private readonly IConnectionMultiplexer _redis;

        public MailService(IOptions<AppConfig> config, IAppLogger logger, IConnectionMultiplexer redis)
        {
            _config = config.Value;
            _logger = logger;
            _redis = redis;
            
            // Load email configuration from appsettings.json
            _smtpHost = _config.SmtpHost;
            _smtpPort = int.Parse(_config.SmtpPort);
            _smtpUsername = _config.Username;
            _smtpPassword = _config.Password;
            _fromEmail = _config.FromEmail;
            _fromDisplayName = _config.FromDisplayName;
        }

        public async Task<ResponseModel> SendOtpAsync(string to){
            string otp = GenerateNumericOTP(5);
            string body = $@"
    <!DOCTYPE html>
    <html lang='vi'>
    <head>
        <meta charset='UTF-8'>
        <meta name='viewport' content='width=device-width, initial-scale=1.0'>
        <title>Xác nhận đăng ký tài khoản TicketResell</title>
        <style>
            body, html {{
                margin: 0;
                padding: 0;
                font-family: 'Roboto', 'Helvetica Neue', Arial, sans-serif;
                background-color: #f0f7f0;
                line-height: 1.6;
                color: #333;
            }}
            .container {{
                max-width: 600px;
                margin: 40px auto;
                background-color: #ffffff;
                border-radius: 12px;
                overflow: hidden;
                box-shadow: 0 5px 25px rgba(0,0,0,0.1);
            }}
            .header {{
                background: linear-gradient(135deg, #2ecc71, #27ae60);
                color: white;
                text-align: center;
                padding: 40px 20px;
            }}
            .header h1 {{
                margin: 0;
                font-size: 32px;
                font-weight: 700;
                letter-spacing: 1px;
            }}
            .header p {{
                margin: 10px 0 0;
                font-size: 18px;
                opacity: 0.9;
            }}
            .content {{
                padding: 40px 30px;
                color: #333;
            }}
            .content h2 {{
                color: #2ecc71;
                margin-top: 0;
                font-size: 24px;
            }}
            .otp-code {{
                font-size: 36px;
                font-weight: bold;
                color: #2ecc71;
                text-align: center;
                margin: 30px 0;
                padding: 20px;
                background-color: #e0f2e0;
                border-radius: 8px;
                letter-spacing: 8px;
                box-shadow: 0 4px 10px rgba(46, 204, 113, 0.2);
            }}
            .footer {{
                background-color: #e8f6e8;
                text-align: center;
                padding: 25px;
                font-size: 14px;
                color: #555;
            }}
            .button {{
                display: inline-block;
                padding: 14px 28px;
                background-color: #2ecc71;
                color: white;
                text-decoration: none;
                border-radius: 50px;
                font-weight: bold;
                margin-top: 25px;
                transition: all 0.3s ease;
                text-transform: uppercase;
                letter-spacing: 1px;
            }}
            .button:hover {{
                background-color: #27ae60;
                transform: translateY(-2px);
                box-shadow: 0 5px 15px rgba(46, 204, 113, 0.4);
            }}
            .info-box {{
                background-color: #e8f6e8;
                border-left: 4px solid #2ecc71;
                padding: 15px;
                margin: 20px 0;
                border-radius: 4px;
            }}
            .highlight {{
                color: #2ecc71;
                font-weight: bold;
            }}
        </style>
    </head>
    <body>
        <div class='container'>
            <div class='header'>
                <h1>TicketResell</h1>
                <p>Xác nhận đăng ký tài khoản</p>
            </div>
            <div class='content'>
                <h2>Xin chào</h2>
                <p>Chúng tôi rất vui mừng chào đón bạn đến với cộng đồng TicketResell. Để hoàn tất quá trình đăng ký và bảo mật tài khoản của bạn, vui lòng sử dụng mã OTP dưới đây:</p>
                <div class='otp-code'>{otp}</div>
                <div class='info-box'>
                    <p><strong>Lưu ý quan trọng:</strong></p>
                    <ul>
                        <li>Mã OTP có hiệu lực trong vòng <span class='highlight'>1 phút</span>.</li>
                        <li>Không chia sẻ mã này với bất kỳ ai, kể cả nhân viên TicketResell.</li>
                        <li>Nếu bạn không yêu cầu mã này, vui lòng bỏ qua email và liên hệ ngay với chúng tôi.</li>
                    </ul>
                </div>
                <p>Tại TicketResell, chúng tôi cam kết mang đến cho bạn trải nghiệm mua bán vé an toàn và thuận tiện nhất. Hãy khám phá ngay các tính năng độc đáo của chúng tôi!</p>
                <p>Trân trọng,<br><strong>Đội ngũ TicketResell</strong></p>
            </div>
            <div class='footer'>
                <p>&copy; 2024 TicketResell. Tất cả các quyền được bảo lưu.</p>
                <p>Cần hỗ trợ? Liên hệ chúng tôi tại <a href='mailto:quanglmse184185@fpt.edu.vn' style='color: #2ecc71;'>quanglmse184185@fpt.edu.vn</a></p>
            </div>
        </div>
    </body>
    </html>";
            var response = await SendEmailAsync(to, "TicketResell: Here is your OTP code", body);
            if (response.StatusCode == 200){
                await CacheAccessKeyAsync("email_verification", to, otp, TimeSpan.FromMinutes(5));
                return ResponseModel.Success("Sucess");
            }
            else{
                return ResponseModel.Error(response.Message?? "Error");
            }
        }

        private async Task CacheAccessKeyAsync(string cacheName, string userId, string cacheKey, TimeSpan timeSpan)
        {
            var db = _redis.GetDatabase();
            await db.StringSetAsync($"{cacheName}:{userId}", cacheKey, timeSpan);
        }
        public static string GenerateNumericOTP(int length)
        {
            var random = new Random();
            var otp = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                otp.Append(random.Next(0, 10)); // Add a random digit (0-9)
            }

            return otp.ToString();
        }

        public async Task<ResponseModel> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_fromEmail, _fromDisplayName);
                    message.To.Add(new MailAddress(to));
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    using (var client = new SmtpClient(_smtpHost, _smtpPort))
                    {
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        client.EnableSsl = true;

                        await client.SendMailAsync(message);
                    }
                }

                _logger.LogInformation($"Email sent successfully to {to}");
                return ResponseModel.Success($"Email sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email to {to}: {ex.Message}");
                return ResponseModel.BadRequest("Failed to send email", ex.Message);
            }
        }

        public async Task<ResponseModel> SendEmailWithAttachmentAsync(string to, string subject, string body, string attachmentPath)
        {
            try
            {
                using (var message = new MailMessage())
                {
                    message.From = new MailAddress(_fromEmail, _fromDisplayName);
                    message.To.Add(new MailAddress(to));
                    message.Subject = subject;
                    message.Body = body;
                    message.IsBodyHtml = true;

                    if (File.Exists(attachmentPath))
                    {
                        message.Attachments.Add(new Attachment(attachmentPath));
                    }
                    else
                    {
                        return ResponseModel.BadRequest("Attachment file not found");
                    }

                    using (var client = new SmtpClient(_smtpHost, _smtpPort))
                    {
                        client.UseDefaultCredentials = false;
                        client.Credentials = new NetworkCredential(_smtpUsername, _smtpPassword);
                        client.EnableSsl = true;

                        await client.SendMailAsync(message);
                    }
                }

                _logger.LogInformation($"Email with attachment sent successfully to {to}");
                return ResponseModel.Success($"Email with attachment sent successfully to {to}");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error sending email with attachment to {to}: {ex.Message}");
                return ResponseModel.BadRequest("Failed to send email with attachment", ex.Message);
            }
        }
    }
}