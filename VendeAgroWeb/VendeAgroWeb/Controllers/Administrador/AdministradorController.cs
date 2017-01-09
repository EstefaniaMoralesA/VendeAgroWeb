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
using System.Web.Script.Serialization;
using Newtonsoft.Json.Linq;
using System.IO;

namespace VendeAgroWeb.Controllers.Administrador
{
    public class AdministradorController : Controller
    {
        private MercampoEntities db = new MercampoEntities();

        public async Task<ActionResult> Index()
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }
            return View();
        }

        public async Task<ActionResult> UsuariosAdministradorPartial()
        {
            UsuariosViewModel model = new UsuariosViewModel(0, await ObtenerUsuariosAdmin(), null);
            return PartialView("UsuariosAdministradorPartial", model);
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
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }

                    List<UsuarioPortalViewModel> lista = new List<UsuarioPortalViewModel>();
                    var usuarios = _dbContext.Usuarios;
                    foreach (var item in usuarios)
                    {
                        var numAnuncios = item.Anuncios.Count;
                        lista.Add(new UsuarioPortalViewModel(item.id, item.nombre, item.apellidos, item.telefono.ToString(), item.email, numAnuncios));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }
            });
        }

        public async Task<ICollection<UsuarioAdministradorViewModel>> ObtenerUsuariosAdmin()
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

                    List<UsuarioAdministradorViewModel> lista = new List<UsuarioAdministradorViewModel>();
                    var usuarios = _dbContext.Usuario_Administrador;
                    foreach (var item in usuarios)
                    {
                        lista.Add(new UsuarioAdministradorViewModel(item.id, item.nombre, item.email, item.activo));
                    }

                    _dbContext.Database.Connection.Close();
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
            if (id == null)
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
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }

                    List<SubcategoriaViewModel> lista = new List<SubcategoriaViewModel>();
                    var subcategorias = _dbContext.Subcategorias.Where(s => s.idCategoria == id);
                    var nombreCategoria = _dbContext.Categorias.Where(c => c.id == id).FirstOrDefault()?.nombre;
                    foreach (var item in subcategorias)
                    {
                        lista.Add(new SubcategoriaViewModel(item.id, item.nombre, item.activo, nombreCategoria, item.Anuncios.Count()));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<bool> CambiarEstadoUsuarioAdmin(int? id, int tipo)
        {

            if (id == null)
            {
                return false;
            }

            await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return false;
                    }

                    var usuario = _dbContext.Usuario_Administrador.Where(u => u.id == id).FirstOrDefault();

                    if (usuario == null)
                    {
                        _dbContext.Database.Connection.Close();
                        return false;
                    }

                    if (tipo == 0)
                    {
                        usuario.activo = false;
                    }
                    else
                    {
                        usuario.activo = true;
                    }
                    _dbContext.SaveChanges();
                    _dbContext.Database.Connection.Close();
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

            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return false;
                    }

                    var categoria = _dbContext.Categorias.Where(u => u.id == id).FirstOrDefault();

                    if (categoria == null)
                    {
                        _dbContext.Database.Connection.Close();
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
                    _dbContext.Database.Connection.Close();

                }
                return true;
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<bool> CambiarEstadoPaquete(int? id, int tipo)
        {

            if (id == null)
            {
                return false;
            }

            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return false;
                    }

                    var paquete = _dbContext.Paquetes.Where(p => p.id == id).FirstOrDefault();

                    if (paquete == null)
                    {
                        _dbContext.Database.Connection.Close();
                        return false;
                    }

                    if (tipo == 0)
                    {
                        paquete.activo = false;
                    }
                    else
                    {
                        paquete.activo = true;
                    }
                    _dbContext.SaveChanges();
                    _dbContext.Database.Connection.Close();

                }
                return true;
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<bool> CambiarEstadoSubcategoria(int? id, int tipo)
        {

            if (id == null)
            {
                return false;
            }

            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return false;
                    }

                    var subcategoria = _dbContext.Subcategorias.Where(u => u.id == id).FirstOrDefault();

                    if (subcategoria == null)
                    {
                        _dbContext.Database.Connection.Close();
                        return false;
                    }

                    if (tipo == 0)
                    {
                        subcategoria.activo = false;
                    }
                    else
                    {
                        subcategoria.activo = true;
                    }
                    _dbContext.SaveChanges();
                    _dbContext.Database.Connection.Close();

                }
                return true;
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

        public async Task<ActionResult> Categorias()
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }

            CategoriasViewModel model = new CategoriasViewModel(await ObtenerCategorias());

            return View(model);
        }

        public async Task<ActionResult> Banners()
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }

            BannersViewModel model = new BannersViewModel(await ObtenerBanners());

            return View(model);
        }

        public async Task<ICollection<BannerViewModel>> ObtenerBanners()
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

                    List<BannerViewModel> lista = new List<BannerViewModel>();
                    var banners = _dbContext.Banners;
                    foreach (var item in banners)
                    {
                        lista.Add(new BannerViewModel(item.Id, item.ruta, item.link, item.activo, item.tipo));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }
            });
        }


        public async Task<ICollection<CategoriaViewModel>> ObtenerCategorias()
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

                    List<CategoriaViewModel> lista = new List<CategoriaViewModel>();
                    var categorias = _dbContext.Categorias;
                    foreach (var item in categorias)
                    {
                        var numSubcategorias = _dbContext.Subcategorias.Where(s => s.idCategoria == item.id).ToList();
                        var numAnuncios = 0;

                        foreach (var subcategoria in numSubcategorias)
                        {
                            numAnuncios += subcategoria.Anuncios.Count();
                        }
                        lista.Add(new CategoriaViewModel(item.id, item.nombre, item.activo, numSubcategorias.Count(), numAnuncios));
                    }

                    _dbContext.Database.Connection.Close();
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

        public async Task<ActionResult> NuevoPaquete()
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }
            return View();
        }

        public async Task<ActionResult> NuevaCategoria()
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> NuevaCategoria(NuevaCategoriaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool estado = true;


            await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        ModelState.AddModelError("", "Hubo un error en la conexión vuelva a intentarlo.");
                        estado = false;
                    }
                    else if (_dbContext.Categorias.Where(c => c.nombre.ToLower() == model.Nombre.ToLower()).FirstOrDefault() != null)
                    {
                        ModelState.AddModelError("", "Error ya existe una categoría con ese nombre.");
                        estado = false;
                    }
                    else
                    {
                        _dbContext.Categorias.Add(new Categoria
                        {
                            nombre = model.Nombre,
                            activo = true
                        });
                        _dbContext.SaveChanges();
                    }
                    _dbContext.Database.Connection.Close();
                }
            });
            if (estado)
            {
                return RedirectToAction("Categorias", "Administrador");
            }
            return View(model);
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
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }

                    List<SubcategoriaViewModel> lista = new List<SubcategoriaViewModel>();
                    var subcategorias = _dbContext.Subcategorias;

                    foreach (var item in subcategorias)
                    {
                        var nombreCategoria = _dbContext.Categorias.Where(c => c.id == item.idCategoria).FirstOrDefault()?.nombre;
                        lista.Add(new SubcategoriaViewModel(item.id, item.nombre, item.activo, nombreCategoria, item.Anuncios.Count()));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }
            });
        }

        public async Task<ActionResult> NuevaSubcategoria()
        {

            var model = new NuevaSubcategoriaViewModel();
            model.Categorias = await ObtenerCategoriasNuevaSubcategoria();
            return View(model);
        }


        public async Task<List<SelectListItem>> ObtenerCategoriasNuevaSubcategoria()
        {
            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return new List<SelectListItem>();
                    }
                    List<SelectListItem> opciones = new List<SelectListItem>();

                    var categorias = _dbContext.Categorias.ToList();

                    foreach (var categoria in categorias)
                    {
                        opciones.Add(
                            new SelectListItem { Value = categoria.id.ToString(), Text = categoria.nombre }
                            );
                    }

                    _dbContext.Database.Connection.Close();
                    return opciones;
                }
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> NuevaSubcategoria(NuevaSubcategoriaViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool estado = true;

            await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        ModelState.AddModelError("", "Hubo un error en la conexión vuelva a intentarlo.");
                        estado = false;
                    }
                    else
                    {
                        if (_dbContext.Subcategorias.Where(s => s.nombre.ToLower() == model.Nombre.ToLower() && s.idCategoria == model.Categoria).FirstOrDefault() != null)
                        {
                            ModelState.AddModelError("", "Error ya existe una subcategoría con ese nombre.");
                            estado = false;
                        }
                        else
                        {
                            _dbContext.Subcategorias.Add(new Subcategoria
                            {
                                nombre = model.Nombre,
                                activo = true,
                                idCategoria = model.Categoria
                            });
                            _dbContext.SaveChanges();
                        }
                        _dbContext.Database.Connection.Close();
                    }

                }
            });
            if (estado)
            {
                return RedirectToAction("Subcategorias", "Administrador");
            }

            model.Categorias = await ObtenerCategoriasNuevaSubcategoria();
            return View(model);
        }

        public async Task<ActionResult> Anuncios()
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }
            return View();
        }

        private async Task<string> ObtenerCategoriaNombre(int? id, string tipo)
        {
            if (tipo != "cat") return string.Empty;

            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }

                    var nombre = _dbContext.Categorias.Where(c => c.id == id).FirstOrDefault()?.nombre;
                    _dbContext.Database.Connection.Close();
                    return nombre;
                }
            });
        }

        private async Task<string> ObtenerSubcategoriaNombre(int? id, string tipo)
        {
            if (tipo != "subcat") return string.Empty;

            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }

                    var nombre = _dbContext.Subcategorias.Where(sc => sc.id == id).FirstOrDefault()?.nombre;
                    _dbContext.Database.Connection.Close();
                    return nombre;
                }
            });
        }

        [HttpPost]
        public async Task<bool> CambiarEstadoBanner(int? id, int? tipo)
        {

            if (id == null || tipo == null)
            {
                return false;
            }

            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return false;
                    }

                    var banner = _dbContext.Banners.Where(b => b.Id == id).FirstOrDefault();

                    if (banner == null)
                    {
                        _dbContext.Database.Connection.Close();
                        return false;
                    }

                    if (tipo == 0)
                    {
                        banner.activo = false;
                    }
                    else
                    {
                        if(banner.ruta == null || banner.link == null)
                        {
                            return false;
                        }
                        banner.activo = true;
                    }
                    _dbContext.SaveChanges();
                    _dbContext.Database.Connection.Close();

                }
                return true;
            });
        }


        public async Task<ActionResult> ModificarBanner(int? id)
        {
            var model = new NuevoBannerViewModel();
            if (id == null)
            {
                return RedirectToAction("Banners", "Administrador");
            }

            bool estado = true;

            await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        estado = false;
                    }
                    else
                    {
                        var banner = _dbContext.Banners.Where(p => p.Id == id).FirstOrDefault();
                        model.Id = banner.Id;
                        model.Ruta = banner.ruta;
                        model.Link = banner.link;
                        model.Tipo = banner.tipo;

                        _dbContext.Database.Connection.Close();
                    }
                }

            });
            if (!estado)
            {
                return RedirectToAction("Paquetes", "Administrador");
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ModificarNombreCategoriaEstatus> ModificarCategoria(int? id, string nombre)
        {
            if (id == null || nombre == null)
            {
                return ModificarNombreCategoriaEstatus.Error;
            }

            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return ModificarNombreCategoriaEstatus.Error;
                    }

                    if (_dbContext.Categorias.Where(c => c.nombre.ToLower() == nombre.ToLower() && c.id != id).FirstOrDefault() != null)
                    {
                        _dbContext.Database.Connection.Close();
                        return ModificarNombreCategoriaEstatus.CategoriaExistente;
                    }

                    var categoria = _dbContext.Categorias.Where(c => c.id == id)?.FirstOrDefault();
                    if (categoria == null)
                    {
                        _dbContext.Database.Connection.Close();
                        return ModificarNombreCategoriaEstatus.Error;
                    }

                    categoria.nombre = nombre;
                    _dbContext.SaveChanges();
                    return ModificarNombreCategoriaEstatus.Exitoso;
                }
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ModificarNombreCategoriaEstatus> ModificarSubcategoria(int? id, string nombre)
        {
            if (id == null || nombre == null)
            {
                return ModificarNombreCategoriaEstatus.Error;
            }

            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return ModificarNombreCategoriaEstatus.Error;
                    }

                    var subcategoria = _dbContext.Subcategorias.Where(c => c.id == id)?.FirstOrDefault();
                    if (subcategoria == null)
                    {
                        _dbContext.Database.Connection.Close();
                        return ModificarNombreCategoriaEstatus.Error;
                    }

                    if (_dbContext.Subcategorias.Where(c => c.nombre.ToLower() == nombre.ToLower() && c.idCategoria == subcategoria.idCategoria).FirstOrDefault() != null)
                    {
                        _dbContext.Database.Connection.Close();
                        return ModificarNombreCategoriaEstatus.CategoriaExistente;
                    }

                    subcategoria.nombre = nombre;
                    _dbContext.SaveChanges();
                    return ModificarNombreCategoriaEstatus.Exitoso;
                }
            });
        }

        private async Task<string> ObtenerUsuarioNombre(int? id, string tipo)
        {
            if (tipo != "usuario") return string.Empty;

            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }

                    var nombre = _dbContext.Usuarios.Where(u => u.id == id).FirstOrDefault()?.nombre;
                    _dbContext.Database.Connection.Close();
                    return nombre;
                }
            });
        }


        public async Task<ActionResult> AnunciosActivosPartial(int? id, string tipo)
        {
            AnunciosViewModel model = new AnunciosViewModel(await ObtenerAnunciosAprobados(id, tipo), await ObtenerCategoriaNombre(id, tipo), await ObtenerSubcategoriaNombre(id, tipo), await ObtenerUsuarioNombre(id, tipo));
            return PartialView("AnunciosPartial", model);
        }

        public async Task<ActionResult> AnunciosVencidosPartial(int? id, string tipo)
        {
            AnunciosViewModel model = new AnunciosViewModel(await ObtenerAnunciosVencidos(id, tipo), "", "", "");
            return PartialView("AnunciosPartial", model);
        }

        public async Task<ActionResult> AnunciosPendientesPartial(int? id, string tipo)
        {
            AnunciosViewModel model = new AnunciosViewModel(await ObtenerAnunciosPorAprobar(id, tipo), "", "", "");
            return PartialView("AnunciosPartial", model);
        }

        public async Task<ActionResult> AnunciosNoAprobadosPartial(int? id, string tipo)
        {
            AnunciosViewModel model = new AnunciosViewModel(await ObtenerAnunciosNoAprobados(id, tipo), "", "", "");
            return PartialView("AnunciosPartial", model);
        }

        private IQueryable<Anuncio> FiltraAnuncios(int? id, string tipo, IQueryable<Anuncio> anuncios, MercampoEntities _dbContext)
        {

            switch (tipo)
            {
                case "cat":
                    var subcategorias = _dbContext.Subcategorias.Where(s => s.idCategoria == id).ToList();
                    List<Anuncio> anunciosFiltrados = new List<Anuncio>();
                    foreach (var subc in subcategorias)
                    {
                        var subcId = subc.id;
                        var anunciosTemp = anuncios.Where(a => a.idSubcategoria == subcId).ToList();
                        anunciosFiltrados = anunciosFiltrados.Concat(anunciosTemp).ToList();
                    }
                    return anunciosFiltrados.AsQueryable();
                case "subcat":
                    return anuncios.Where(a => a.idSubcategoria == id);
                case "usuario":
                    return anuncios.Where(a => a.idUsuario == id);
                default:
                    return null;
            }
        }

        public async Task<ICollection<AnuncioViewModel>> ObtenerAnunciosAprobados(int? id, string tipo)
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

                    var anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado);

                    if (id == null && tipo == null)
                    {
                        var result2 = CreaAnuncios(anuncios, _dbContext);
                        _dbContext.Database.Connection.Close();
                        return result2;
                    }

                    var result = CreaAnuncios(FiltraAnuncios(id, tipo, anuncios, _dbContext), _dbContext);
                    _dbContext.Database.Connection.Close();
                    return result;
                }
            });
        }

        public async Task<ICollection<AnuncioViewModel>> ObtenerAnunciosVencidos(int? id, string tipo)
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

                    var anuncios = _dbContext.Anuncios.Where(a => a.activo == false && a.estado == (int)EstadoAnuncio.Aprobado);

                    if (id == null && tipo == null)
                    {
                        var result2 = CreaAnuncios(anuncios, _dbContext);
                        _dbContext.Database.Connection.Close();
                        return result2;
                    }

                    var result = CreaAnuncios(FiltraAnuncios(id, tipo, anuncios, _dbContext), _dbContext);
                    _dbContext.Database.Connection.Close();
                    return result;
                }
            });
        }

        public async Task<ICollection<AnuncioViewModel>> ObtenerAnunciosPorAprobar(int? id, string tipo)
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

                    var anuncios = _dbContext.Anuncios.Where(a => a.activo == false && a.estado == (int)EstadoAnuncio.PendientePorAprobar);

                    if (id == null && tipo == null)
                    {
                        var result2 = CreaAnuncios(anuncios, _dbContext);
                        _dbContext.Database.Connection.Close();
                        return result2;
                    }

                    var result = CreaAnuncios(FiltraAnuncios(id, tipo, anuncios, _dbContext), _dbContext);
                    _dbContext.Database.Connection.Close();
                    return result;
                }
            });
        }

        public async Task<ICollection<AnuncioViewModel>> ObtenerAnunciosNoAprobados(int? id, string tipo)
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

                    var anuncios = _dbContext.Anuncios.Where(a => a.estado == (int)EstadoAnuncio.NoAprobado);

                    if (id == null && tipo == null)
                    {
                        var result2 = CreaAnuncios(anuncios, _dbContext);
                        _dbContext.Database.Connection.Close();
                        return result2;
                    }

                    var result = CreaAnuncios(FiltraAnuncios(id, tipo, anuncios, _dbContext), _dbContext);
                    _dbContext.Database.Connection.Close();
                    return result;
                }
            });
        }

        private List<AnuncioViewModel> CreaAnuncios(IQueryable<Anuncio> anuncios, MercampoEntities _dbContext)
        {
            List<AnuncioViewModel> lista = new List<AnuncioViewModel>();
            foreach (var item in anuncios)
            {
                var categoria = _dbContext.Categorias.Where(c => c.id == item.Subcategoria.idCategoria).FirstOrDefault()?.nombre;

                var estado = _dbContext.Estadoes.Where(e => e.id == item.Ciudad.idEstado).FirstOrDefault()?.nombre;

                lista.Add(new AnuncioViewModel(item.id, item.titulo, item.Usuario.nombre, item.precio, categoria, item.Subcategoria.nombre, estado, item.Ciudad.nombre, item.clicks));
            }

            return lista;
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<bool> AprobarAnuncio(int? id)
        {
            if (id == null)
            {
                return false;
            }
            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return false;
                    }

                    var anuncio = _dbContext.Anuncios.Where(a => a.id == id).FirstOrDefault();

                    if (anuncio == null)
                    {
                        return false;
                    }

                    var anuncio_paquete = _dbContext.Paquetes.Where(p => p.id == anuncio.idPaquete).FirstOrDefault();

                    if (anuncio == null)
                    {
                        return false;
                    }

                    var duracion = anuncio_paquete?.meses;
                    if (duracion == null)
                    {
                        return false;
                    }

                    anuncio.fecha_inicio = DateTime.Now;
                    anuncio.fecha_fin = DateTime.Now.AddMonths(duracion.Value);
                    anuncio.activo = true;
                    anuncio.estado = (int)EstadoAnuncio.Aprobado;
                    _dbContext.SaveChanges();
                    return true;
                }
            });

        }

        public async Task<ActionResult> AnuncioDetalles(int? id)
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }

            if (id == null)
            {
                return HttpNotFound("Parámetro inválido se espera un id de un anuncio");
            }

            HttpNotFoundResult result = null;
            AnuncioDetallesViewModel model = null;
            await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        result = HttpNotFound("Error en la base de datos");
                    }
                    else
                    {
                        var anuncio = _dbContext.Anuncios.Where(a => a.id == id).FirstOrDefault();
                        if (anuncio == null)
                        {
                            result = HttpNotFound("No se encontro el anuncio con el id solicitado");
                        }
                        else
                        {
                            var anuncioViewModel = new AnuncioViewModel(anuncio.id, anuncio.titulo, anuncio.Usuario.nombre, anuncio.precio,
                                anuncio.Subcategoria.Categoria.nombre, anuncio.Subcategoria.nombre, anuncio.Ciudad.Estado.nombre,
                                anuncio.Ciudad.nombre, anuncio.clicks);
                            List<FotoViewModel> fotos = new List<FotoViewModel>();
                            var paquete = _dbContext.Paquetes.Where(p => p.id == anuncio.idPaquete).FirstOrDefault();

                            AnuncioPaqueteViewModel paqueteViewModel = null;
                            if (paquete != null)
                            {
                                paqueteViewModel = new AnuncioPaqueteViewModel(paquete.nombre, paquete.activo);
                            }

                            var rutaVideo = _dbContext.Videos_Anuncio.Where(v => v.idAnuncio == id).FirstOrDefault()?.ruta;

                            foreach (var foto in anuncio.Fotos_Anuncio)
                            {
                                fotos.Add(new FotoViewModel(foto.principal, foto.ruta));
                            }
                            model = new AnuncioDetallesViewModel(anuncioViewModel, anuncio.estado, anuncio.activo, anuncio.descripcion, fotos, anuncio.fecha_inicio, anuncio.fecha_fin, paqueteViewModel, rutaVideo);
                        }
                    }
                }
            });

            if (result != null)
            {
                return result;
            }

            return View(model);
        }

        public ActionResult Logout()
        {
            Startup.GetAplicacionUsuariosManager().LogoutAdmin();
            return RedirectToAction("Index", "Administrador");
        }

        public async Task<ActionResult> Paquetes()
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }

            PaquetesViewModel model = new PaquetesViewModel(await ObtenerPaquetes());

            return View(model);
        }

        public async Task<ICollection<PaqueteViewModel>> ObtenerPaquetes()
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

                    List<PaqueteViewModel> lista = new List<PaqueteViewModel>();
                    var paquetes = _dbContext.Paquetes;

                    foreach (var item in paquetes)
                    {
                        lista.Add(new PaqueteViewModel(item.id, item.nombre, item.meses, item.precio, item.descripcion, item.paqueteBase, item.fechaModificacion, item.activo, item.porcentajeAhorro));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }
            });
        }
        public async Task<ActionResult> ModificarPaquete(int? id)
        {
            var model = new NuevoPaqueteViewModel();
            if (id == null)
            {
                return RedirectToAction("Paquetes", "Administrador");
            }

            bool estado = true;

            await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        estado = false;
                    }
                    else
                    {
                        var paquete = _dbContext.Paquetes.Where(p => p.id == id).FirstOrDefault();
                        model.Nombre = paquete.nombre;
                        model.Meses = paquete.meses;
                        model.Descripcion = paquete.descripcion;
                        model.Precio = paquete.precio;
                        model.Id = paquete.id;

                        _dbContext.Database.Connection.Close();
                    }
                }

            });
            if (!estado)
            {
                return RedirectToAction("Paquetes", "Administrador");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ModificarBanner(NuevoBannerViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool estado = true;

            await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        ModelState.AddModelError("", "Error en la base de datos, vuelva a intentarlo");
                        estado = false;
                    }
                    else
                    {
                        var banner = _dbContext.Banners.Where(b => b.Id == model.Id).FirstOrDefault();

                        if (banner == null)
                        {
                            ModelState.AddModelError("", "Error banner no encontrado, vuelva a intentarlo");
                            estado = false;
                        }
                        else
                        {
                            banner.ruta = model.Ruta;
                            banner.link = model.Link;
                            banner.activo = true;

                            _dbContext.SaveChanges();
                        }

                        _dbContext.Database.Connection.Close();
                    }
                }

            });
            if (estado)
            {
                return RedirectToAction("Banners", "Administrador");
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> ModificarPaquete(NuevoPaqueteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool estado = true;

            await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        ModelState.AddModelError("", "Error en la base de datos, vuelva a intentarlo");
                        estado = false;
                    }
                    else
                    {
                        var paquete = _dbContext.Paquetes.Where(p => p.id == model.Id).FirstOrDefault();

                        if (paquete == null)
                        {
                            ModelState.AddModelError("", "Error paquete no encontrado, vuelva a intentarlo");
                            estado = false;
                        }
                        else
                        {
                            var paqueteBase = _dbContext.Paquetes.Where(p => p.paqueteBase == true).FirstOrDefault();
                            double ahorro = 0.0;
                            if (model.Meses == 1)
                            {
                                var paquetes = _dbContext.Paquetes;
                                foreach (var item in paquetes)
                                {
                                    if (item.paqueteBase == true)
                                    {
                                        continue;
                                    }
                                    item.porcentajeAhorro = calculaAhorro(model.Precio, item.meses, item.precio);
                                }
                            }
                            else
                            {
                                ahorro = calculaAhorro(paqueteBase, model.Meses, model.Precio);
                            }

                            paquete.descripcion = model.Descripcion;
                            paquete.nombre = model.Nombre;
                            paquete.precio = model.Precio;
                            paquete.porcentajeAhorro = ahorro;
                            paquete.fechaModificacion = DateTime.Now;
                            _dbContext.SaveChanges();
                        }

                        _dbContext.Database.Connection.Close();
                    }
                }

            });
            if (estado)
            {
                return RedirectToAction("Paquetes", "Administrador");
            }
            return View(model);
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<JsonResult> calculaAhorro(int? meses, double? precio)
        {
            double ahorro = 0.0;

            if (!meses.HasValue || !precio.HasValue)
            {
                return Json(new { success = false, error = "Error se necesitan el precio y meses del paquete para calcular el porcentaje de ahorro", porcentaje = ahorro }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                await Task.Run(() =>
                {
                    using (var _dbContext = new MercampoEntities())
                    {
                        Startup.OpenDatabaseConnection(_dbContext);
                        if (_dbContext.Database.Connection.State != ConnectionState.Open)
                        {
                            return Json(new { success = false, error = "Error al conectarse a la base de datos", porcentaje = ahorro }, JsonRequestBehavior.AllowGet);
                        }
                        else
                        {
                            var paqueteBase = _dbContext.Paquetes.Where(p => p.paqueteBase == true).FirstOrDefault();

                            if (paqueteBase == null && meses > 1)
                            {
                                return Json(new { success = false, error = "Error no pueden existir otros paquetes si no existe un paquete base", porcentaje = ahorro }, JsonRequestBehavior.AllowGet);
                            }

                            if (meses == 1)
                            {
                                return Json(new { success = true, error = "", porcentaje = ahorro }, JsonRequestBehavior.AllowGet);
                            }

                            double aPrecio = precio.HasValue ? precio.Value : 0.0;
                            int aMeses = meses.HasValue ? meses.Value : 0;
                            ahorro = Math.Ceiling(Math.Round((100 - ((aPrecio * 100) / (aMeses * paqueteBase.precio))), 2));
                            _dbContext.SaveChanges();
                            _dbContext.Database.Connection.Close();
                        }
                    }
                    return Json(new { success = true, error = "", porcentaje = ahorro }, JsonRequestBehavior.AllowGet);
                });
            }
            return Json(new { success = true, error = "", porcentaje = ahorro }, JsonRequestBehavior.AllowGet);
        }

        public double calculaAhorro(double precioPaqueteBase, int meses, double precio)
        {
            var precioReal = meses * precioPaqueteBase;
            return Math.Ceiling(Math.Round((100 - ((precio * 100) / precioReal)), 2));
        }

        public double calculaAhorro(Paquete paqueteBase, int meses, double precio)
        {
            if (paqueteBase == null)
            {
                if (meses == 1)
                {
                    return 0.0;
                }
                return -1;
            }
            var precioBase = paqueteBase.precio;
            var precioReal = meses * precioBase;
            return Math.Ceiling(Math.Round((100 - ((precio * 100) / precioReal)), 2));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [AllowAnonymous]
        public async Task<ActionResult> NuevoPaquete(NuevoPaqueteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            bool estado = true;

            await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        ModelState.AddModelError("", "Error en la base de datos, vuelva a intentarlo");
                        estado = false;
                    }
                    else
                    {
                        var paquete = _dbContext.Paquetes.Where(p => p.nombre.ToLower() == model.Nombre.ToLower() || p.meses == model.Meses).FirstOrDefault();

                        if (paquete != null)
                        {

                            ModelState.AddModelError("", paquete.meses == model.Meses ?
                                "Error ya existe un paquete con esa duración." :
                                "Error ya existe un paquete con ese nombre.");
                            estado = false;
                        }
                        else
                        {
                            var paqueteBaseFlag = false;
                            var paqueteBase = _dbContext.Paquetes.Where(p => p.paqueteBase == true).FirstOrDefault();
                            if (model.Meses == 1)
                            {
                                paqueteBaseFlag = true;
                            }
                            if (paqueteBase != null && paqueteBaseFlag == true)
                            {
                                ModelState.AddModelError("", "Error ya existe un paquete base (con duración de 1 mes).");
                                estado = false;
                            }
                            else
                            {
                                var ahorro = calculaAhorro(paqueteBase, model.Meses, model.Precio);
                                if (ahorro == -1)
                                {
                                    ModelState.AddModelError("", "Error no pueden existir otros paquetes si no existe un paquete base.");
                                    estado = false;
                                }
                                else
                                {
                                    _dbContext.Paquetes.Add(new Paquete
                                    {
                                        nombre = model.Nombre,
                                        descripcion = model.Descripcion,
                                        precio = model.Precio,
                                        activo = true,
                                        meses = model.Meses,
                                        fechaModificacion = DateTime.Now,
                                        paqueteBase = paqueteBaseFlag,
                                        porcentajeAhorro = ahorro
                                    });
                                    _dbContext.SaveChanges();
                                }
                            }
                        }

                        _dbContext.Database.Connection.Close();
                    }
                }

            });
            if (estado)
            {
                return RedirectToAction("Paquetes", "Administrador");
            }
            return View(model);
        }


        public async Task<ActionResult> Beneficios()
        {
            if (await Startup.GetAplicacionUsuariosManager().VerificarAdminSesionAsync() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Login", "Administrador");
            }

            BeneficiosViewModel model = new BeneficiosViewModel(await ObtenerBeneficios());

            return View(model);
        }

        public async Task<ICollection<BeneficioViewModel>> ObtenerBeneficios()
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

                    List<BeneficioViewModel> lista = new List<BeneficioViewModel>();
                    var beneficios = _dbContext.Beneficios;

                    foreach (var item in beneficios)
                    {
                        lista.Add(new BeneficioViewModel(item.id, item.descripcion, item.precio));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }
            });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<bool> ModificarBeneficio(int? id, double precio)
        {
            if (id == null)
            {
                return false;
            }

            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return false;
                    }

                    var beneficio = _dbContext.Beneficios.Where(b => b.id == id)?.FirstOrDefault();
                    if (beneficio == null)
                    {
                        _dbContext.Database.Connection.Close();
                        return false;
                    }

                    beneficio.precio = precio;
                    _dbContext.SaveChanges();
                    return true;
                }
            });
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

        public async Task<ActionResult> NuevoAnuncio(int? id)
        {
            NuevoAnuncioViewModel model = new NuevoAnuncioViewModel(id.Value, await ObtenerNombreUsuario(id));
            return View(model);
        }

        [HttpPost]
        public string SubirFotos()
        {
            List<string> fotos = new List<string>();
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        // and optionally write the file to disk
                        var fileExtension = Path.GetExtension(fileContent.FileName);
                        var guid = Guid.NewGuid().ToString();
                        var name = AplicacionUsuariosManager.Hash(Guid.NewGuid().ToString());
                        string serverPath = Server.MapPath("~/Uploads/Images");

                        if(!Directory.Exists(serverPath))
                        {
                            Directory.CreateDirectory(serverPath);
                        }

                        var path = Path.Combine(serverPath, $"{name}{fileExtension}");

                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }

                        fotos.Add($"/Uploads/Images/{name}{fileExtension}");
                    }
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JavaScriptSerializer().Serialize(Json(e.Message).Data);
            }

            return new JavaScriptSerializer().Serialize(Json(fotos).Data);
        }

        [HttpPost]
        public string SubirVideo()
        {
            var result = string.Empty;
            try
            {
                foreach (string file in Request.Files)
                {
                    var fileContent = Request.Files[file];
                    if (fileContent != null && fileContent.ContentLength > 0)
                    {
                        // get a stream
                        var stream = fileContent.InputStream;
                        // and optionally write the file to disk
                        var fileExtension = Path.GetExtension(fileContent.FileName);
                        var guid = Guid.NewGuid().ToString();
                        var name = AplicacionUsuariosManager.Hash(Guid.NewGuid().ToString());
                        string serverPath = Server.MapPath("~/Uploads/Videos");

                        if (!Directory.Exists(serverPath))
                        {
                            Directory.CreateDirectory(serverPath);
                        }

                        var path = Path.Combine(serverPath, $"{name}{fileExtension}");

                        using (var fileStream = System.IO.File.Create(path))
                        {
                            stream.CopyTo(fileStream);
                        }

                        result = $"/Uploads/Videos/{name}{fileExtension}";
                    }
                }
            }
            catch (Exception e)
            {
                Response.StatusCode = (int)HttpStatusCode.BadRequest;
                return new JavaScriptSerializer().Serialize(Json(e.Message).Data);
            }

            return new JavaScriptSerializer().Serialize(Json(result).Data);
        }


        [HttpPost]
        public async Task<bool> NuevoAnuncio(string json)
        {
            var anuncio = JObject.Parse(json);
            var titulo = (string)anuncio["jtitulo"];
            var descripcion = (string)anuncio["jdescripcion"];
            var precio = (double)anuncio["jprecio"];
            var idUsuario = (int)anuncio["jidUsuario"];
            var idSubcategoria = (int)anuncio["jidSubcategoria"];
            var idCiudad = (int)anuncio["jidCiudad"];
            var meses = (int)anuncio["jmeses"];
            var fotoDisplay = (string)anuncio["jfotoDisplay"];
            var fotos = (JArray)anuncio["jfotos"];
            var video = (string)anuncio["jvideo"];

            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        ModelState.AddModelError("", "Error en la base de datos, vuelva a intentarlo");
                        return false;
                    }
                    else
                    {
                        var anuncioBD = _dbContext.Anuncios.Where(a => (a.titulo.ToLower() == titulo.ToLower())).FirstOrDefault();

                        if (anuncioBD != null)
                        {

                            ModelState.AddModelError("", "Error ya existe un anuncio con ese título.");
                            return false;
                        }
                        else
                        {
                            var nuevoAnuncio = _dbContext.Anuncios.Add(new Anuncio
                            {
                                titulo = titulo,
                                descripcion = descripcion,
                                precio = precio,
                                activo = true,
                                idUsuario = idUsuario,
                                idSubcategoria = idSubcategoria,
                                idCiudad = idCiudad, 
                                estado = (int)EstadoAnuncio.Aprobado,
                                clicks = 0, 
                                vistas = 0,
                                fecha_inicio = DateTime.Now,
                                fecha_fin =  DateTime.Now.AddMonths(meses),
                            });

                            List<Fotos_Anuncio> fotosAnuncio = new List<Fotos_Anuncio>();

                            _dbContext.Fotos_Anuncio.Add( new Fotos_Anuncio
                            {
                                ruta = fotoDisplay,
                                idAnuncio = nuevoAnuncio.id,
                                principal = true
                            });

                            foreach (var item in fotos)
                            {
                                var url = (string)item;
                                _dbContext.Fotos_Anuncio.Add( new Fotos_Anuncio {
                                    ruta = url,
                                    idAnuncio = nuevoAnuncio.id,
                                    principal = false
                                });
                            }

                            if (!string.IsNullOrEmpty(video))
                            {
                                _dbContext.Videos_Anuncio.Add(new Videos_Anuncio
                                {
                                    ruta = video,
                                    idAnuncio = nuevoAnuncio.id
                                });
                            }

                            _dbContext.SaveChanges();
                        }

                        _dbContext.Database.Connection.Close();
                        return true;
                    }
                }
            });
        }

        public async Task<string> ObtenerNombreUsuario(int? idUsuario) {
            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }
                    var usuario = _dbContext.Usuarios.Where(u => u.id == idUsuario).FirstOrDefault();

                    _dbContext.Database.Connection.Close();
                    return usuario.nombre + " " + usuario.apellidos;
                }

            });
        }

        public async Task<ActionResult> CategoriasAnuncioPartial() {
            CategoriasViewModel model = new CategoriasViewModel(await ObtenerCategoriasAnuncio());
            return PartialView("CategoriasAnuncioPartial", model);
        }

        public async Task<ActionResult> PaisesAnuncioPartial()
        {
            PaisesViewModel model = new PaisesViewModel(await ObtenerPaisesAnuncio());
            return PartialView("PaisesAnuncioPartial", model);
        }

        public async Task<ICollection<PaisViewModel>> ObtenerPaisesAnuncio()
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
                    List<PaisViewModel> lista = new List<PaisViewModel>();
                    var paises = _dbContext.Pais;
                    foreach (var item in paises)
                    {
                        lista.Add(new PaisViewModel(item.id, item.nombre));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }

            });
        }

        [HttpPost]
        public async Task<ActionResult> SubcategoriasAnuncioPartial(int? idCategoria)
        {
            SubcategoriasViewModel model = new SubcategoriasViewModel(await ObtenerSubcategoriasAnuncio(idCategoria));
            return PartialView("SubcategoriasAnuncioPartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> EstadosAnuncioPartial(int? idPais)
        {
            EstadosViewModel model = new EstadosViewModel(await ObtenerEstadosAnuncio(idPais));
            return PartialView("EstadosAnuncioPartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> CiudadesAnuncioPartial(int? idEstado)
        {
            CiudadesViewModel model = new CiudadesViewModel(await ObtenerCiudadesAnuncio(idEstado));
            return PartialView("CiudadesAnuncioPartial", model);
        }

        public async Task<ICollection<CategoriaViewModel>> ObtenerCategoriasAnuncio()
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
                    List<CategoriaViewModel> lista = new List<CategoriaViewModel>();
                    var categorias = _dbContext.Categorias.Where(c=> c.activo == true);
                    foreach (var item in categorias)
                    {
                        lista.Add(new CategoriaViewModel(item.id, item.nombre));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }

            });
        }

        public async Task<ICollection<SubcategoriaViewModel>> ObtenerSubcategoriasAnuncio(int? idCategoria)
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
                    List<SubcategoriaViewModel> lista = new List<SubcategoriaViewModel>();
                    var subcategorias = _dbContext.Subcategorias.Where(s => (s.activo == true) && (s.idCategoria == idCategoria));
                    foreach (var item in subcategorias)
                    {
                        lista.Add(new SubcategoriaViewModel(item.id, item.nombre));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }

            });
        }

        public async Task<ICollection<EstadoViewModel>> ObtenerEstadosAnuncio(int? idPais)
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
                    List<EstadoViewModel> lista = new List<EstadoViewModel>();
                    var estados = _dbContext.Estadoes.Where(e => e.idPais == idPais);
                    foreach (var item in estados)
                    {
                        lista.Add(new EstadoViewModel(item.id, item.nombre));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }

            });
        }

        public async Task<ICollection<CiudadViewModel>> ObtenerCiudadesAnuncio(int? idEstado)
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
                    List<CiudadViewModel> lista = new List<CiudadViewModel>();
                    var ciudades = _dbContext.Ciudads.Where(c => c.idEstado == idEstado);
                    foreach (var item in ciudades)
                    {
                        lista.Add(new CiudadViewModel(item.id, item.nombre));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }

            });
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
