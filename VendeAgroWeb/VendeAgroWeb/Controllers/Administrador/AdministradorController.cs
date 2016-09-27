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

        public async Task<ActionResult> SubcategoriasDeCategoria(int? id)
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }
            if(id == null)
            {
                return RedirectToAction("Categorias", "Administrador");
            }
            SubcategoriasViewModel model = new SubcategoriasViewModel(await ObtenerSubcategoriasDeCategoria(id));

            return View(model);
        }

        public async Task<ICollection<SubcategoriaViewModel>> ObtenerSubcategoriasDeCategoria(int? id)
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

                    List<SubcategoriaViewModel> lista = new List<SubcategoriaViewModel>();
                    var subcategorias = _dbContext.Subcategorias.Where(s => s.idCategoria == id);
                    var nombreCategoria = _dbContext.Categorias.Where(c => c.id == id).FirstOrDefault()?.nombre;
                    foreach (var item in subcategorias)
                    {
                        lista.Add(new SubcategoriaViewModel(item.id, item.nombre, item.activo, nombreCategoria));
                    }
                    
                    return lista;
                }
            });
        }

        /*public async Task<ActionResult> AnunciosDeCategoria(int? id)
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }
            if (id == null)
            {
                return RedirectToAction("Categorias", "Administrador");
            }
            AnunciosViewModel model = new AnunciosViewModel(await ObtenerAnunciosDeCategoria(id));

            return View(model);
        }

        public async Task<ICollection<AnunciosViewModel>> ObtenerAnunciosDeCategoria(int? id)
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

                    List<AnunciosViewModel> lista = new List<AnunciosViewModel>();
                    var subcategorias = _dbContext.Subcategorias.Where(s => s.idCategoria == id);
                    foreach (var item in subcategorias)
                    {
                        var anuncios = _dbContext.Anuncios.Where(a => a.idSubcategoria == item.id);
                        foreach (var anuncio in anuncios)
                        {
                            lista.Add(new AnuncioViewModel(item.id, item.nombre, item.activo));
                        }
                    }

                    return lista;
                }
            });
        }*/

        [HttpPost]
        [AllowAnonymous]
        public async Task<bool> CambiarEstadoUsuarioAdmin(int? id, int tipo) {

            if (id == null)
            {
                return false;
            }

            await Task.Run(() =>
            {
                using (var _dbContext = new VendeAgroEntities())
                {
                    _dbContext.Database.Connection.Open();
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return false;
                    }

                    var usuario = _dbContext.Usuario_Administrador.Where(u => u.id == id).FirstOrDefault();

                    if (usuario == null)
                    {
                        return false;
                    }

                    if (tipo == 0) {
                        usuario.activo = false;
                    }
                    else {
                        usuario.activo = true;
                    }
                    _dbContext.SaveChanges();
                }
                return true;
            });
            return true;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<bool> CambiarEstadoCategoria(int? id, int tipo)
        {

            if (id == null)
            {
                return false;
            }

            await Task.Run(() =>
            {
                using (var _dbContext = new VendeAgroEntities())
                {
                    _dbContext.Database.Connection.Open();
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return false;
                    }

                    var categoria = _dbContext.Categorias.Where(u => u.id == id).FirstOrDefault();

                    if (categoria == null)
                    {
                        return false;
                    }

                    if (tipo == 0)
                    {
                        categoria.activo = false;
                    }
                    else
                    {
                        categoria.activo = true;
                    }
                    _dbContext.SaveChanges();
                }
                return true;
            });
            return true;
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

        public async Task<ActionResult> Categorias()
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }

            CategoriasViewModel model = new CategoriasViewModel(await ObtenerCategorias());

            return View(model);
        }

        public async Task<ICollection<CategoriaViewModel>> ObtenerCategorias()
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

                    List<CategoriaViewModel> lista = new List<CategoriaViewModel>();
                    var categorias = _dbContext.Categorias;
                    foreach (var item in categorias)
                    {
                        var numSubcategorias = _dbContext.Subcategorias.Where(s => s.idCategoria == item.id).ToList().Count;
                        lista.Add(new CategoriaViewModel(item.id, item.nombre, item.activo, numSubcategorias));
                    }

                    return lista;
                }
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Categorias(CategoriasViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            return View();
        }

        public ActionResult NuevaCategoria()
        {
            return View();
        }

        public async Task<ActionResult> Subcategorias()
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }

            SubcategoriasViewModel model = new SubcategoriasViewModel(await ObtenerSubcategorias());

            return View(model);
        }

        public async Task<ICollection<SubcategoriaViewModel>> ObtenerSubcategorias()
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

                    List<SubcategoriaViewModel> lista = new List<SubcategoriaViewModel>();
                    var subcategorias = _dbContext.Subcategorias;
                    
                    foreach (var item in subcategorias)
                    {
                        var nombreCategoria = _dbContext.Categorias.Where(c => c.id == item.idCategoria).FirstOrDefault()?.nombre;
                        lista.Add(new SubcategoriaViewModel(item.id, item.nombre, item.activo, nombreCategoria));
                    }

                    return lista;
                }
            });
        }

        public ActionResult NuevaSubcategoria()
        {
            return View();
        }

        public async Task<ActionResult> Anuncios()
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }
            return View();
        }

        public async Task<ActionResult> AnunciosActivosPartial()
        {
            AnunciosViewModel model = new AnunciosViewModel(await ObtenerAnunciosAprobados());
            return PartialView("AnunciosActivosPartial", model);
        }

        public async Task<ActionResult> AnunciosVencidosPartial()
        {
            AnunciosViewModel model = new AnunciosViewModel(await ObtenerAnunciosAprobados());
            return PartialView("AnunciosVencidosPartial", model);
        }

        public async Task<ActionResult> AnunciosPendientesPartial()
        {
            AnunciosViewModel model = new AnunciosViewModel(await ObtenerAnunciosAprobados());
            return PartialView("AnunciosPendientesPartial", model);
        }

        public async Task<ICollection<AnuncioViewModel>> ObtenerAnunciosAprobados()
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

                    List<AnuncioViewModel> lista = new List<AnuncioViewModel>();
                    var anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == 1);
                    foreach (var item in anuncios)
                    {
                        var categoria = _dbContext.Categorias.Where(c => c.id == item.Subcategoria.idCategoria).FirstOrDefault()?.nombre;

                        var estado = _dbContext.Estadoes.Where(e => e.id == item.Ciudad.idEstado).FirstOrDefault()?.nombre;

                        lista.Add(new AnuncioViewModel(item.id, item.titulo, item.Usuario.nombre, item.precio,categoria, item.Subcategoria.nombre,estado, item.Ciudad.nombre, item.clicks));
                    }

                    return lista;
                }
            });
        }

        public ActionResult Logout()
        {
            Startup.GetAplicacionUsuariosManager().LogoutAdmin();
            return RedirectToAction("Index", "Administrador");
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
