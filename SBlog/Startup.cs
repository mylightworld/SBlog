using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(SBlog.Startup))]
namespace SBlog
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
