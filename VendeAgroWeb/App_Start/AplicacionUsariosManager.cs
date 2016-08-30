using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using VendeAgroWeb.Models;

namespace VendeAgroWeb
{
    public class AplicacionUsuariosManager
    {
        private AdministradorUsuario _usuarioAdministradorActual;

        public AdministradorUsuario UsuarioAdministradorActual
        {
            get
            {
                return _usuarioAdministradorActual;
            }
        }

        public async Task<LoginStatus> LoginAdministradorAsync(string email, string password)
        {
            HttpResponse response = HttpContext.Current.Response;
            return await Task.Run(() =>
            {
                using (var _dbContext = new VendeAgroEntities())
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

                    _usuarioAdministradorActual = new AdministradorUsuario(usuario.id, usuario.email, usuario.nombre);
                    usuario.tokenSesion = getSessionToken();
                    setCookie("AdminVendeAgro", usuario.tokenSesion, response);
                    _dbContext.SaveChanges();
                    return LoginStatus.Exitoso;
                }
                
            });
            
        }

        public async Task<LoginStatus> VerificarAdminSesionAsync()
        {
            if (_usuarioAdministradorActual != null) return LoginStatus.Exitoso;

            HttpRequest request = HttpContext.Current.Request;
            return await Task.Run(() =>
            {
                if(request.Cookies["AdminVendeAgro"] != null)
                {
                    var token = request.Cookies["AdminVendeAgro"]["token"];
                    if(token != null)
                    {
                        using (var _dbContext = new VendeAgroEntities())
                        {
                            var usuario = _dbContext.Usuario_Administrador.Where(u => u.tokenSesion == token).FirstOrDefault();
                            if (usuario == null)
                            {
                                return LoginStatus.Incorrecto;
                            }

                            if (!usuario.activo)
                            {
                                return LoginStatus.Incorrecto;
                            }

                            _usuarioAdministradorActual = new AdministradorUsuario(usuario.id, usuario.email, usuario.nombre);
                            return LoginStatus.Exitoso;
                        }
                        
                    }
                }
                return LoginStatus.Incorrecto;
            });
        }

        private string getSessionToken()
        {
            StringBuilder sb = new StringBuilder();
            Random random = new Random();

            for (int i = 0; i < 20; i++)
            {
                int opcion = random.Next(0, 9);
                if(opcion < 5)
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
}