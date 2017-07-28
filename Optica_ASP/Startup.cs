using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Optica_ASP.Startup))]
namespace Optica_ASP
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
