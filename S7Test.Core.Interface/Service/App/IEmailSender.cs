using System.Threading.Tasks;

namespace S7Test.Core.Interface.Service.App
{
    public interface IEmailSender
    {
        Task SendEmailAsync(string email, string subject, string message);
    }
}
