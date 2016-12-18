using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using VendeAgroWeb.Models;
using VendeAgroWeb.Models.Administrador;
using VendeAgroWeb.Models.Pagina;

namespace VendeAgroWeb.Controllers.Home
{

    public class HomeController : Controller
    {           
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult OfertasDelDia()
        {
            return View();
        }

        public async Task<ActionResult> Anunciate()
        {
            AnunciateViewModel model = new AnunciateViewModel(await ObtenerPaquetes());
            return View(model);
        }

        public ActionResult Contacto()
        {
            return View();
        }

        public async Task<ActionResult> BeneficiosExtra(int? id)
        {
            var paquete = await ObtenerPaquete(id);
            var carrito = Startup.GetCarritoDeCompra();
            var paqueteCarrito = carrito.insertarPaqueteEnCarrito(carrito.Paquetes.Count(), paquete.Nombre, paquete.Meses, paquete.Precio);
            BeneficiosExtraViewModel model = new BeneficiosExtraViewModel(paqueteCarrito, await ObtenerBeneficios(), carrito.TotalCarrito);
            return View(model);
        }

        [HttpPost]
        public async Task<bool> InsertaBeneficiosEnCarrito(int index, string json)
        {
            var beneficios =  new JavaScriptSerializer().Deserialize<List<int>>(json);
            var paquete = Startup.GetCarritoDeCompra().Paquetes.ElementAt(index);
            foreach (var item in beneficios) {
                if (item == -1) {
                    continue;
                }
                var beneficio = await ObtenerBeneficio(item);
                if (beneficio == null) {
                    return false;
                }
                paquete.agregaBeneficioAPaquete(beneficio);
            }
            return true;
        }
    
        public async Task<BeneficioCarrito> ObtenerBeneficio(int id)
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

                    var beneficio = _dbContext.Beneficios.Where(b => b.id == id).FirstOrDefault();

