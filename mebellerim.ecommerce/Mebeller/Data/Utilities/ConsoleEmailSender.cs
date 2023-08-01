using System;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Identity.UI.Services;

namespace Mebeller.Data.Utilities;

public class ConsoleEmailSender : IEmailSender
{
    public Task SendEmailAsync(string emailAddress,
        string subject, string htmlMessage)
    {
        Console.WriteLine("---New Email----");
        Console.WriteLine($"To: {emailAddress}");
        Console.WriteLine($"Subject: {subject}");
        Console.WriteLine(HttpUtility.HtmlDecode(htmlMessage));
        Console.WriteLine("-------");
        return Task.CompletedTask;
    }
}