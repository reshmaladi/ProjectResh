using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(HackathonPMA.Startup))]
namespace HackathonPMA
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
