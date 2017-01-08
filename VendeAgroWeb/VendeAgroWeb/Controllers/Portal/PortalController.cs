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
            return View();
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
            return View();
        }

        public async Task<ActionResult> AnunciosActivosPartial()
        {
            MisAnunciosViewModel model = new MisAnunciosViewModel(await ObtenerAnunciosActivos());
            return PartialView("AnunciosActivosPartial", model);
        }

        public ActionResult AnunciosVencidosPartial()
        {
            return PartialView("AnunciosVencidosPartial");
        }

        public async Task<ICollection<AnuncioViewModel>> ObtenerAnunciosActivos()
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

                    var id = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request).Id;
                    var anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.idUsuario == id);

                    foreach (var item in anuncios) {
                        var tiempoRestante = (item.fecha_fin.Value - item.fecha_inicio.Value).Days;
                        lista.Add(new AnuncioViewModel(item.id, item.titulo, item.estado, tiempoRestante));
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

        // GET: Usuario_Administrador/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario_Administrador usuario_Administrador = await db.Usuario_Administrador.FindAsync(id);
            if (usuario_Administrador == null)
            {
                return HttpNotFound();
            }
            return View(usuario_Administrador);
        }

        // GET: Usuario_Administrador/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Usuario_Administrador/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Create([Bind(Include = "id,nombre,email,token,confirmaEmail,password,activo")] Usuario_Administrador usuario_Administrador)
        {
            if (ModelState.IsValid)
            {
                db.Usuario_Administrador.Add(usuario_Administrador);
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }

            return View(usuario_Administrador);
        }

        // GET: Usuario_Administrador/Edit/5
        public async Task<ActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario_Administrador usuario_Administrador = await db.Usuario_Administrador.FindAsync(id);
            if (usuario_Administrador == null)
            {
                return HttpNotFound();
            }
            return View(usuario_Administrador);
        }

        // POST: Usuario_Administrador/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Edit([Bind(Include = "id,nombre,email,token,confirmaEmail,password,activo")] Usuario_Administrador usuario_Administrador)
        {
            if (ModelState.IsValid)
            {
                db.Entry(usuario_Administrador).State = EntityState.Modified;
                await db.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(usuario_Administrador);
        }

        // GET: Usuario_Administrador/Delete/5
        public async Task<ActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Usuario_Administrador usuario_Administrador = await db.Usuario_Administrador.FindAsync(id);
            if (usuario_Administrador == null)
            {
                return HttpNotFound();
            }
            return View(usuario_Administrador);
        }

        // POST: Usuario_Administrador/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> DeleteConfirmed(int id)
        {
            Usuario_Administrador usuario_Administrador = await db.Usuario_Administrador.FindAsync(id);
            db.Usuario_Administrador.Remove(usuario_Administrador);
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
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
