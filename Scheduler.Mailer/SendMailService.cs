using FluentMailer.Interfaces;
using System.Threading.Tasks;

namespace Scheduler.Mailer
{
    public interface ISendMailService
    {
        Task SendEmail(string email, string body, string subject);
    }

    public class SendMailService : ISendMailService
    {
        private readonly IFluentMailer _fluentMailer;

        public SendMailService(IFluentMailer fluentMailer)
        {
            _fluentMailer = fluentMailer;
        }

        public async Task SendEmail(string email, string subject, string body)
        {
            await _fluentMailer.CreateMessage()
                .WithViewBody(body)
                .WithReceiver(email)
                .WithSubject(subject)
                .SendAsync();
        }
    }
}
