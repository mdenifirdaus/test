using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(PaymentDuration.Startup))]
namespace PaymentDuration
{
    public partial class Startup {
        public void Configuration(IAppBuilder app) {
            ConfigureAuth(app);
        }
    }
}
