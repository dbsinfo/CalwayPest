using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Volo.Abp.DependencyInjection;

namespace CalwayPest.Web.Services
{
    public interface ICustomEmailSender : ITransientDependency
    {
        Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml = true);
    }

    public class CustomEmailSender : ICustomEmailSender
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<CustomEmailSender> _logger;

        public CustomEmailSender(IConfiguration configuration, ILogger<CustomEmailSender> logger)
        {
            _configuration = configuration;
            _logger = logger;
        }

        public async Task SendEmailAsync(string to, string subject, string body, bool isBodyHtml = true)
        {
            try
            {
                var smtpHost = _configuration["AbpMailKit:Smtp:Host"];
                var smtpPort = int.Parse(_configuration["AbpMailKit:Smtp:Port"] ?? "587");
                var smtpUsername = _configuration["AbpMailKit:Smtp:UserName"];
                var smtpPassword = _configuration["AbpMailKit:Smtp:Password"];
                var enableSsl = bool.Parse(_configuration["AbpMailKit:Smtp:EnableSsl"] ?? "true");
                var fromAddress = _configuration["Settings:Abp:Mailing:DefaultFromAddress"] ?? smtpUsername ?? string.Empty;
                var fromDisplayName = _configuration["Settings:Abp:Mailing:DefaultFromDisplayName"] ?? "Calway Pest Control";

                _logger.LogInformation("Attempting to send email to {To} using SMTP {Host}:{Port}", to, smtpHost, smtpPort);

                using (var smtpClient = new SmtpClient(smtpHost, smtpPort))
                {
                    smtpClient.Credentials = new NetworkCredential(smtpUsername, smtpPassword);
                    smtpClient.EnableSsl = enableSsl;
                    smtpClient.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtpClient.Timeout = 30000; // 30 seconds

                    using (var mailMessage = new MailMessage())
                    {
                        mailMessage.From = new MailAddress(fromAddress, fromDisplayName);
                        mailMessage.To.Add(to);
                        mailMessage.Subject = subject;
                        mailMessage.Body = body;
                        mailMessage.IsBodyHtml = isBodyHtml;

                        await smtpClient.SendMailAsync(mailMessage);
                        _logger.LogInformation("Email sent successfully to {To}", to);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {To}. Error: {ErrorMessage}", to, ex.Message);
                throw;
            }
        }
    }
}
