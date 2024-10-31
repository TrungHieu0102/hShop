using Application.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Mail;
using System.Net;
using Application.Common.TemplateEmail;
using Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace Application.Services
{
    public class EmailService(IConfiguration config, UserManager<User> userManager) : IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string body, bool isBodyHtml)
        {
            string mailServer = config["EmailSettings:MailServer"]!;
            string fromEmail = config["EmailSettings:FromEmail"]!;
            string password = config["EmailSettings:Password"]!;
            int port = int.Parse(config["EmailSettings:MailPort"]!);
            var client = new SmtpClient(mailServer, port)
            {
                Credentials = new NetworkCredential(fromEmail, password),
                EnableSsl = true,
            };
            var mailMessage = new MailMessage(fromEmail, toEmail, subject, body)
            {
                IsBodyHtml = isBodyHtml
            };
            return client.SendMailAsync(mailMessage);
        }
        public async Task SendConfirmationEmail(string email, User user)
        {
            var token = await userManager.GenerateEmailConfirmationTokenAsync(user);
            var confirmationLink = $"http://localhost:5000/confirm-email?UserId={user.Id}&Token={token}";
            var body = GenerateEmailBody.GetEmailConfirmationBody(user.GetFullName(), confirmationLink);
            await SendEmailAsync(email, "Xác nhận đăng ký tài khoản", body, true);
        }
    }
}
