using System.Threading.Tasks;

namespace Scheduler.Mailer
{
    public interface IMailService
    {
        Task SendEmail(string email, string body, string subject);
    }
}
