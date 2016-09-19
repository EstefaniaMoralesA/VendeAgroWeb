using Microsoft.Owin;
using Owin;
using System.Web;

[assembly: OwinStartupAttribute(typeof(VendeAgroWeb.Startup))]
namespace VendeAgroWeb
{
    public partial class Startup
    {
        private static AplicacionUsuariosManager _usuariosManager;
        private static ServicioEmail _servicioEmail;
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            _usuariosManager = new AplicacionUsuariosManager();
            _servicioEmail = new ServicioEmail();
        }

        public static AplicacionUsuariosManager GetAplicacionUsuariosManager()
        {
            return _usuariosManager;
        }

        public static ServicioEmail GetServicioEmail()
        {
            return _servicioEmail;
        }
    }
}
