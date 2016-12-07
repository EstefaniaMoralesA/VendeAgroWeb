using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using VendeAgroWeb.Models;

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
            return await Task.Run(() =>
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

                    var result = Startup.GetServicioEmail().SendAsync(string.Format(mailMensaje, usuario.nombre, usuario.password), "Recuperar Contraseña VendeAgro", usuario.email);

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
            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    var usuario = _dbContext.Usuarios.Where(u => u.email == model.Email).FirstOrDefault();
                    if (usuario != null)
                    {
                        return RegistroStatus.MailOcupado;
                    }

                    usuario = _dbContext.Usuarios.Where(u => u.telefono == model.Celular).FirstOrDefault();

                    if (usuario != null)
                    {
                        return RegistroStatus.TelefonoOcupado;
                    }

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

                    });

                    _dbContext.SaveChanges();
                    setCookie("VendeAgroUser", tokenSesion, response);

                    var usuarioRegistrado = _dbContext.Usuarios.Where(u => u.email == model.Email).FirstOrDefault();
                    string mailMensaje = "<p>Estimado {0} gracias por registrarte en vendeagro.com</p>" +
                    "<p>Para completar tu registro y poder hacer login da click <a href=\'" + Startup.getBaseUrl() + "/Portal/ConfirmarMail?token=" + "{1}\'>AQUÍ</a></p>";

                    var result = Startup.GetServicioEmail().SendAsync(string.Format(mailMensaje, model.Nombre + " " + model.Apellidos, tokenEmail), "Registro VendeAgro", model.Email);
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
            if (getUsuarioPortalActual(HttpContext.Current.Request) != null) return LoginStatus.Exitoso;
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

        private string getToken()
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < 20; i++)
            {
                int opcion = random.Next(0, 9);
                if (opcion < 5)
                {
                    sb.Append(random.Next(0, 9));
                }
                else
                {
                    sb.Append(Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65))));
                }
            }

            return sb.ToString();
        }

        private void setCookie(string name, string value, HttpResponse response)
        {
            HttpCookie myCookie = new HttpCookie(name);
            myCookie["token"] = value;
            myCookie.Expires = DateTime.Now.AddDays(5d);
            response.Cookies.Add(myCookie);
        }

        private void borrarCookie(HttpResponse response, string nombre)
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

                        var resultado = new PortalUsuario(usuario.id, usuario.email, usuario.nombre, usuario.apellidos, usuario.telefono.ToString());
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
}