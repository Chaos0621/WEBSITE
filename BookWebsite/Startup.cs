using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BookWebsite.Startup))]
namespace BookWebsite
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
