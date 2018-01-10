using Autofac;
using Scheduler.Mailer;
using Scheduler.Data;
using Serilog;
using System;
using Hangfire;
using Scheduler.Interfaces;
using Topshelf;
using Topshelf.Autofac;

namespace Scheduler
{
    public class Program
    {
        public static IContainer Container { get; set; }

        static void Main(string[] args)
        {
            ConfigureAutofac();
            ConfigureHangfire();
            ConfigureLogger();
            ConfigureTopshelf();

            Console.ReadKey();
        }

        private static void ConfigureAutofac()
        {
            var builder = new ContainerBuilder();

            builder.RegisterType<Scheduler>();
            
            builder.RegisterModule<MailerModule>();
            builder.RegisterModule<DataModule>();
            builder.RegisterType<Sender>();
            builder.RegisterType<Sender>().As<ISender>();

            Container = builder.Build();
        }

        private static void ConfigureHangfire()
        {
            GlobalConfiguration.Configuration.UseAutofacActivator(Container);
        }

        private static void ConfigureLogger()
        {
            Log.Logger = new LoggerConfiguration()
                .WriteTo.LiterateConsole()
                .WriteTo.RollingFile("log-{Date}.txt")
                .CreateLogger();
        }

        private static void ConfigureTopshelf()
        {
            HostFactory.Run(configure =>
            {
                configure.UseAutofacContainer(Container);
                configure.Service<Scheduler>(service =>
                {
                    service.ConstructUsingAutofacContainer();
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });
                
                configure.SetServiceName("MailerService");
                configure.SetDisplayName("Mailer Service");
                configure.SetDescription("Mailer service ZTP");
                configure.RunAsLocalService();
            });
        }
    }
}
