using System;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Mebeller.Data.Services.Interfaces;
using Microsoft.Extensions.Configuration;

namespace Mebeller.Data.Services;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;

    public EmailService(IConfiguration configuration) => _configuration = configuration;

    public async Task SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false)
    {
        using var client = new SmtpClient(_configuration["EmailSettings:Host"], Convert.ToInt32(_configuration["EmailSettings:Port"]))
        {
            EnableSsl = _configuration.GetValue<bool>("EmailSettings:EnableSsl"),
            Credentials = new NetworkCredential(_configuration["EmailSettings:Username"], _configuration["EmailSettings:Password"])
        };

        var emailMessage = new MailMessage(_configuration["EmailSettings:From"], toEmail, subject, message)
        {
            IsBodyHtml = isMessageHtml
        };

        await client.SendMailAsync(emailMessage);
    }
}