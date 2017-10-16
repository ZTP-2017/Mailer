using Topshelf;

namespace Scheduler
{
    internal static class ConfigureService
    {
        internal static void Configure()
        {
            HostFactory.Run(configure =>
            {
                configure.Service<MailerService>(service =>
                {
                    service.ConstructUsing(s => new MailerService());
                    service.WhenStarted(s => s.Start());
                    service.WhenStopped(s => s.Stop());
                });

                configure.RunAsLocalSystem();
                configure.SetServiceName("MailerService");
                configure.SetDisplayName("Mailer Service");
                configure.SetDescription("Mailer service  ZTP");
            });
        }
    }
}