                    _dbContext.Database.Connection.Close();
                    return new BeneficioCarrito(beneficio.id, beneficio.descripcion, beneficio.precio, beneficio.tipo, beneficio.numero);
                }
            });
        }

        public async Task<PaginaPaqueteViewModel> ObtenerPaquete(int? id)
        {
            return await Task.Run(() =>
            {
                if (id == null) {
                    return null;
                }
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }

                    var paquete = _dbContext.Paquetes.Where(p => p.id == id).FirstOrDefault();
                    PaginaPaqueteViewModel paqueteModel = new PaginaPaqueteViewModel(paquete.id, paquete.nombre, paquete.meses, paquete.precio, paquete.descripcion, paquete.porcentajeAhorro);

                    _dbContext.Database.Connection.Close();
                    return paqueteModel;
                }
            });
        }

        public async Task<ICollection<PaginaBeneficioViewModel>> ObtenerBeneficios(){
            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }

                    List<PaginaBeneficioViewModel> lista = new List<PaginaBeneficioViewModel>();
                    var beneficios = _dbContext.Beneficios;

                    foreach (var item in beneficios) {
                        lista.Add(new PaginaBeneficioViewModel(item.id, item.descripcion, item.precio, item.tipo, item.numero));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }
            });
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Contacto(ContactoViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var mensaje = $"<p>Recibio un mensaje a trav&eacute;s de la p&aacute;gina de contacto</p><p>Nombre: {model.Nombre}</p><p>Mail: {model.Email}</p><p>Mensaje: {model.Mensaje}</p>";
            await Startup.GetServicioEmail().SendAsync(mensaje, "Forma de contacto VendeAgro", Startup.GetServicioEmail().MailContacto);
            return View(model);
        }

        public ActionResult EliminaPaqueteDeCarrito(int index) {
            if (Startup.GetCarritoDeCompra().borraPaqueteDeCarrito(index)) {
                return RedirectToAction("CarritoDeCompra");
            }
            return RedirectToAction("CarritoDeCompra");
        }

        public ActionResult EliminaBeneficioDePaquete(int index, int id)
        {
            if (Startup.GetCarritoDeCompra().Paquetes.ElementAt(index).borraBeneficioDePaquete(id))
            {
                return RedirectToAction("CarritoDeCompra");
            }
            return RedirectToAction("CarritoDeCompra");
        }

        public ActionResult CarritoDeCompra()
        {
            return View(Startup.GetCarritoDeCompra());
        }

        public async Task<ActionResult> AnunciosDestacadosPartial()
        {
            PortalAnunciosViewModel model = new PortalAnunciosViewModel(await ObtenerAnunciosDestacados(), "", "", "");
            return PartialView("_AnunciosPartial", model);
        }

        public async Task<ActionResult> AnunciosDestacadosMovilPartial()
        {
            PortalAnunciosViewModel model = new PortalAnunciosViewModel(await ObtenerAnunciosDestacados(), "", "", "");
            return PartialView("_AnunciosMovil", model);
        }

        public async Task<ActionResult> OfertasDelDiaPartial()
        {
            //TODO: Ofertas del dia
            PortalAnunciosViewModel model = new PortalAnunciosViewModel(await ObtenerOfertasDelDia(), "", "", "");
            return PartialView("_OfertasPartial", model);
        }

        public async Task<string> GenerarCargo(string token)
        {
            if(token == null)
            {
                return "token invalido";
            }

            return await Task.Run(() => {
                    return "exitoso";
            });
        }

        public async Task<ICollection<PortalAnuncioViewModel>> ObtenerAnunciosDestacados()
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

                    var anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado).OrderByDescending(a => a.clicks).Take(20);

                    var result = CreaAnuncios(anuncios, _dbContext);
                    _dbContext.Database.Connection.Close();
                    return result;
                }
            });
        }

        public async Task<ICollection<PortalAnuncioViewModel>> ObtenerOfertasDelDia()
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

                    var anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado).OrderByDescending(a => a.clicks).Take(20);

                    var result = CreaAnuncios(anuncios, _dbContext);
                    _dbContext.Database.Connection.Close();
                    return result;
                }
            });
        }

        public async Task<ICollection<PaginaCategoriaViewModel>> ObtenerCategorias()
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

                    var categorias = _dbContext.Categorias.Where(c => c.activo == true);
                    List<PaginaCategoriaViewModel> listaCategorias = new List<PaginaCategoriaViewModel>();

                    foreach (var categoria in categorias) {
                        listaCategorias.Add(new PaginaCategoriaViewModel(categoria.id, categoria.nombre));
                    }

                    _dbContext.Database.Connection.Close();
                    return listaCategorias;
                }
            });
        }

        public async Task<ICollection<PaginaPaqueteViewModel>> ObtenerPaquetes()
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

                    List<PaginaPaqueteViewModel> lista = new List<PaginaPaqueteViewModel>();
                    var paquetes = _dbContext.Paquetes.Where(p => p.activo == true);

                    foreach (var item in paquetes) {
                        lista.Add(new PaginaPaqueteViewModel(item.id, item.nombre, item.meses, item.precio, item.descripcion, item.porcentajeAhorro));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }
            });
        }

        private List<PortalAnuncioViewModel> CreaAnuncios(IQueryable<Anuncio> anuncios, MercampoEntities _dbContext)
        {
            List<PortalAnuncioViewModel> lista = new List<PortalAnuncioViewModel>();
            foreach (var item in anuncios)
            {
                var categoria = _dbContext.Categorias.Where(c => c.id == item.Subcategoria.idCategoria).FirstOrDefault()?.nombre;

                var estado = _dbContext.Estadoes.Where(e => e.id == item.Ciudad.idEstado).FirstOrDefault()?.nombre;
                string fotoPrincipal = item.Fotos_Anuncio.Where(f => f.principal == true).FirstOrDefault()?.ruta;
                lista.Add(new PortalAnuncioViewModel(item.id, item.titulo, item.Usuario.nombre, item.precio, categoria, item.Subcategoria.nombre, estado, item.Ciudad.nombre, item.clicks, fotoPrincipal ?? string.Empty));
            }

            return lista;
        }
    }
}