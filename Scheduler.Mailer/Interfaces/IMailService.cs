using System.Threading.Tasks;

namespace Scheduler.Mailer.Interfaces
{
    public interface IMailService
    {
        Task SendEmail(string email, string body, string subject);
    }
}
