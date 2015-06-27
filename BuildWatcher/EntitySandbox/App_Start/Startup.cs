using EntitySandbox.Hubs;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(EntitySandbox.Startup))]
namespace EntitySandbox
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
