using Autofac;
using FluentMailer.Factory;
using FluentMailer.Interfaces;

namespace Scheduler.Mailer
{
    public class MailerModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            builder.RegisterType<MailService>().As<IMailService>();
            builder.Register(c => FluentMailerFactory.Create()).As<IFluentMailer>();
        }
    }
}
