using System;
using System.Collections.Generic;
using System.Linq;
using Scheduler.Data;
using Scheduler.Logger;
using Scheduler.Mailer;
using Scheduler.Models;

namespace Scheduler
{
    public class Sender : ISender
    {
        private static int _skipMessagesCount;
        private static List<Message> _messages;

        private readonly IMailService _mailService;
        private readonly IDataService _dataService;
        private readonly ILoggerService _loggerService;

        public Sender(IMailService mailService, IDataService dataService, ILoggerService loggerService)
        {
            _mailService = mailService;
            _dataService = dataService;
            _loggerService = loggerService;
        }

        public void LoadAllMessagesFromFile(string path)
        {
            _messages = _dataService.GetAllMessages<Message>(path);
        }

        public void SendEmails()
        {
            try
            {
                _loggerService.CreateLog(LoggerService.LogType.Info, "Get data from file", null);
                var messages = GetMessages(100);

                messages.ForEach(message =>
                {
                    _mailService.SendEmail(message.Email, message.Body, message.Subject);
                });
            }
            catch (Exception ex)
            {
                _loggerService.CreateLog(LoggerService.LogType.Error, "Messages send error", ex);
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
