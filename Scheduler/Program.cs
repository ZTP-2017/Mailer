using Autofac;
using FluentMailer.Factory;
using FluentMailer.Interfaces;
using Scheduler.Mailer;
using Scheduler.Models;
using Serilog;
using System;

namespace Scheduler
{
    class Program
    {
        public static IContainer Container { get; set; }

        static void Main(string[] args)
        {
            var builder = new ContainerBuilder();
            builder.RegisterType<Message>().As<IMessage>();
            builder.RegisterType<SendMailService>().As<ISendMailService>();
            builder.Register(c => FluentMailerFactory.Create()).As<IFluentMailer>();
            Container = builder.Build();

            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .WriteTo.RollingFile(Settings.LogsFilePath + "log-{Date}.txt")
                .CreateLogger();

            ConfigureService.Configure();

            Console.ReadKey();
        }
    }
}
