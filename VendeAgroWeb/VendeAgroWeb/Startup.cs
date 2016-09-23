using Microsoft.Owin;
using Owin;
using System;
using System.Web;

[assembly: OwinStartupAttribute(typeof(VendeAgroWeb.Startup))]
namespace VendeAgroWeb
{
    public partial class Startup
    {
        private static AplicacionUsuariosManager _usuariosManager;
        private static ServicioEmail _servicioEmail;
        private static string _baseUrl;
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            _usuariosManager = new AplicacionUsuariosManager();
            _servicioEmail = new ServicioEmail();
            _baseUrl = "http://localhost:50827";
        }

        public static string getBaseUrl()
        {
            return _baseUrl;
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
