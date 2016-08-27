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
        private VendeAgroEntities _dbContext;

        public AplicacionUsuariosManager(VendeAgroEntities dbContext)
        {
            _dbContext = dbContext;
        }

        public AdministradorUsuario UsuarioAdministradorActual
        {
            get
            {
                return _usuarioAdministradorActual;
            }
        }

        public async Task<LoginStatus> LoginAdministradorAsync(string email, string password)
        {
            return await Task.Run(() =>
            {
                var usuario = _dbContext.Usuario_Administrador.Where(u => u.email == email).FirstOrDefault();
                if(usuario == null)
                {
                    return LoginStatus.Incorrecto;
                }

                if(!usuario.confirmaEmail)
                {
                    //TO DO: Reenviar token
                    return LoginStatus.ConfirmacionMail;
                }

                if (!usuario.activo || usuario.password.CompareTo(password) != 0)
                {
                    return LoginStatus.Incorrecto;
                }

                _usuarioAdministradorActual = new AdministradorUsuario(usuario.id, usuario.email, usuario.nombre);

                return LoginStatus.Exitoso;
            });
            
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