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
using VendeAgroWeb.Models.Administrador;

namespace VendeAgroWeb.Controllers.Administrador
{
    public class AdministradorController : Controller
    {
        private VendeAgroEntities db = new VendeAgroEntities();

        public async Task<ActionResult> Index()
        {
            if(await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }
            return View();
        }

        public async Task<ActionResult> UsuariosAdministradorPartial()
        {
            UsuariosViewModel model = new UsuariosViewModel(0, await ObtenerUsuariosAdmin(), null);
            return PartialView("UsuariosAdministradorPartial",model);
        }

        public async Task<ActionResult> UsuariosPortalPartial()
        {
            UsuariosViewModel model = new UsuariosViewModel(1, null, await ObtenerUsuariosPortal());
            return PartialView("UsuariosPortalPartial", model);
        }

        public async Task<ICollection<UsuarioPortalViewModel>> ObtenerUsuariosPortal()
        {
            return await Task.Run(() =>
            {
                using (var _dbContext = new VendeAgroEntities())
                {
                    _dbContext.Database.Connection.Open();
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }

                    List<UsuarioPortalViewModel> lista = new List<UsuarioPortalViewModel>();
                    var usuarios = _dbContext.Usuarios;
                    foreach (var item in usuarios)
                    {
                        lista.Add(new UsuarioPortalViewModel(item.id, item.nombre,item.apellidos, item.telefono.ToString(), item.email));
                    }

                    return lista;
                }
            });
        }

        public async Task<ICollection<UsuarioAdministradorViewModel>> ObtenerUsuariosAdmin()
        {
            return await Task.Run(() => 
            {
                using(var _dbContext = new VendeAgroEntities())
                {
                    _dbContext.Database.Connection.Open();
                    if(_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }

                    List<UsuarioAdministradorViewModel> lista = new List<UsuarioAdministradorViewModel>();
                    var usuarios = _dbContext.Usuario_Administrador;
                    foreach (var item in usuarios)
                    {
                        lista.Add(new UsuarioAdministradorViewModel(item.id, item.nombre, item.email, item.activo));
                    }
                
                    return lista;
                }
            });
        }

        public async Task<ActionResult> Login()
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Exitoso)
            {
                return RedirectToAction("Index", "Administrador");
            }
            return View();
        }

        public ActionResult OlvidoContrasena()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> OlvidoContrasena(OlvidoContrasenaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var resultado = await Startup.GetAplicacionUsuariosManager().OlvidoContrasenaAdminAsync(model.Email);
            ViewData["ResultadoMail"] = resultado.ToString();
            return View();
        }

        public async Task<ActionResult> CambiarContrasena(string token)
        {
            var resultado = await Startup.GetAplicacionUsuariosManager().VerificarTokenCambiarContrasenaAdminAsync(token);
            ViewData["ResultadoUrl"] = resultado.ToString();
            ViewData["Token"] = token;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CambiarContrasena(CambiarContrasenaViewModel model)
        {
            var resultado = await Startup.GetAplicacionUsuariosManager().CambiarContrasenaAdminAsync(AplicacionUsuariosManager.Hash(model.Password), model.Token);
            ViewData["ResultadoUrl"] = resultado.ToString();
            return View();
        }

        public ActionResult Categorias()
        {
            return View();
        }

        public ActionResult NuevaCategoria()
        {
            return View();
        }

        public ActionResult Subcategorias()
        {
            return View();
        }

        public ActionResult NuevaSubcategoria()
        {
            return View();
        }

        public ActionResult Beneficios()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(Models.Administrador.LoginViewModel model, string redirectUrl)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var resultado = await Startup.GetAplicacionUsuariosManager().LoginAdministradorAsync(model.Email, AplicacionUsuariosManager.Hash(model.Password));
            switch (resultado)
            {
                case LoginStatus.Exitoso:
                    return RedirectToAction("Index", "Administrador");
                case LoginStatus.ConfirmacionMail:
                    ModelState.AddModelError("", "Confirmacion de email pendiente.");
                    return View(model);
                case LoginStatus.Incorrecto:
                default:
                    ModelState.AddModelError("", "Usuario o contraseña incorrecto.");
                    return View(model);
            }

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
