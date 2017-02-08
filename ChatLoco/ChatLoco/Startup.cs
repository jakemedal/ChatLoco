using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(ChatLoco.Startup))]
namespace ChatLoco
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
