using Autofac;
using Hangfire;
using Microsoft.Win32;
using Scheduler.Mailer;
using Scheduler.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Timers;

namespace Scheduler
{
    public class MailerService
    {
        private IMessage message;
        private ISendMailService sendMailService;
        private Timer timer;
        private List<Message> sentData;

        public void Start()
        {
            try
            {
                Data.LoadDataFromFile();
                sentData = new List<Message>();

                using (var scope = Program.Container.BeginLifetimeScope())
                {
                    message = scope.Resolve<IMessage>();
                    sendMailService = scope.Resolve<ISendMailService>();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Start service error");
            }

            Log.Information("Start service");
            LoadData();
        }

        public void Stop()
        {
            Log.Information("Stop service");
        }

        private void LoadData()
        {
            var data = message.GetData();

            if (data.Count == 0)
            {
                timer = new Timer(60000 * 10); //10 min
                timer.Elapsed += TryGetData;
                timer.AutoReset = true;
                timer.Enabled = true;
            }
            else
            {
                SendEmails(data);
            }
        }

        private void TryGetData(Object source, ElapsedEventArgs e)
        {
            LoadData();
        }

        public async void SendEmails(List<Message> data)
        {
            if (data.Count() <= 100)
            {
                foreach (var message in data)
                {
                    await sendMailService.SendEmail(message.Email, message.Body, message.Subject);
                    message.Status = "sent";
                    sentData.Add(message);
                }

                Data.UpdateFile(sentData);
            }
            else
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                for (int i = 0; i < 100; i++)
                {
                    await sendMailService.SendEmail(data[i].Email, data[i].Body, data[i].Subject);
                    data[i].Status = "sent";
                }

                stopWatch.Stop();

                timer = new Timer(60000 - stopWatch.Elapsed.Milliseconds);
                timer.Elapsed += Timer_Elapsed;
                timer.AutoReset = true;
                timer.Enabled = true;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SendEmails(message.GetData());
        }
    }
}
