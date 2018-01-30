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
        private static int _skipMessagesCount;
        private static List<Message> _messages;

        private readonly IMailService _mailService;
        private readonly IDataService _dataService;

        public Sender(IMailService mailService, IDataService dataService)
        {
            _mailService = mailService;
            _dataService = dataService;
        }

        public void LoadAllMessagesFromFile(string path)
        {
            _messages = _dataService.GetAllMessages<Message>(path);
        }

        public void SendEmails()
        {
            try
            {
                Log.Information("Get data from file");
                var messages = GetMessages(100);

                messages.ForEach(message =>
                {
                    _mailService.SendEmail(message.Email, message.Body, message.Subject);
                    Log.Information($"Message {message.Subject} to {message.Email} was sent");
                });
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Messages send error: ");
            }
        }

        public void SetSkipValue(int value)
        {
            _skipMessagesCount = value;
        }

        private List<Message> GetMessages(int count)
        {
            var messages = _messages.Skip(_skipMessagesCount).Take(count).ToList();

            _skipMessagesCount += messages.Count;

            return messages;
        }
    }
}
