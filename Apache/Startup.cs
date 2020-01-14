using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Apache.Startup))]
namespace Apache
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
