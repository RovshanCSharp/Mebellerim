using System.Threading.Tasks;

namespace Mebeller.Data.Services.Interfaces
{
    public interface IEmailService
    {
        public Task SendEmailAsync(string toEmail, string subject, string message, bool isMessageHtml = false);
        
    }
}
