using Autofac;
using Hangfire;
using Scheduler.Mailer;
using Scheduler.Models;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Microsoft.Owin.Hosting;
using System.Timers;

namespace Scheduler
{
    public class MailerService
    {
        private IDisposable application;
        private IMessage message;
        private ISendMailService sendMailService;
        private Timer timer;
        private List<Message> updatedData;

        public void Start()
        {
            try
            {
                application = WebApp.Start<Startup>("http://localhost:8080");

                RecurringJob.AddOrUpdate(
                    () => Log.Information("Start service"),
                    Cron.Minutely()
                );

                Data.LoadDataFromFile();

                using (var scope = Program.Container.BeginLifetimeScope())
                {
                    message = scope.Resolve<IMessage>();
                    sendMailService = scope.Resolve<ISendMailService>();
                }

                updatedData = message.GetData();

                LoadData();
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Start service error");
            }
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
                SendEmails(data.Where(x => string.IsNullOrEmpty(x.Status) || x.Status != "sent").ToList());
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
                    Log.Information("Email sent to " + message.Email);
                }

                Data.UpdateFile(updatedData);
            }
            else
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();

                for (int i = 0; i < 100; i++)
                {
                    await sendMailService.SendEmail(data[i].Email, data[i].Body, data[i].Subject);
                    data[i].Status = "sent";
                    Log.Information("Email sent to " + data[i].Email);
                }

                Data.UpdateFile(updatedData);

                stopWatch.Stop();

                timer = new Timer(60000 - stopWatch.Elapsed.Milliseconds);
                timer.Elapsed += Timer_Elapsed;
                timer.AutoReset = true;
                timer.Enabled = true;
            }
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            SendEmails(message.GetData().Where(x => string.IsNullOrEmpty(x.Status) || x.Status != "sent").ToList());
        }
    }
}
