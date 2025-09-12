using System.Net;
using System.Net.Mail;
using Uber.Uber.Application.Interfaces;
using MailKit.Net.Smtp;

using MailKit.Security;
using MimeKit;
using static Org.BouncyCastle.Math.EC.ECCurve;
namespace Uber.Uber.Infrastructure.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration config;
        public EmailService(IConfiguration configuration)
        {
            this.config = configuration;
        }
        public async Task SendEmailAsync(string to, string subject, string body)
        {
            var emailSettings = config.GetSection("Email");

            var email = new MimeMessage();
            email.From.Add(new MailboxAddress(emailSettings["FromName"], emailSettings["FromEmail"]));
            email.To.Add(MailboxAddress.Parse(to));
            email.Subject = subject;
            email.Body = new TextPart("html") { Text = body };

            using var smtp = new MailKit.Net.Smtp.SmtpClient();

            try
            {
                await smtp.ConnectAsync(
                    emailSettings["SmtpServer"],
                    int.Parse(emailSettings["SmtpPort"]),
                    SecureSocketOptions.StartTls
                );

                await smtp.AuthenticateAsync(
                    emailSettings["Username"],
                    emailSettings["Password"]
                );

                await smtp.SendAsync(email);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Email sending failed: {ex.Message}");
                throw;
            }
            finally
            {
                await smtp.DisconnectAsync(true);
            }
        }

    }
    }

