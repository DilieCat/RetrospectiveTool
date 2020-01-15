using System.Threading.Tasks;
using Retrospective_Back_End.Utils;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Retrospective_Back_End.Services
{
    public static class SendGridEmailService
    {
        private static readonly string RecoveryUrl = "https://true-lime.herokuapp.com/updatepassword/";

        public static async Task ExecuteSendRecoveryEmail(string email, string token)
        {
            var apiKey = ConfigConstants.SendGridKey;
            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(ConfigConstants.ServiceEmail, ConfigConstants.ServiceName);
            var subject = ConfigConstants.Subject;
            var to = new EmailAddress(email);
            var plainTextContent = ConfigConstants.DescriptionDutch + GetRecoveryUrl(token);
            var htmlContent = $"<strong>{ConfigConstants.DescriptionDutch + GetRecoveryUrl(token)}</strong>";
            var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
            var response = await client.SendEmailAsync(msg);
        }

        private static string GetRecoveryUrl(string token)
        {
            return RecoveryUrl + token;
        }
    }
}
