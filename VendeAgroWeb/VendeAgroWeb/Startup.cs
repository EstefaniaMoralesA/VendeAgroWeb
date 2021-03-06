﻿using Microsoft.Owin;
using Openpay;
using Owin;
using System;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Web;
using VendeAgroWeb.Models;

[assembly: OwinStartup(typeof(VendeAgroWeb.Startup))]
namespace VendeAgroWeb
{
    public partial class Startup
    {
        private static AplicacionUsuariosManager _usuariosManager;
        private static ServicioEmail _servicioEmail;
        private static string _baseUrl;
        private static OpenpayAPI api;

        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
            _usuariosManager = new AplicacionUsuariosManager();
            _servicioEmail = new ServicioEmail();
            _baseUrl = "https://www.mercampo.mx";
            api = new OpenpayAPI("sk_f4ff6c6ddfb04f6d817e5a1e49c759dd", "mkvaxqzeaclhhptjqtd4", true);
        }

        public static OpenpayAPI OpenPayLib => api;

        public static string getBaseUrl() => _baseUrl;

        public static AplicacionUsuariosManager GetAplicacionUsuariosManager() => _usuariosManager;

        public static CarritoDeCompra GetCarritoDeCompra(HttpCookieCollection Cookies)
        {
            if (Cookies["carritoVendeAgro"] != null)
            {
                var carrito = Cookies["carritoVendeAgro"]["token"];
                if (carrito != null)
                {
                    return ReadToObject(carrito);
                }
            }

            return new CarritoDeCompra();
        }

        public static string SerializeCarrito(CarritoDeCompra carrito)
        {
            //Create User object.

            //Create a stream to serialize the object to.
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(CarritoDeCompra));
            ser.WriteObject(ms, carrito);
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        public static string SerializeResultadoCargo(ResultadoCargo resultado)
        {
            //Create a stream to serialize the object to.
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResultadoCargo));
            ser.WriteObject(ms, resultado);
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        public static string SerializeResultadoAgregarTarjeta(ResultadoAgregarTarjeta resultado)
        {
            //Create a stream to serialize the object to.
            MemoryStream ms = new MemoryStream();

            // Serializer the User object to the stream.
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(ResultadoAgregarTarjeta));
            ser.WriteObject(ms, resultado);
            byte[] json = ms.ToArray();
            ms.Close();
            return Encoding.UTF8.GetString(json, 0, json.Length);
        }

        public static CarritoDeCompra ReadToObject(string json)
        {
            MemoryStream ms = new MemoryStream(Encoding.UTF8.GetBytes(json));
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(CarritoDeCompra));
            var carrito = ser.ReadObject(ms) as CarritoDeCompra;
            ms.Close();
            return carrito;
        }

        public static void UpdateCarritoCookie(CarritoDeCompra carrito, HttpResponseBase response)
        {
            var serializedCarrito = Startup.SerializeCarrito(carrito);
            AplicacionUsuariosManager.setCookie("carritoVendeAgro", serializedCarrito, response);
        }

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
