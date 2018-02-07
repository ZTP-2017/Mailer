using System;
using FluentMailer.Interfaces;
using System.Threading.Tasks;
using Scheduler.Logger;

namespace Scheduler.Mailer
{
    public class MailService : IMailService
    {
        private readonly IFluentMailer _fluentMailer;
        private readonly ILoggerService _loggerService;

        public MailService(IFluentMailer fluentMailer, ILoggerService loggerService)
        {
            _fluentMailer = fluentMailer;
            _loggerService = loggerService;
        }

        public async Task SendEmail(string email, string subject, string body)
        {
            try
            {
                await _fluentMailer.CreateMessage()
                    .WithViewBody(body)
                    .WithReceiver(email)
                    .WithSubject(subject)
                    .SendAsync();

                _loggerService.CreateLog(LoggerService.LogType.Info, "Message was sent", null);
            }
            catch (Exception ex)
            {
                _loggerService.CreateLog(LoggerService.LogType.Warning, "Messages was not sent", ex);
            }
            
        }
    }
}
