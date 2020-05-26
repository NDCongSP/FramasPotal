using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(FramasVietNam.Startup))]
namespace FramasVietNam
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
