using System.Threading.Tasks;
using Retrospective_Back_End.Utils;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Retrospective_Back_End.Services
{
    public static class SendGridEmailService
    {
        private static async Task ExecuteSendRecoveryEmail(string email, string name)
        {
            var apiKey = ConfigConstants.SendGridKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(ConfigConstants.ServiceEmail, ConfigConstants.ServiceName);
            var subject = ConfigConstants.Subject;
            var to = new EmailAddress(email, name);
            var plainTextContent = "and easy to do anywhere, even with C#";
            var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }
    }
}
