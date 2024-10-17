using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;
        private readonly UserManager<User> _userManager;
        public EmailService( IConfiguration config,UserManager<User> userManager)
        {
            _config = config;
            _userManager = userManager;
        }
        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHTML)
        {
            string MailServer = _config["EmailSettings:MailServer"];
            string FromEmail = _config["EmailSettings:FromEmail"];
            string Password = _config["EmailSettings:Password"];
            int Port = int.Parse(_config["EmailSettings:MailPort"]);
            var client = new SmtpClient(MailServer, Port)
            {
                Credentials = new NetworkCredential(FromEmail, Password),
                EnableSsl = true,
            };
            MailMessage mailMessage = new MailMessage(FromEmail, toEmail, subject, body)
            {
                IsBodyHtml = isBodyHTML
            };
            return client.SendMailAsync(mailMessage);
        }
        public async Task SendConfirmationEmail(string email, User user)
        {
            var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"http://localhost:5000/confirm-email?UserId={user.Id}&Token={token}";
            await SendEmailAsync(email, "Xác nhận đăng ký tài khoản", $"Vui lòng ấn vào <a href='{confirmationLink}'>đây</a>;.", true);
        }
    }
}
