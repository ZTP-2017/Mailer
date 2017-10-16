using Hangfire;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(Scheduler.Startup))]
namespace Scheduler
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            GlobalConfiguration.Configuration
                .UseSqlServerStorage(@"Server=ZTP\SQLEXPRESS;Database=HangfireDb;Trusted_Connection=True;");

            app.UseHangfireDashboard();
            app.UseHangfireServer();
        }
    }
}
