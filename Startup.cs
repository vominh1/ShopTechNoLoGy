using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ShopTechNoLoGy.StartupOwin))]

namespace ShopTechNoLoGy
{
    public partial class StartupOwin
    {
        public void Configuration(IAppBuilder app)
        {
            //AuthStartup.ConfigureAuth(app);
        }
    }
}
