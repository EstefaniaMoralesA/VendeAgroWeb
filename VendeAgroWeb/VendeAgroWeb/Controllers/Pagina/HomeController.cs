using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
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
            return View();
        }

        public ActionResult Contacto()
        {
            return View();
        }

        public ActionResult CarritoDeCompra()
        {
            return View();
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
            PortalAnunciosViewModel model = new PortalAnunciosViewModel(await ObtenerAnunciosDestacados(), "", "", "");
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

                    var result2 = CreaAnuncios(anuncios, _dbContext);
                    _dbContext.Database.Connection.Close();
                    return result2;
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

        public Dictionary<Paquete, int> calculaAhorro(IQueryable<Paquete> paquetes) {
            var PaqueteBase = paquetes.Where(p => p.paqueteBase == true).FirstOrDefault();
            var precioBase = PaqueteBase.precio;
            return null;
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

                    Dictionary<Paquete, int> paquetesConAhorro = calculaAhorro(paquetes);
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