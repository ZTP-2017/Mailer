using FluentMailer.Interfaces;
using System.Threading.Tasks;
using Scheduler.Mailer.Interfaces;

namespace Scheduler.Mailer
{
    public class MailService : IMailService
    {
        private readonly IFluentMailer _fluentMailer;

        public MailService(IFluentMailer fluentMailer)
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
