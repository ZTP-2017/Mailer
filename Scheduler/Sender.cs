using System;
using System.Collections.Generic;
using System.Linq;
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
        private static int skipMessagesCount;
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

                foreach (var message in messages)
                {
                    _mailService.SendEmail(message.Email, message.Body, message.Subject);
                    Log.Information($"Message {message.Subject} to {message.Email} was sent");
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Messages send error: ");
            }
        }

        public void SetSkipValue(int value)
        {
            skipMessagesCount = value;
        }

        private List<Message> GetMessages()
        {
            var messages = _messages.Skip(skipMessagesCount).Take(100).ToList();

            skipMessagesCount += messages.Count;

            return messages;
        }
    }
}
