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
                Email = email,
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



        public ActionResult MisPagos()
        {
            return View();
        }

        public async Task<ActionResult> Perfil(int? id)
        {
            return View();
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
