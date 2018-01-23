using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(testapp.Startup))]
namespace testapp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
