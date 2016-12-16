using Microsoft.Owin;
using Owin;
using System;
using System.Web;
using VendeAgroWeb.Models;

[assembly: OwinStartupAttribute(typeof(VendeAgroWeb.Startup))]
namespace VendeAgroWeb
{
    public partial class Startup
    {
        private static AplicacionUsuariosManager _usuariosManager;
        private static CarritoDeCompra _carrito;
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

        public static CarritoDeCompra GetCarritoDeCompra() {
            return _carrito;
        }

        public static ServicioEmail GetServicioEmail()
        {
            return _servicioEmail;
        }

        public static void OpenDatabaseConnection(MercampoEntities _dbContext)
        {
            try
            {
                _dbContext.Database.Connection.Open();
            }
            catch(Exception e)
            {
                return;
            }
        }
    }
}
