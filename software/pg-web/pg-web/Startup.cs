using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(pg_web.Startup))]
namespace pg_web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
