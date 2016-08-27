using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(VendeAgroWeb.Startup))]
namespace VendeAgroWeb
{
    public partial class Startup
    {
        private static AplicacionUsuariosManager _usuariosManager;
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            _usuariosManager = new AplicacionUsuariosManager(new Models.VendeAgroEntities());
        }

        public static AplicacionUsuariosManager GetAplicacionUsuariosManager()
        {
            return _usuariosManager;
        }
    }
}
