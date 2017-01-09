using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Net;
using System.Web;
using System.Web.Mvc;
using VendeAgroWeb.Models;
using VendeAgroWeb.Models.Portal;
using Openpay;

namespace VendeAgroWeb.Controllers.Administrador
{
    public class PortalController : Controller
    {
        private MercampoEntities db = new MercampoEntities();

        // GET: Usuario_Administrador
        public async Task<ActionResult> Index()
        {
            if (Startup.GetAplicacionUsuariosManager().VerificarPortalSesion() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request));
        }

        [HttpPost]
        public async Task<RegistroStatus> Registro(string email, string password, string telefono, string nombre, string apellidos)
        {
            var model = new RegistroViewModel
            {
                Email = email.ToLower(),
                Password = password,
                Celular = telefono,
                Nombre = nombre,
                Apellidos = apellidos
            };

            return await Startup.GetAplicacionUsuariosManager().RegistroUsuarioAsync(model);
        }

        public async Task<ActionResult> ConfrimarMail(string token)
        {
            var result = await Startup.GetAplicacionUsuariosManager().ConfirmarMailPortalAsync(token);
            return View();
        }

        [HttpPost]
        public async Task<LoginStatus> Login(string email, string password)
        {
            return await Startup.GetAplicacionUsuariosManager().LoginPortalAsync(email, AplicacionUsuariosManager.Hash(password));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult LogOut()
        {
            Startup.GetAplicacionUsuariosManager().LogoutPortal();
            return RedirectToAction("Index", "Home");
        }

        public ActionResult MisAnuncios()
        {
            if (Startup.GetAplicacionUsuariosManager().VerificarPortalSesion() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }

        public async Task<ActionResult> AnunciosActivosPartial()
        {
            var id = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request).Id;
            MisAnunciosViewModel model = new MisAnunciosViewModel(await ObtenerAnunciosActivos(id));
            return PartialView("AnunciosActivosPartial", model);
        }

        public ActionResult AnunciosVencidosPartial()
        {
            return PartialView("AnunciosVencidosPartial");
        }

        public async Task<ICollection<AnuncioViewModel>> ObtenerAnunciosActivos(int id)
        {
            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }

                    List<AnuncioViewModel> lista = new List<AnuncioViewModel>();

                    var anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.idUsuario == id);

                    foreach (var item in anuncios) {
                        var tiempoRestante = (item.fecha_fin.Value - DateTime.Now).Days;
                        var duracion = (item.fecha_fin.Value - item.fecha_inicio.Value).Days;
                        var porcentajeDuracion = (int)((tiempoRestante * 100.0) / duracion);
                        if (porcentajeDuracion < 0) porcentajeDuracion = 0;
                        var imagenPrincipal = item.Fotos_Anuncio.Where(foto => foto.principal == true).FirstOrDefault()?.ruta ?? string.Empty;
                        lista.Add(new AnuncioViewModel(item.id, item.titulo, item.estado, porcentajeDuracion, imagenPrincipal));
                    }
                    
                    _dbContext.Database.Connection.Close();
                    return lista;
                }
            });
        }

        public async Task<ActionResult> BorrarTarjeta(int? id)
        {
            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }

                    var usuarioActual = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request);
                    var tarjetaActual = usuarioActual.Tarjetas.Where(t => t.Id == id).FirstOrDefault();

                    try
                    {
                        Startup.OpenPayLib.CardService.Delete(usuarioActual.IdConekta, tarjetaActual.IdConekta);
                    }
                    catch (OpenpayException e)
                    {
                        return null;
                    }

                    var tarjetaBD = _dbContext.Usuario_Tarjeta.Where(t => t.id == id).FirstOrDefault();

                    if (tarjetaBD == null)
                    {
                        return null;
                    }
                    else
                    {
                        tarjetaBD.activo = false;
                        _dbContext.SaveChanges();
                    }


                    _dbContext.Database.Connection.Close();
                    return RedirectToAction("Perfil", "Portal");
                }
            });
        }

        public ActionResult NuevaTarjeta(int? id)
        {
            if (id == null) {
                return RedirectToAction("Index", "Home");
            }
            if (Startup.GetAplicacionUsuariosManager().VerificarPortalSesion() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Index", "Home");
            }
            NuevaTarjetaViewModel model = new NuevaTarjetaViewModel(id.Value);
            return View(model);
        }

        public ActionResult MisPagos()
        {
            return View();
        }

        public async Task<ActionResult> Perfil() 
        {
            var usuario = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request);
            PerfilViewModel model = new PerfilViewModel(usuario);
            return View(model);
        }

        public async Task<bool> CambiarNombre(string nombre, string apellidos) {
            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return false;
                    }

                    var idActual = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request).Id;
                    var usuario = _dbContext.Usuarios.Where(u => u.id == idActual).FirstOrDefault();

                    if (usuario == null)
                    {
                        return false;
                    }
                    else
                    {
                        usuario.nombre = nombre;
                        usuario.apellidos = apellidos;

                        _dbContext.SaveChanges();
                    }

                    _dbContext.Database.Connection.Close();
                    return true;
                }
            });
        }

        public async Task<bool> CambiarTelefono(string telefono)
        {
            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return false;
                    }

                    var idActual = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request).Id;
                    var usuario = _dbContext.Usuarios.Where(u => u.id == idActual).FirstOrDefault();

                    if (usuario == null)
                    {
                        return false;
                    }
                    else
                    {
                        usuario.telefono = telefono;

                        _dbContext.SaveChanges();
                    }

                    _dbContext.Database.Connection.Close();
                    return true;
                }
            });
        }

        public async Task<RegistroStatus> CambiarEmail(string email)
        {
            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return RegistroStatus.Incorrecto;
                    }

                    var usuario = _dbContext.Usuarios.Where(u => u.email == email).FirstOrDefault();

                    if (usuario != null)
                    {
                        return RegistroStatus.MailOcupado;
                    }

                    var idActual = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request).Id;

                    usuario = _dbContext.Usuarios.Where(u => u.id == idActual).FirstOrDefault();

                    if (usuario == null)
                    {
                        return RegistroStatus.Incorrecto;
                    }
                    else
                    {
                        usuario.email = email;

                        _dbContext.SaveChanges();
                    }

                    _dbContext.Database.Connection.Close();
                    return RegistroStatus.Exitoso;
                }
            });
        }

        [HttpPost]
        public async Task<bool> CambiarPassword(string contrasena)
        {
            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return false;
                    }

                    var idActual = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request).Id;
                    var usuario = _dbContext.Usuarios.Where(u => u.id == idActual).FirstOrDefault();

                    if (usuario == null)
                    {
                        return false;
                    }
                    else
                    {
                        usuario.password = AplicacionUsuariosManager.Hash(contrasena);
                        _dbContext.SaveChanges();
                    }

                    _dbContext.Database.Connection.Close();
                    return true;
                }
            });
        }

        [HttpPost]
        public async Task<bool> AgregarTarjeta(int? id, string tokenTarjeta, string sessionId)
        {
            return await Startup.GetAplicacionUsuariosManager().AgregarTarjetaAsync(id.Value, tokenTarjeta, sessionId);
        }

        public ActionResult CrearAnuncio()
        {
            return View();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
