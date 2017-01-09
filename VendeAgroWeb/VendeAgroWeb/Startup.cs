using Microsoft.Owin;
using Openpay;
using Owin;
using System;
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
        private static OpenpayAPI api;

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            _usuariosManager = new AplicacionUsuariosManager();
            _carrito = new CarritoDeCompra();
            _servicioEmail = new ServicioEmail();
            _baseUrl = "http://localhost:50827";
            api = new OpenpayAPI("sk_6f55e32ca0b74855b4ec592a56f5c152", "m4tpnfpemahz4tgdvadb");
        }

        public static OpenpayAPI OpenPayLib => api;

        public static string getBaseUrl() => _baseUrl;

        public static AplicacionUsuariosManager GetAplicacionUsuariosManager() => _usuariosManager;

        public static CarritoDeCompra GetCarritoDeCompra() => _carrito;

        public static ServicioEmail GetServicioEmail() => _servicioEmail;

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
