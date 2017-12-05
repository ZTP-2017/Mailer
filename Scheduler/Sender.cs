using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Timers;
using Scheduler.Data.Interfaces;
using Scheduler.Interfaces;
using Scheduler.Mailer.Interfaces;
using Scheduler.Models;
using Serilog;

namespace Scheduler
{
    public class Sender : ISender
    {
        private readonly IMailService _mailService;

        private Timer _timer;
        private List<Message> _messages;

        public Sender(IMailService mailService, IDataService dataService)
        {
            _mailService = mailService;
            _messages = dataService.GetAllMessages<Message>(Settings.DataFilePath);
        }

        public void SendEmails()
        {
            try
            {
                Log.Information("Get data from file");

                var messages = GetMessages();
                if (messages.Count == 0)
                {
                    Log.Information("Messages send");
                    return;
                }

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                foreach (var message in messages)
                {
                    _mailService.SendEmail(message.Email, message.Body, message.Subject);
                }

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

        private List<Message> GetMessages()
        {
            var messages = _messages.Count() > 100 ?
                _messages.Take(100).ToList() :
                _messages;

            _messages = _messages.Where(x => messages.All(y => x != y)).ToList();

            return messages;
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SendEmails();
        }
    }
}
