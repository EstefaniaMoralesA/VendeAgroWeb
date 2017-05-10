using Openpay;
using Openpay.Entities;
using Openpay.Entities.Request;
using System;
using System.Data;
using System.Linq;
using System.Runtime.Serialization;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using VendeAgroWeb.Models;
using VendeAgroWeb.Models.Pagina;

namespace VendeAgroWeb
{
    public class AplicacionUsuariosManager
    {

        public async Task<LoginStatus> LoginAdministradorAsync(string email, string password)
        {
            HttpResponse response = HttpContext.Current.Response;
            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    var usuario = _dbContext.Usuario_Administrador.Where(u => u.email == email).FirstOrDefault();
                    if (usuario == null)
                    {
                        return LoginStatus.Incorrecto;
                    }

                    if (!usuario.confirmaEmail)
                    {
                        //TO DO: Reenviar token
                        return LoginStatus.ConfirmacionMail;
                    }

                    if (!usuario.activo || usuario.password.CompareTo(password) != 0)
                    {
                        return LoginStatus.Incorrecto;
                    }

                    usuario.tokenSesion = getToken();
                    setCookie("AdminVendeAgro", usuario.tokenSesion, response);
                    _dbContext.SaveChanges();
                    return LoginStatus.Exitoso;
                }

            });

        }

        internal async Task<ConfirmacionMailStatus> ConfirmarMailPortalAsync(string token)
        {
            return await Task.Run(() =>
            {
                return ConfirmacionMailStatus.MailConfirmado;
            });
        }

        public async Task<OlvidoContrasenaStatus> OlvidoContrasenaAdminAsync(string email)
        {
            return await Task.Run(async () =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);

                    if (_dbContext.Database.Connection.State != System.Data.ConnectionState.Open)
                    {
                        return OlvidoContrasenaStatus.Error;
                    }

                    var usuario = _dbContext.Usuario_Administrador.Where(u => u.email == email).FirstOrDefault();
                    if (usuario == null)
                    {
                        _dbContext.Database.Connection.Close();
                        return OlvidoContrasenaStatus.MailInexistente;
                    }

                    string mailMensaje = "<p>Estimado {0},</p>" +
                    "<p>Para cambiar tu contraseña da click <a href=\'" + Startup.getBaseUrl() + "/Administrador/CambiarContrasena?token=" + "{1}\'>AQUÍ</a></p>";

                    var result = await Startup.GetServicioEmail().SendAsync(string.Format(mailMensaje, usuario.nombre, usuario.password), "Recuperar Contraseña Mercampo", usuario.email);

                    _dbContext.Database.Connection.Close();
                    return OlvidoContrasenaStatus.MailEnviado;

                }
            });

        }

        public async Task<OlvidoContrasenaStatus> OlvidoContrasenaPortalAsync(string email)
        {
            return await Task.Run(async () =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);

                    if (_dbContext.Database.Connection.State != System.Data.ConnectionState.Open)
                    {
                        return OlvidoContrasenaStatus.Error;
                    }

                    var usuario = _dbContext.Usuarios.Where(u => u.email == email).FirstOrDefault();
                    if (usuario == null)
                    {
                        _dbContext.Database.Connection.Close();
                        return OlvidoContrasenaStatus.MailInexistente;
                    }

                    string mailMensaje = "<p>Estimado {0},</p>" +
                    "<p>Para cambiar tu contraseña da click <a href=\'" + Startup.getBaseUrl() + "/Portal/CambiarContrasena?token=" + "{1}\'>AQUÍ</a></p>";

                    var result = await Startup.GetServicioEmail().SendAsync(string.Format(mailMensaje, usuario.nombre, usuario.password), "Recuperar Contraseña Mercampo", usuario.email);

                    _dbContext.Database.Connection.Close();
                    return OlvidoContrasenaStatus.MailEnviado;

                }
            });

        }

        public async Task<CambiarContrasenaStatus> VerificarTokenCambiarContrasenaAdminAsync(string token)
        {
            return await Task.Run(() =>
            {
                if (token == null)
                {
                    return CambiarContrasenaStatus.TokenInvalido;
                }

                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != System.Data.ConnectionState.Open)
                    {
                        return CambiarContrasenaStatus.Error;
                    }

                    var usuario = _dbContext.Usuario_Administrador.Where(u => u.password == token).FirstOrDefault();

                    if (usuario == null)
                    {
                        _dbContext.Database.Connection.Close();
                        return CambiarContrasenaStatus.TokenInvalido;
                    }

                    _dbContext.Database.Connection.Close();
                    return CambiarContrasenaStatus.UrlValido;
                }
            });
        }

        public async Task<CambiarContrasenaStatus> CambiarContrasenaAdminAsync(string password, string token)
        {
            return await Task.Run(() =>
            {
                if (token == null)
                {
                    return CambiarContrasenaStatus.TokenInvalido;
                }

                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != System.Data.ConnectionState.Open)
                    {
                        return CambiarContrasenaStatus.Error;
                    }

                    var usuario = _dbContext.Usuario_Administrador.Where(u => u.password == token).FirstOrDefault();

                    if (usuario == null)
                    {
                        _dbContext.Database.Connection.Close();
                        return CambiarContrasenaStatus.TokenInvalido;
                    }

                    usuario.password = password;
                    _dbContext.SaveChanges();

                    _dbContext.Database.Connection.Close();
                    return CambiarContrasenaStatus.ContrasenaActualizada;
                }
            });
        }

        public async Task<RegistroStatus> RegistroUsuarioAsync(Models.Portal.RegistroViewModel model)
        {
            HttpResponse response = HttpContext.Current.Response;
            return await Task.Run(async () =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    var usuario = _dbContext.Usuarios.Where(u => u.email.Equals(model.Email, StringComparison.InvariantCulture)).FirstOrDefault();
                    if (usuario != null)
                    {
                        return RegistroStatus.MailOcupado;
                    }

                    usuario = _dbContext.Usuarios.Where(u => u.telefono.Equals(model.Celular, StringComparison.InvariantCulture)).FirstOrDefault();

                    if (usuario != null)
                    {
                        return RegistroStatus.TelefonoOcupado;
                    }

                    var tokenId = CrearClienteConektaId(model.Nombre, model.Apellidos, model.Email, model.Celular);
                    string tokenSesion = getToken();
                    string tokenEmail = getToken();
                    _dbContext.Usuarios.Add(new Usuario
                    {
                        nombre = model.Nombre,
                        apellidos = model.Apellidos,
                        telefono = model.Celular,
                        password = Hash(model.Password),
                        email = model.Email,
                        confirmaEmail = true,
                        tokenSesion = tokenSesion,
                        tokenEmail = tokenEmail,
                        idConekta = tokenId
                    });

                    _dbContext.SaveChanges();
                    setCookie("VendeAgroUser", tokenSesion, response);

                    var usuarioRegistrado = _dbContext.Usuarios.Where(u => u.email == model.Email).FirstOrDefault();
                    string mailMensaje = "<p>Estimado {0} gracias por registrarte en mercampo.mx</p>" +
                    "<p>Para completar tu registro y poder hacer login da click <a href=\'" + Startup.getBaseUrl() + "/Portal/ConfirmarMail?token=" + "{1}\'>AQUÍ</a></p>";

                    var result = await Startup.GetServicioEmail().SendAsync(string.Format(mailMensaje, model.Nombre + " " + model.Apellidos, tokenEmail), "Registro Mercampo", model.Email);
                    return RegistroStatus.Exitoso;
                }

            });

        }

        public async Task<LoginStatus> VerificarAdminSesionAsync()
        {

            HttpRequest request = HttpContext.Current.Request;
            return await Task.Run(() =>
            {
                if (getUsuarioAdministradorActual(request) != null) return LoginStatus.Exitoso;
                return LoginStatus.Incorrecto;
            });
        }

        public LoginStatus VerificarPortalSesion()
        {
            var request = HttpContext.Current.Request;
            if (getUsuarioPortalActual(request) != null) return LoginStatus.Exitoso;
            return LoginStatus.Incorrecto;
        }

        public void LogoutAdmin()
        {
            HttpRequest request = HttpContext.Current.Request;
            if (request.Cookies["AdminVendeAgro"] != null)
            {
                var token = request.Cookies["AdminVendeAgro"]["token"];
                if (token != null)
                {
                    borrarCookie(HttpContext.Current.Response, "AdminVendeAgro");
                    using (var _dbContext = new MercampoEntities())
                    {
                        Startup.OpenDatabaseConnection(_dbContext);
                        if (_dbContext.Database.Connection.State != System.Data.ConnectionState.Open)
                        {
                            return;
                        }
                        var usuario = _dbContext.Usuario_Administrador.Where(u => u.tokenSesion == token).FirstOrDefault();
                        if (usuario == null)
                        {
                            _dbContext.Database.Connection.Close();
                            return;
                        }

                        usuario.tokenSesion = "";
                        _dbContext.SaveChanges();
                        _dbContext.Database.Connection.Close();


                    }

                }
            }
        }

        public async Task<LoginStatus> LoginPortalAsync(string email, string password)
        {
            HttpResponse response = HttpContext.Current.Response;
            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    var usuario = _dbContext.Usuarios.Where(u => u.email == email).FirstOrDefault();
                    if (usuario == null)
                    {
                        return LoginStatus.Incorrecto;
                    }

                    if (!usuario.confirmaEmail)
                    {
                        //TO DO: Reenviar token
                        return LoginStatus.ConfirmacionMail;
                    }

                    if (usuario.password.CompareTo(password) != 0)
                    {
                        return LoginStatus.Incorrecto;
                    }

                    usuario.tokenSesion = getToken();
                    setCookie("VendeAgroUser", usuario.tokenSesion, response);
                    _dbContext.SaveChanges();
                    return LoginStatus.Exitoso;
                }

            });

        }

        public void LogoutPortal()
        {
            HttpRequest request = HttpContext.Current.Request;
            if (request.Cookies["VendeAgroUser"] != null)
            {
                var token = request.Cookies["VendeAgroUser"]["token"];
                if (token != null)
                {
                    borrarCookie(HttpContext.Current.Response, "VendeAgroUser");
                    using (var _dbContext = new MercampoEntities())
                    {
                        Startup.OpenDatabaseConnection(_dbContext);
                        if (_dbContext.Database.Connection.State != System.Data.ConnectionState.Open)
                        {
                            return;
                        }
                        var usuario = _dbContext.Usuarios.Where(u => u.tokenSesion == token).FirstOrDefault();
                        if (usuario == null)
                        {
                            _dbContext.Database.Connection.Close();
                            return;
                        }

                        usuario.tokenSesion = "";
                        _dbContext.SaveChanges();
                        _dbContext.Database.Connection.Close();

                    }

                }
            }
        }

        public static string getToken()
        {
            byte[] time = BitConverter.GetBytes(DateTime.UtcNow.ToBinary());
            byte[] key = Guid.NewGuid().ToByteArray();
            return Convert.ToBase64String(time.Concat(key).ToArray());
        }

        public static void setCookie(string name, string value, HttpResponseBase response)
        {
            setCookie(name, value, response.Cookies);
        }

        public static void setCookie(string name, string value, HttpResponse response)
        {
            setCookie(name, value, response.Cookies);
        }

        private static void setCookie(string name, string value, HttpCookieCollection cookies)
        {
            HttpCookie myCookie = new HttpCookie(name);
            myCookie["token"] = value;
            myCookie.Expires = DateTime.Now.AddDays(5d);
            cookies.Add(myCookie);
        }

        public static void borrarCookie(HttpResponse response, string nombre)
        {
            HttpCookie temp = response.Cookies[nombre];
            temp.Expires = DateTime.Now.AddDays(-1D);
            response.Cookies.Add(temp);
        }

        public AdministradorUsuario getUsuarioAdministradorActual(HttpRequestBase request)
        {
            return getUsuarioAdministradorActual(request.Cookies);
        }

        public AdministradorUsuario getUsuarioAdministradorActual(HttpRequest request)
        {
            return getUsuarioAdministradorActual(request.Cookies);
        }

        private AdministradorUsuario getUsuarioAdministradorActual(HttpCookieCollection Cookies)
        {
            if (Cookies["AdminVendeAgro"] != null)
            {
                var token = Cookies["AdminVendeAgro"]["token"];
                if (token != null)
                {
                    using (var _dbContext = new MercampoEntities())
                    {
                        Startup.OpenDatabaseConnection(_dbContext);
                        if (_dbContext.Database.Connection.State != System.Data.ConnectionState.Open)
                        {
                            return null;
                        }

                        var usuario = _dbContext.Usuario_Administrador.Where(u => u.tokenSesion == token).FirstOrDefault();
                        if (usuario == null)
                        {
                            _dbContext.Database.Connection.Close();
                            return null;
                        }

                        if (!usuario.activo)
                        {
                            _dbContext.Database.Connection.Close();
                            return null;
                        }

                        var resultado = new AdministradorUsuario(usuario.id, usuario.email, usuario.nombre);
                        _dbContext.Database.Connection.Close();
                        return resultado;
                    }

                }
            }
            return null;
        }

        public bool AgregarTarjetaAsync(int id, string tokenTarjeta, string sessionId)
        {
            using (var _dbContext = new MercampoEntities())
            {
                Startup.OpenDatabaseConnection(_dbContext);
                if (_dbContext.Database.Connection.State != System.Data.ConnectionState.Open)
                {
                    return false;
                }


                var usuario = _dbContext.Usuarios.Where(u => u.id == id).FirstOrDefault();

                if (usuario == null)
                {
                    _dbContext.Database.Connection.Close();
                    return false;
                }

                Card request = new Card();
                request.TokenId = tokenTarjeta;
                request.DeviceSessionId = sessionId;

                try
                {
                    request = Startup.OpenPayLib.CardService.Create(usuario.idConekta, request);
                }
                catch (OpenpayException e)
                {
                    _dbContext.Database.Connection.Close();
                    return false;
                }

                string last4 = request.CardNumber.Substring(request.CardNumber.Length - 4, 4);

                _dbContext.Usuario_Tarjeta.Add(new Usuario_Tarjeta
                {
                    tipoTarjeta = GetTipoTarjeta(request.Brand),
                    digitosTarjeta = last4,
                    tokenTarjeta = request.Id,
                    idUsuario = id,
                    activo = true
                });
                _dbContext.SaveChanges();
                return true;
            }
        }

        private int GetTipoTarjeta(string tipo)
        {
            switch (tipo.ToLower())
            {
                case "american_express":
                    return (int)TarjetaTipo.Amex;
                case "mastercard":
                    return (int)TarjetaTipo.MasterCard;
                case "visa":
                    return (int)TarjetaTipo.Visa;
                default:
                    return 0;

            }
        }


        public bool RealizarCargoTarjeta(int id, string tarjetaToken, string sessionId, CarritoDeCompra carrito, out string resultadoJson)
        {
            if (carrito == null || carrito.TotalCarrito <= 0.0)
            {
                resultadoJson = ResultadoCargo.IncorrectoAsJson;
                return false;
            }

            HttpRequest request = HttpContext.Current.Request;
            var usuario = getUsuarioPortalActual(request);
            if (usuario.Id != id)
            {
                resultadoJson = ResultadoCargo.IncorrectoAsJson;
                return false;
            }
            using (var _dbContext = new MercampoEntities())
            {
                Startup.OpenDatabaseConnection(_dbContext);
                if (_dbContext.Database.Connection.State != System.Data.ConnectionState.Open)
                {
                    resultadoJson = ResultadoCargo.IncorrectoAsJson;
                    return false;
                }

                try
                {
                    Customer cliente = Startup.OpenPayLib.CustomerService.Get(usuario.IdConekta);
                    if (cliente == null)
                    {
                        resultadoJson = ResultadoCargo.IncorrectoAsJson;
                        return false;
                    }

                    ChargeRequest chargeRequest = new ChargeRequest();
                    chargeRequest.Method = "card";
                    chargeRequest.SourceId = tarjetaToken;
                    chargeRequest.Amount = new decimal(carrito.TotalCarrito);
                    chargeRequest.Currency = "MXN";
                    chargeRequest.Description = "Servicio de anuncios Mercampo.mx";
                    chargeRequest.OrderId = getToken();
                    chargeRequest.DeviceSessionId = sessionId;
                    chargeRequest.SendEmail = true;

                    Charge cargo = Startup.OpenPayLib.ChargeService.Create(usuario.IdConekta, chargeRequest);
                    if (cargo.ErrorMessage != null)
                    {
                        resultadoJson = ResultadoCargo.IncorrectoAsJson;
                        return false;
                    }
                    ResultadoCargo resultado = new ResultadoCargo(ResultadoCargoTarjeta.Aprobado, cargo.OrderId, cargo.Authorization);
                    AgregarAnuncios(carrito, usuario.Id);
                    resultadoJson = Startup.SerializeResultadoCargo(resultado);
                    return true;
                }
                catch (OpenpayException e)
                {
                    ResultadoCargoTarjeta res = ResultadoCargoTarjeta.ErrorInterno;
                    if ((int)ResultadoCargoTarjeta.Rechazado == e.ErrorCode)
                    {
                        res = ResultadoCargoTarjeta.Rechazado;
                    }

                    resultadoJson = Startup.SerializeResultadoCargo(new ResultadoCargo(res, mensaje: e.Message));
                    return false;
                }

            }

        }

        private bool AgregarAnuncios(CarritoDeCompra carrito, int idUsuario)
        {
            using (var _dbContext = new MercampoEntities())
            {
                Startup.OpenDatabaseConnection(_dbContext);
                if (_dbContext.Database.Connection.State != ConnectionState.Open)
                {
                    return false;
                }

                var paquetes = carrito.Paquetes;
                foreach (var paquete in paquetes)
                {
                    if(paquete.EsRenovacion())
                    {
                        Anuncio anuncio = _dbContext.Anuncios.Where(a => a.id == paquete.IdAnuncio).FirstOrDefault();
                        anuncio.fecha_fin = anuncio.fecha_fin.Value.AddMonths(paquete.Meses);
                        anuncio.idPaquete = paquete.Id;
                        anuncio.estado = (int)EstadoAnuncio.Aprobado;
                        anuncio.activo = true;
                        _dbContext.Anuncio_Beneficio.RemoveRange(_dbContext.Anuncio_Beneficio.Where(b => b.idAnuncio == anuncio.id));
                        _dbContext.SaveChanges();

                        var beneficios = paquete.Beneficios;
                        foreach (var beneficio in beneficios)
                        {
                            _dbContext.Anuncio_Beneficio.Add(new Anuncio_Beneficio
                            {
                                idAnuncio = anuncio.id,
                                idBeneficio = beneficio.Id
                            });
                            _dbContext.SaveChanges();
                        }
                    }
                    else
                    {
                        var nuevoAnuncio = _dbContext.Anuncios.Add(new Anuncio
                        {
                            activo = false,
                            idUsuario = idUsuario,
                            estado = (int)EstadoAnuncio.Vacio,
                            idPaquete = paquete.Id
                        });
                        _dbContext.SaveChanges();
                        var beneficios = paquete.Beneficios;
                        foreach (var beneficio in beneficios)
                        {
                            _dbContext.Anuncio_Beneficio.Add(new Anuncio_Beneficio
                            {
                                idAnuncio = nuevoAnuncio.id,
                                idBeneficio = beneficio.Id
                            });
                            _dbContext.SaveChanges();
                        }
                    }
                }
                _dbContext.Database.Connection.Close();
                return true;
            }
        }

        private string CrearClienteConektaId(string nombre, string apellidos, string email, string telefono)
        {
            if (nombre == null || email == null)
            {
                return string.Empty;
            }

            Customer request = new Customer();
            request.ExternalId = Hash(getToken());
            request.Name = nombre;
            request.LastName = apellidos;
            request.Email = email;
            request.PhoneNumber = telefono;
            request.RequiresAccount = false;

            request = Startup.OpenPayLib.CustomerService.Create(request);
            return request.Id;

        }

        public PortalUsuario getUsuarioPortalActual(HttpRequestBase request)
        {
            return getUsuarioPortalActual(request.Cookies);
        }

        public PortalUsuario getUsuarioPortalActual(HttpRequest request)
        {
            return getUsuarioPortalActual(request.Cookies);
        }

        private PortalUsuario getUsuarioPortalActual(HttpCookieCollection Cookies)
        {
            if (Cookies["VendeAgroUser"] != null)
            {
                var token = Cookies["VendeAgroUser"]["token"];
                if (token != null)
                {
                    using (var _dbContext = new MercampoEntities())
                    {
                        Startup.OpenDatabaseConnection(_dbContext);
                        if (_dbContext.Database.Connection.State != System.Data.ConnectionState.Open)
                        {
                            return null;
                        }

                        var usuario = _dbContext.Usuarios.Where(u => u.tokenSesion == token).FirstOrDefault();

                        if (usuario == null)
                        {
                            _dbContext.Database.Connection.Close();
                            return null;
                        }

                        var resultado = new PortalUsuario(usuario.id, usuario.email, usuario.nombre, usuario.apellidos, usuario.telefono, usuario.idConekta);
                        _dbContext.Database.Connection.Close();
                        return resultado;
                    }

                }
            }
            return null;
        }

        public static string Hash(string input)
        {
            var hash = (new SHA1Managed()).ComputeHash(Encoding.UTF8.GetBytes(input));
            return string.Join("", hash.Select(b => b.ToString("x2")).ToArray());
        }

    }

    public enum LoginStatus
    {
        Exitoso,
        ConfirmacionMail,
        Incorrecto
    }

    public enum RegistroStatus
    {
        Exitoso,
        MailOcupado,
        TelefonoOcupado,
        Incorrecto
    }

    public enum OlvidoContrasenaStatus
    {
        MailInexistente,
        Error,
        MailEnviado
    }

    public enum CambiarContrasenaStatus
    {
        TokenInvalido,
        UrlValido,
        Error,
        ContrasenaActualizada
    }

    public enum ConfirmacionMailStatus
    {
        TokenInvalido,
        Error,
        MailConfirmado
    }

    public enum ResultadoCargoTarjeta
    {
        Aprobado = 0,
        Rechazado = 1007,
        ErrorInterno = 2000
    }

    [DataContract]
    public class ResultadoCargo
    {
        [DataMember]
        public ResultadoCargoTarjeta Resultado { get; private set; }

        [DataMember]
        public string NoPedido { get; private set; }

        [DataMember]
        public string Autorizacion { get; private set; }

        public string Mensaje { get; private set; }

        public ResultadoCargo(ResultadoCargoTarjeta resultado, string pedido = "", string autorizacion = "", string mensaje = "")
        {
            Resultado = resultado;
            NoPedido = pedido;
            Autorizacion = autorizacion;
            Mensaje = mensaje;
        }

        public static string IncorrectoAsJson => Startup.SerializeResultadoCargo(new ResultadoCargo(ResultadoCargoTarjeta.ErrorInterno));
    }
}