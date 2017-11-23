using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Scheduler.Data;
using Scheduler.Mailer;
using Scheduler.Models;
using Serilog;

namespace Scheduler
{
    public class Sender : ISender
    {
        private readonly IMailService _mailService;
        private readonly IDataService _dataService;

        private Timer _timer;

        public Sender(IMailService mailService, IDataService dataService)
        {
            _mailService = mailService;
            _dataService = dataService;
        }

        public void SendEmails()
        {
            try
            {
                Log.Information("Get data from file");

                var messages = GetNotSendedMessages();
                if (messages.Count == 0) return;

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                foreach (var message in messages)
                {
                    _mailService.SendEmail(message.Email, message.Body, message.Subject);
                    message.Status = "sent";
                }

                _dataService.UpdateData(messages, Consts.FilePath);
                Log.Information("Messages send");

                stopWatch.Stop();

                _timer = new Timer(60000 - stopWatch.Elapsed.Milliseconds);
                _timer.Elapsed += Timer_Elapsed;
                _timer.AutoReset = true;
                _timer.Enabled = true;
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Messages send error: ");
            }
        }

        private List<Message> GetNotSendedMessages()
        {
            var data = _dataService.GetData<Message>(Consts.FilePath);
            var notSendMessages = data.Where(x => string.IsNullOrEmpty(x.Status)).ToList();

            return notSendMessages.Count() > 100 ? 
                notSendMessages.Take(100).ToList() : 
                notSendMessages;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SendEmails();
        }
    }
}
