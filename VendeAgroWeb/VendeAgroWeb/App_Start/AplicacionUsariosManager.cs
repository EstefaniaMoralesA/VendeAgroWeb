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

                    string mailMensaje = "<p>Estimado/a {0},</p>" +
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

                    string mailMensaje = "<p>Estimado/a {0},</p>" +
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
                    string mailMensaje = "<p>Estimado/a {0} gracias por registrarte en mercampo.mx</p>" +
                    "<p>Para completar tu registro y poder hacer login da click <a href=\'" + Startup.getBaseUrl() + "/Portal/ConfirmarMail?token=" + "{1}\'>AQUÍ</a></p>";

                    var result = await Startup.GetServicioEmail().SendAsync(string.Format(mailMensaje, model.Nombre + " " + model.Apellidos, tokenEmail), "Registro Mercampo", model.Email);
                    return RegistroStatus.Exitoso;
                }

            });

        }

        public async Task<AproboAnuncioStatus> AproboAnuncioAdminAsync(string email, int idAnuncio, string tituloAnuncio)
        {
            return await Task.Run(async () =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);

                    if (_dbContext.Database.Connection.State != System.Data.ConnectionState.Open)
                    {
                        return AproboAnuncioStatus.Error;
                    }

                    var usuario = _dbContext.Usuarios.Where(u => u.email == email).FirstOrDefault();
                    if (usuario == null)
                    {
                        _dbContext.Database.Connection.Close();
                        return AproboAnuncioStatus.MailInexistente;
                    }

                    string mailMensaje = "<p>Estimado/a {0},</p>" +
                    "<p>Tu anuncio " + tituloAnuncio + " ha sido aprobado y publicado. Para consultarlo, da click <a href=\'" + Startup.getBaseUrl() + "/Home/AnuncioDetalles?id=" + idAnuncio + "{1}\'>AQUÍ</a></p>";

                    var result = await Startup.GetServicioEmail().SendAsync(string.Format(mailMensaje, usuario.nombre, usuario.password), "Tu Anuncio ha sido Aprobado en Mercampo", usuario.email);

                    _dbContext.Database.Connection.Close();
                    return AproboAnuncioStatus.MailEnviado;

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

        public string AgregarTarjetaAsync(int id, string tokenTarjeta, string sessionId)
        {
            using (var _dbContext = new MercampoEntities())
            {
                Startup.OpenDatabaseConnection(_dbContext);
                if (_dbContext.Database.Connection.State != ConnectionState.Open)
                {
                    return new ResultadoAgregarTarjeta(false, "Error en el servidor, vuelva a intentarlo de nuevo en unos minutos").AsJson();
                }


                var usuario = _dbContext.Usuarios.Where(u => u.id == id).FirstOrDefault();

                if (usuario == null)
                {
                    _dbContext.Database.Connection.Close();
                    return new ResultadoAgregarTarjeta(false, "Error, el id del usuario al que se le quiere agregar la tarjeta, no existe").AsJson();
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
                    return new ResultadoAgregarTarjeta(false, TarjetaResultadoHelpers.ObtenerMensajeError((OpenPayErrorCodes)e.ErrorCode)).AsJson();
                    
                }

                string last4 = request.CardNumber.Substring(request.CardNumber.Length - 4, 4);

                _dbContext.Usuario_Tarjeta.Add(new Usuario_Tarjeta
                {
                    tipoTarjeta = TarjetaResultadoHelpers.GetTipoTarjeta(request.Brand),
                    digitosTarjeta = last4,
                    tokenTarjeta = request.Id,
                    idUsuario = id,
                    activo = true
                });
                _dbContext.SaveChanges();
                return new ResultadoAgregarTarjeta(true, "La tarjeta se agregó correctamente.").AsJson();
            }
        }


        public bool RealizarCargoTarjeta(int id, string tarjetaToken, string sessionId, CarritoDeCompra carrito, out string resultadoJson)
        {
            if (carrito == null || carrito.TotalCarrito <= 0.0)
            {
                resultadoJson = new ResultadoCargo(false, ResultadoCargoTarjeta.ErrorInterno, mensaje: "El carrito de compras esta vacio").AsJson();
                return false;
            }

            HttpRequest request = HttpContext.Current.Request;
            var usuario = getUsuarioPortalActual(request);
            if (usuario.Id != id)
            {
                resultadoJson = new ResultadoCargo(false, ResultadoCargoTarjeta.ErrorInterno, mensaje: "Error favor de hacer login").AsJson();
                return false;
            }
            using (var _dbContext = new MercampoEntities())
            {
                Startup.OpenDatabaseConnection(_dbContext);
                if (_dbContext.Database.Connection.State != ConnectionState.Open)
                {
                    resultadoJson = new ResultadoCargo(false, ResultadoCargoTarjeta.ErrorInterno, mensaje: "Error en el servidor, vuelva a intentarlo de nuevo en unos minutos").AsJson();
                    return false;
                }

                try
                {
                    Customer cliente = Startup.OpenPayLib.CustomerService.Get(usuario.IdConekta);
                    if (cliente == null)
                    {
                        resultadoJson = new ResultadoCargo(false, ResultadoCargoTarjeta.ErrorInterno, mensaje: "Error en el servidor, vuelva a intentarlo de nuevo en unos minutos").AsJson();
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
                    ResultadoCargo resultado = new ResultadoCargo(true, ResultadoCargoTarjeta.Aprobado, cargo.OrderId, cargo.Authorization, "El cargo ha sido exitoso", (double)cargo.Amount);
                    Usuario_Tarjeta tarjeta = _dbContext.Usuario_Tarjeta.Where(t => t.tokenTarjeta == tarjetaToken).FirstOrDefault();
                    AgregarAnuncios(carrito, usuario.Id);
                    AgregarPago(usuario.Id, tarjeta, (double)cargo.Amount, cargo.Authorization, carrito);
                    resultadoJson = resultado.AsJson();
                    return true;
                }
                catch (OpenpayException e)
                {
                    ResultadoCargoTarjeta res = ResultadoCargoTarjeta.ErrorInterno;
                    if ((int)ResultadoCargoTarjeta.Rechazado == e.ErrorCode)
                    {
                        res = ResultadoCargoTarjeta.Rechazado;
                    }

                    resultadoJson = new ResultadoCargo(false, res, mensaje: TarjetaResultadoHelpers.ObtenerMensajeError((OpenPayErrorCodes)e.ErrorCode)).AsJson();
                    return false;
                }

            }

        }

        private bool AgregarPago(int idUsuario, Usuario_Tarjeta tarjeta, double total, string referencia, CarritoDeCompra carrito)
        {
            using (var _dbContext = new MercampoEntities())
            {
                Startup.OpenDatabaseConnection(_dbContext);
                if (_dbContext.Database.Connection.State != ConnectionState.Open)
                {
                    return false;
                }

                Pago newPago = _dbContext.Pagoes.Add(new Pago
                {
                    idUsuario = idUsuario, 
                    tipoTarjeta = tarjeta.tipoTarjeta,
                    digitosTarjeta = int.Parse(tarjeta.digitosTarjeta),
                    total = total, 
                    fecha = DateTime.Now,
                    Referencia = referencia
                });
                _dbContext.SaveChanges();

                foreach (var paquete in carrito.Paquetes)
                {
                    if (paquete.Beneficios.Count() < 1)
                    {
                        Pago_Concepto pagoConcepto = _dbContext.Pago_Concepto.Add(new Pago_Concepto
                        {
                            idPago = newPago.id,
                            idPaquete = paquete.Id,
                            tipo = paquete.EsRenovacion()
                        });
                        continue;
                    }
                    
                    foreach (var beneficio in paquete.Beneficios)
                    {
                        Pago_Concepto pagoConcepto = _dbContext.Pago_Concepto.Add(new Pago_Concepto
                        {
                            idPago = newPago.id, 
                            idPaquete = paquete.Id, 
                            idBeneficio = beneficio.Id, 
                            tipo = paquete.EsRenovacion()
                        });
                    }
                }

                _dbContext.SaveChanges();
                _dbContext.Database.Connection.Close();
                return true;
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

    public enum AproboAnuncioStatus
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
    public class ResultadoAgregarTarjeta
    {
        [DataMember]
        public bool Exitoso { get; set; }

        [DataMember]
        public string Mensaje { get; set; }

        public ResultadoAgregarTarjeta(bool exitoso, string mensaje)
        {
            Exitoso = exitoso;
            Mensaje = mensaje;
        }

        public string AsJson()
        {
            return Startup.SerializeResultadoAgregarTarjeta(this);
        }
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

        [DataMember]
        public string Mensaje { get; private set; }

        [DataMember]
        public bool Exitoso { get; set; }

        [DataMember]
        public double Monto { get; set; }

        public ResultadoCargo(bool exitoso, ResultadoCargoTarjeta resultado, string pedido = "", string autorizacion = "", string mensaje = "", double monto = 0)
        {
            Exitoso = exitoso;
            Resultado = resultado;
            NoPedido = pedido;
            Autorizacion = autorizacion;
            Mensaje = mensaje;
            Monto = monto;
        }

        public string AsJson()
        {
            return Startup.SerializeResultadoCargo(this);
        }
    }

    public enum OpenPayErrorCodes
    {
        TarjetaRegistradaCliente = 2002,
        ClienteYaExiste = 2003,
        NumeroDeTarjetaInvalido = 2004,
        FechaExpirada = 2005,
        CodigoSeguridadInvalido = 2006,
        TarjetaSoloPrueba = 2007,
        TarjetaNoValidaPuntos = 2008,
        TarjetaDeclinada = 3001,
        TarjetaExpirada = 3002,
        TarjetaSinFondos = 3003,
        TarjetaRobada = 3004,
        TarjetaFraude = 3005,
        OperacionNoPermitida = 3006,
        TarjetaNoSoportadaEnLinea = 3008,
        TarjetaReportadaPerdida = 3009,
        TarjetaRestringida = 3010,
        TarjetaRetenida = 3011,
        NecesitaAutorizacion = 3012
    }

    public static class TarjetaResultadoHelpers
    {
        public static string ObtenerMensajeError(OpenPayErrorCodes error)
        {
            switch (error)
            {
                case OpenPayErrorCodes.TarjetaRegistradaCliente:
                    return "La tarjeta ya esta registrada con el cliente actual.";
                case OpenPayErrorCodes.ClienteYaExiste:
                    return "El cliente ya existe.";
                case OpenPayErrorCodes.NumeroDeTarjetaInvalido:
                    return "El número de tarjeta es invalido.";
                case OpenPayErrorCodes.FechaExpirada:
                    return "La fecha de expiración debe de ser mayor a la fecha actual.";
                case OpenPayErrorCodes.CodigoSeguridadInvalido:
                    return "El código de seguridad ingresado es invalido.";
                case OpenPayErrorCodes.TarjetaSoloPrueba:
                    return "La tarjeta ingresada es solo válida en modo de prueba.";
                case OpenPayErrorCodes.TarjetaNoValidaPuntos:
                    return "La tarjeta no es válida para utilizarse con puntos.";
                case OpenPayErrorCodes.TarjetaDeclinada:
                    return "La tarjeta fue declinada.";
                case OpenPayErrorCodes.TarjetaExpirada:
                    return "La tarjeta ha expirado.";
                case OpenPayErrorCodes.TarjetaSinFondos:
                    return "La tarjeta no tiene fondos suficientes.";
                case OpenPayErrorCodes.TarjetaRobada:
                    return "La tarjeta ha sido identificada como robada.";
                case OpenPayErrorCodes.TarjetaFraude:
                    return "La tarjeta ha sido identificada como fraudalenta.";
                case OpenPayErrorCodes.OperacionNoPermitida:
                    return "La operacion no esta permitida para este cliente o esta transacción";
                case OpenPayErrorCodes.TarjetaNoSoportadaEnLinea:
                    return "La tarjeta no soporta transacciones en línea";
                case OpenPayErrorCodes.TarjetaReportadaPerdida:
                    return "La tarjeta ha sido reportada como perdida.";
                case OpenPayErrorCodes.TarjetaRestringida:
                    return "El banco ha restringido la tarjeta";
                case OpenPayErrorCodes.TarjetaRetenida:
                    return "El banco ha solicitado que la tarjeta sea retenida. Favor de contactar al banco.";
                case OpenPayErrorCodes.NecesitaAutorizacion:
                    return "Se requere solicitar al banco autorización para realizar este pago.";
                default:
                    return "Error en el servidor, vuelva a intentarlo";
            }
        }

        public static int GetTipoTarjeta(string tipo)
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
    }
}