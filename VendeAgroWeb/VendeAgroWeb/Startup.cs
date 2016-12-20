using Microsoft.Owin;
using Owin;
using System;
using VendeAgroWeb.Models;
using Conekta;

[assembly: OwinStartupAttribute(typeof(VendeAgroWeb.Startup))]
namespace VendeAgroWeb
{
    public partial class Startup
    {
        private static AplicacionUsuariosManager _usuariosManager;
        private static CarritoDeCompra _carrito;
        private static ServicioEmail _servicioEmail;
        private static string _baseUrl;
        private static ConektaLib _conektaLib;

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            _usuariosManager = new AplicacionUsuariosManager();
            _carrito = new CarritoDeCompra();
            _servicioEmail = new ServicioEmail();
            _baseUrl = "http://localhost:50827";
            _conektaLib = new ConektaLib("key_G4trRCLgCH4zYs5bEDDyWAQ");
        }

        public static string getBaseUrl() => _baseUrl;

        public static AplicacionUsuariosManager GetAplicacionUsuariosManager() => _usuariosManager;

        public static CarritoDeCompra GetCarritoDeCompra() => _carrito;

        public static ServicioEmail GetServicioEmail() => _servicioEmail;

        public static ConektaLib GetConektaLib() => _conektaLib;

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
