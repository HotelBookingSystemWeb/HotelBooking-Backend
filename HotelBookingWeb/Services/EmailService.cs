using HotelBookingAPI.Data;
using HotelBookingWeb.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Mail;

namespace HotelBookingWeb.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;
        private readonly AppDbContext _context;

        public EmailService(IConfiguration configuration, AppDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task<bool> SendEmailAsync(string toEmail, string subject, string body)
        {
            try
            {
                Console.WriteLine($"[EmailService] Sending email to: {toEmail}");

                var smtpHost = _configuration["EmailSettings:SmtpHost"];
                var smtpPort = _configuration["EmailSettings:SmtpPort"];
                var senderName = _configuration["EmailSettings:SenderName"];
                var senderEmail = _configuration["EmailSettings:SenderEmail"];
                var senderPassword = _configuration["EmailSettings:SenderPassword"];

                if (string.IsNullOrWhiteSpace(smtpHost) ||
                    string.IsNullOrWhiteSpace(smtpPort) ||
                    string.IsNullOrWhiteSpace(senderEmail) ||
                    string.IsNullOrWhiteSpace(senderPassword))
                {
                    Console.WriteLine("[EmailService] Email configuration is missing.");
                    return false;
                }

                using var client = new SmtpClient(smtpHost, int.Parse(smtpPort))
                {
                    Credentials = new NetworkCredential(senderEmail, senderPassword),
                    EnableSsl = true
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(senderEmail, senderName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(toEmail);

                await client.SendMailAsync(message);

                Console.WriteLine("[EmailService] Email sent successfully.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService] Error while sending email: {ex.Message}");
                return false;
            }
        }

        public async Task<bool> SendPromotionEmailToAllUsersAsync(string subject, string body)
        {
            try
            {
                Console.WriteLine("[EmailService] Sending promotion email to all active users...");

                var users = await _context.Users
                    .Where(u => u.IsActive)
                    .Select(u => u.Email)
                    .ToListAsync();

                if (!users.Any())
                {
                    Console.WriteLine("[EmailService] No active users found.");
                    return false;
                }

                foreach (var email in users)
                {
                    var sent = await SendEmailAsync(email, subject, body);
                    Console.WriteLine($"[EmailService] Email to {email} status: {sent}");
                }

                Console.WriteLine("[EmailService] Promotion email processing completed.");
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[EmailService] Error while sending bulk promotion email: {ex.Message}");
                return false;
            }
        }
    }
}