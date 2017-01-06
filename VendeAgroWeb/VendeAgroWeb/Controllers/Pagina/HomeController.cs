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
                    continue;
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
                if (id == null)
                {
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

        public async Task<ICollection<PaginaBeneficioViewModel>> ObtenerBeneficios()
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

                    List<PaginaBeneficioViewModel> lista = new List<PaginaBeneficioViewModel>();
                    var beneficios = _dbContext.Beneficios;

                    foreach (var item in beneficios)
                    {
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
            if (token == null)
            {
                return "token invalido";
            }

            return await Task.Run(() =>
            {
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

                    var result = CreaAnuncios(anuncios.ToList(), _dbContext);
                    _dbContext.Database.Connection.Close();
                    return result;
                }
            });
        }

        [HttpPost]
        public async Task<ActionResult> DestacadosFiltradosPartial(int? idCategoria, int? idSubcategoria, int? idPais, int? idEstado, int? idCiudad, double? precioBajo, double? precioAlto) {
            PortalAnunciosViewModel model = new PortalAnunciosViewModel(await ObtenerDestacadosFiltrados(idCategoria, idSubcategoria, idPais, idEstado, idCiudad, precioBajo, precioAlto), "", "", "");
            return PartialView("_AnunciosPartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> DestacadosFiltradosMovilPartial(int? idCategoria, int? idSubcategoria, int? idPais, int? idEstado, int? idCiudad, double? precioBajo, double? precioAlto)
        {
            PortalAnunciosViewModel model = new PortalAnunciosViewModel(await ObtenerDestacadosFiltrados(idCategoria, idSubcategoria, idPais, idEstado, idCiudad, precioBajo, precioAlto), "", "", "");
            return PartialView("_AnunciosMovil", model);
        }

        public ActionResult ResultadosBusqueda(string query)
        {
            return View(new QueryViewModel(query));
        }

        public async Task<ActionResult> ObtenerDestacadosFiltradosTexto(string query) {

            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }

                    IQueryable<Anuncio> anuncios = null;

                    if (query == null)
                    {
                        return null;
                    }

                    anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && (
                    a.Subcategoria.nombre == query || a.Subcategoria.Categoria.nombre == query || a.Ciudad.nombre == query || a.Ciudad.Estado.nombre == query
                    || a.Ciudad.Estado.Pai.nombre == query || a.titulo.Contains(query) == true)).Take(20);

                    _dbContext.Database.Connection.Close();
                    PortalAnunciosBusquedaViewModel model = new PortalAnunciosBusquedaViewModel(CreaAnuncios(anuncios.ToList(), _dbContext), query);
                    return PartialView("_BusquedaPartial", model);
                }
            });

        }


        public async Task<ICollection<PortalAnuncioViewModel>> ObtenerDestacadosFiltrados(int? idCategoria, int? idSubcategoria, int? idPais, int? idEstado, int? idCiudad, double? precioBajo, double? precioAlto) {
            return await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        return null;
                    }

                    IQueryable<Anuncio> anuncios = null;

                    if (idCategoria == -1 && idSubcategoria == -1 && idPais == -1 && idCiudad == -1 && idEstado == -1) {
                        anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.precio < precioAlto && a.precio > precioBajo).OrderByDescending(a => a.clicks).Take(20);
                        _dbContext.Database.Connection.Close();
                        return CreaAnuncios(anuncios.ToList(), _dbContext);
                    }

                    if (idCategoria != -1)
                    {
                        if (idSubcategoria != -1)
                        {
                            if (idPais != -1)
                            {
                                if (idEstado != -1)
                                {
                                    if (idCiudad != -1)
                                    {
                                        anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.idSubcategoria == idSubcategoria && a.idCiudad == idCiudad && a.precio < precioAlto && a.precio > precioBajo).OrderByDescending(a => a.clicks).Take(20);
                                        _dbContext.Database.Connection.Close();
                                        return CreaAnuncios(anuncios.ToList(), _dbContext);
                                    }
                                    else
                                    {
                                        anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.idSubcategoria == idSubcategoria && a.Ciudad.idEstado == idEstado && a.precio < precioAlto && a.precio > precioBajo).OrderByDescending(a => a.clicks).Take(20);
                                        _dbContext.Database.Connection.Close();
                                        return CreaAnuncios(anuncios.ToList(), _dbContext);
                                    }
                                }
                                else
                                {
                                    anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.idSubcategoria == idSubcategoria && a.Ciudad.Estado.idPais == idPais && a.precio < precioAlto && a.precio > precioBajo).OrderByDescending(a => a.clicks).Take(20);
                                    _dbContext.Database.Connection.Close();
                                    return CreaAnuncios(anuncios.ToList(), _dbContext);
                                }
                            }
                            else
                            {
                                anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.idSubcategoria == idSubcategoria && a.precio < precioAlto && a.precio > precioBajo).OrderByDescending(a => a.clicks).Take(20);
                                _dbContext.Database.Connection.Close();
                                return CreaAnuncios(anuncios.ToList(), _dbContext);
                            }
                        }
                        else
                        {
                            anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.Subcategoria.idCategoria == idCategoria && a.precio < precioAlto && a.precio > precioBajo).OrderByDescending(a => a.clicks).Take(20);
                            _dbContext.Database.Connection.Close();
                            return CreaAnuncios(anuncios.ToList(), _dbContext);
                        }
                    }
                    else
                    {
                        if (idPais != -1)
                        {
                            if (idEstado != -1)
                            {
                                if (idCiudad != -1)
                                {
                                    anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.idCiudad == idCiudad && a.precio < precioAlto && a.precio > precioBajo).OrderByDescending(a => a.clicks).Take(20);
                                    _dbContext.Database.Connection.Close();
                                    return CreaAnuncios(anuncios.ToList(), _dbContext);
                                }
                                else
                                {
                                    anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.Ciudad.idEstado == idEstado && a.precio < precioAlto && a.precio > precioBajo).OrderByDescending(a => a.clicks).Take(20);
                                    _dbContext.Database.Connection.Close();
                                    return CreaAnuncios(anuncios.ToList(), _dbContext);
                                }
                            }
                            else
                            {
                                anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.Ciudad.Estado.idPais == idPais ).OrderByDescending(a => a.clicks).Take(20);
                                _dbContext.Database.Connection.Close();
                                return CreaAnuncios(anuncios.ToList(), _dbContext);
                            }
                        }
                    }

                    var result = CreaAnuncios(anuncios.ToList(), _dbContext);
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

                    var anuncios = _dbContext.Anuncios.Where(a => a.activo == true &&
                    a.estado == (int)EstadoAnuncio.Aprobado && a.Anuncio_Beneficio.Where(ab => ab.idAnuncio == a.id &&
                    ab.Beneficio.tipo == (int)BeneficiosExtraTipo.OfertaDelDia).FirstOrDefault() != null);

                    var result = CreaAnuncios(Shuffle(anuncios.ToList()), _dbContext);
                    _dbContext.Database.Connection.Close();
                    return result;
                }
            });
        }

        public async Task<ICollection<PaginaPaisViewModel>> ObtenerPaises()
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
                    List<PaginaPaisViewModel> lista = new List<PaginaPaisViewModel>();
                    lista.Add(new PaginaPaisViewModel("Elige un país"));

                    var paises = _dbContext.Pais;
                    foreach (var item in paises)
                    {
                        lista.Add(new PaginaPaisViewModel(item.id, item.nombre));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }

            });
        }

        [HttpPost]
        public async Task<ActionResult> EstadosPartial(int? idPais)
        {
            PaginaEstadosViewModel model = new PaginaEstadosViewModel(await ObtenerEstados(idPais));
            return PartialView("EstadosPartial", model);
        }

        public async Task<ICollection<PaginaEstadoViewModel>> ObtenerEstados(int? idPais)
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
                    List<PaginaEstadoViewModel> lista = new List<PaginaEstadoViewModel>();
                    lista.Add(new PaginaEstadoViewModel("Elige un estado"));

                    var estados = _dbContext.Estadoes.Where(e => e.idPais == idPais);
                    foreach (var item in estados)
                    {
                        lista.Add(new PaginaEstadoViewModel(item.id, item.nombre));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }

            });
        }

        [HttpPost]
        public async Task<ActionResult> CiudadesPartial(int? idEstado)
        {
            PaginaCiudadesViewModel model = new PaginaCiudadesViewModel(await ObtenerCiudades(idEstado));
            return PartialView("CiudadesPartial", model);
        }

        public async Task<ICollection<PaginaCiudadViewModel>> ObtenerCiudades(int? idEstado)
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
                    List<PaginaCiudadViewModel> lista = new List<PaginaCiudadViewModel>();
                    lista.Add(new PaginaCiudadViewModel("Elige una ciudad"));

                    var ciudades = _dbContext.Ciudads.Where(c => c.idEstado == idEstado);
                    foreach (var item in ciudades)
                    {
                        lista.Add(new PaginaCiudadViewModel(item.id, item.nombre));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }

            });
        }

        public async Task<ActionResult> AnuncioDetalles(int? id, ConsultarDetalles consulta, string query)
        {
            if (id == null)
            {
                return HttpNotFound("Parámetro inválido se espera un id de un anuncio");
            }

            HttpNotFoundResult result = null;
            PortalDetallesAnuncioViewModel model = null;

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
                            anuncio.clicks += 1;
                            _dbContext.SaveChanges();
                            var anuncioViewModel = new PortalAnuncioViewModel(anuncio.id, anuncio.titulo, anuncio.precio, anuncio.Subcategoria.Categoria.nombre, anuncio.Subcategoria.nombre, 
                                anuncio.Ciudad.Estado.nombre, anuncio.Ciudad.nombre, anuncio.Fotos_Anuncio.Where(f => f.principal == true).FirstOrDefault()?.ruta);

                            List<PaginaFotoViewModel> fotos = new List<PaginaFotoViewModel>();

                            var rutaVideo = anuncio.Videos_Anuncio.Where(v => v.idAnuncio == id).FirstOrDefault()?.ruta;

                            string nombre = anuncio.Usuario.nombre + " " + anuncio.Usuario.apellidos;
                            var owner = new PaginaOwnerAnuncioViewModel(anuncio.Usuario.id, nombre, anuncio.Usuario.telefono, anuncio.Usuario.email);

                            foreach (var foto in anuncio.Fotos_Anuncio)
                            {
                                fotos.Add(new PaginaFotoViewModel(foto.principal, foto.ruta));
                            }

                            model = new PortalDetallesAnuncioViewModel(anuncioViewModel, anuncio.descripcion, fotos, rutaVideo, owner, anuncio.clicks, consulta, query);
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

                    listaCategorias.Add(new PaginaCategoriaViewModel("Elige una categoría"));

                    foreach (var categoria in categorias)
                    {
                        listaCategorias.Add(new PaginaCategoriaViewModel(categoria.id, categoria.nombre));
                    }

                    _dbContext.Database.Connection.Close();
                    return listaCategorias;
                }
            });
        }

        public async Task<ICollection<PaginaSubcategoriaViewModel>> ObtenerSubcategorias(int? idCategoria)
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

                    var subcategorias = _dbContext.Subcategorias.Where(s => (s.activo == true) && (s.idCategoria == idCategoria));
                    List<PaginaSubcategoriaViewModel> listaSubcategorias = new List<PaginaSubcategoriaViewModel>();

                    listaSubcategorias.Add(new PaginaSubcategoriaViewModel("Elige una subcategoría"));


                    foreach (var subcategoria in subcategorias)
                    {
                        listaSubcategorias.Add(new PaginaSubcategoriaViewModel(subcategoria.id, subcategoria.nombre));
                    }

                    _dbContext.Database.Connection.Close();
                    return listaSubcategorias;
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

                    foreach (var item in paquetes)
                    {
                        lista.Add(new PaginaPaqueteViewModel(item.id, item.nombre, item.meses, item.precio, item.descripcion, item.porcentajeAhorro));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }
            });
        }

        private List<Anuncio> Shuffle(List<Anuncio> anuncios)
        {
            if (anuncios.Count <= 1) return anuncios;

            Random r = new Random((int)DateTime.Now.Ticks & (0x0000FFFF + new Random().Next(0,anuncios.Count)));
            if (anuncios.Count == 2)
            {
                var random = r.Next(1, 10);

                if (random <= 5) return anuncios;

                var temp = anuncios[1];
                anuncios[1] = anuncios[0];
                anuncios[0] = temp;
                return anuncios;
            }

            for (int i = 0; i < anuncios.Count - 1; i++)
            {
                var random = r.Next(i, anuncios.Count - 1);
                var temp = anuncios[i];
                anuncios[i] = anuncios[random];
                anuncios[random] = temp;
            }
            return anuncios;
        }

        private List<PortalAnuncioViewModel> CreaAnuncios(ICollection<Anuncio> anuncios, MercampoEntities _dbContext)
        {
            List<PortalAnuncioViewModel> lista = new List<PortalAnuncioViewModel>();
            foreach (var item in anuncios)
            {
                lista.Add(new PortalAnuncioViewModel(item.id, item.titulo, item.precio, item.Subcategoria.Categoria.nombre, item.Subcategoria.nombre, item.Ciudad.Estado.nombre, item.Ciudad.nombre, item.Fotos_Anuncio.Where(f => f.principal == true).FirstOrDefault()?.ruta));
            }

            return lista;
        }

        [HttpPost]
        public async Task<ActionResult> SubcategoriasPartial(int? idCategoria)
        {
            PaginaSubcategoriasViewModel model = new PaginaSubcategoriasViewModel(await ObtenerSubcategorias(idCategoria));
            return PartialView("SubcategoriasPartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> CategoriasPartial()
        {
            PaginaCategoriasViewModel model = new PaginaCategoriasViewModel(await ObtenerCategorias());
            return PartialView("CategoriasPartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> PaisesPartial()
        {
            PaginaPaisesViewModel model = new PaginaPaisesViewModel(await ObtenerPaises());
            return PartialView("PaisesPartial", model);
        }

    }
}