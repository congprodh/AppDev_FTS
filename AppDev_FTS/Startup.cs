using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AppDev_FTS.Startup))]
namespace AppDev_FTS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
