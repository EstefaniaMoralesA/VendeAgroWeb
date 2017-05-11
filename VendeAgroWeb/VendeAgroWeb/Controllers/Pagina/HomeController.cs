using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using VendeAgroWeb.Models;
using VendeAgroWeb.Models.Pagina;

namespace VendeAgroWeb.Controllers.Home
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            using (MercampoEntities _dbContext = new MercampoEntities())
            {
                Startup.OpenDatabaseConnection(_dbContext);
                if (_dbContext.Database.Connection.State != ConnectionState.Open)
                {
                    return View();
                }
                var ultimo = _dbContext.Accesos.Where(a => a.activo == true).FirstOrDefault();
                var necesitaActualizarAnuncios = false;
                if (ultimo == null)
                {
                    necesitaActualizarAnuncios = true;
                }
                else
                {
                    var result = ultimo.Fecha.Subtract(DateTime.UtcNow);

                    if (result.Days != 0)
                    {
                        necesitaActualizarAnuncios = true;
                        ultimo.activo = false;
                        _dbContext.SaveChanges();
                    }
                }

                if (necesitaActualizarAnuncios)
                {
                    _dbContext.Accesos.Add(new Acceso
                    {
                        Fecha = DateTime.UtcNow,
                        activo = true
                    });
                    _dbContext.SaveChanges();

                    var anunciosQueDebeDesactivar = _dbContext.Anuncios.Where(a => a.fecha_fin != null && a.activo == true && (int)EstadoAnuncio.Aprobado == a.estado);
                    foreach (var anuncio in anunciosQueDebeDesactivar)
                    {
                        if (anuncio.fecha_fin.Value.Subtract(DateTime.UtcNow).Days < 0)
                        {
                            var a = _dbContext.Anuncios.Where(an => an.id == anuncio.id).FirstOrDefault();
                            a.activo = false;
                            a.estado = (int)EstadoAnuncio.Vencido;
                        }
                    }
                    _dbContext.SaveChanges();
                }
                return View();
            }
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

        public ActionResult BeneficiosRedirect(int? id)
        {
            return RedirectToAction("BeneficiosExtra", new { idPaquete = id, anuncio = -1 });
        }

        public async Task<ActionResult> BeneficiosExtra(int? idPaquete, int? anuncio)
        {
            if (!idPaquete.HasValue) return RedirectToAction("CarritoDeCompra");
            if (!anuncio.HasValue) return RedirectToAction("CarritoDeCompra");
            var paquete = await ObtenerPaquete(idPaquete);
            if (paquete == null)
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Id de paquete invalido");
            string nombreAnuncio = string.Empty;
            var carrito = Startup.GetCarritoDeCompra(Request.Cookies);
            if (anuncio.Value != -1)
            {
                var usuario = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request);
                if (usuario == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Para renovar un anuncio debe de hacer login");
                }

                string value;
                if (!ValidaAnuncio(anuncio.Value, usuario.Id, out value))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "El id del anuncio a renovar es invalido");
                }
                nombreAnuncio = value;
                PaqueteCarrito outPaqueteCarrito;
                if (carrito.ActualizaRenovacionSiExiste(anuncio.Value, nombreAnuncio, paquete, out outPaqueteCarrito))
                {
                    Startup.UpdateCarritoCookie(carrito, Response);
                    return View(new BeneficiosExtraViewModel(outPaqueteCarrito, await ObtenerBeneficios(), carrito.TotalCarrito));
                }
            }

            var paqueteCarrito = carrito.insertarPaqueteEnCarrito(paquete.Id, paquete.Nombre, paquete.Meses, paquete.Precio, anuncio.Value, nombreAnuncio);
            Startup.UpdateCarritoCookie(carrito, Response);
            BeneficiosExtraViewModel model = new BeneficiosExtraViewModel(paqueteCarrito, await ObtenerBeneficios(), carrito.TotalCarrito);
            return View(model);
        }

        private bool ValidaAnuncio(int idAnuncio, int idUsuario, out string nombreAnuncio)
        {
            nombreAnuncio = string.Empty;
            using (MercampoEntities _dbContext = new MercampoEntities())
            {
                Startup.OpenDatabaseConnection(_dbContext);
                if (_dbContext.Database.Connection.State != ConnectionState.Open)
                {
                    return false;
                }

                Anuncio anuncio = _dbContext.Anuncios.Where(a => a.id == idAnuncio).FirstOrDefault();

                if (anuncio == null || anuncio.idUsuario != idUsuario)
                {
                    return false;
                }
                nombreAnuncio = anuncio.titulo;
            }
            return true;
        }

        [HttpPost]
        public async Task<bool> InsertaBeneficiosEnCarrito(int index, string json)
        {
            var beneficios = new JavaScriptSerializer().Deserialize<List<int>>(json);
            var carrito = Startup.GetCarritoDeCompra(Request.Cookies);
            var paquete = carrito.Paquetes.ElementAt(index);
            foreach (var item in beneficios)
            {
                if (item == -1)
                {
                    continue;
                }
                var beneficio = await ObtenerBeneficio(item);
                if (beneficio == null)
                {
                    continue;
                }
                paquete.agregaBeneficioAPaquete(beneficio);
            }
            carrito.Paquetes[index] = paquete;
            Startup.UpdateCarritoCookie(carrito, Response);
            return true;
        }

        public ActionResult CarritoLogin(int? redirect)
        {
            return View(Startup.GetCarritoDeCompra(Request.Cookies));
        }

        public ActionResult PagoRecibido(int? resultado, string numero, string autorizacion, double? monto)
        {
            if (resultado == null || numero == null || autorizacion == null || !(Enum.IsDefined(typeof(ResultadoCargoTarjeta), resultado.Value)) || Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request) == null)
            {
                return View(new ResultadoCargo(false, ResultadoCargoTarjeta.ErrorInterno));
            }
            return View(new ResultadoCargo(true, (ResultadoCargoTarjeta)resultado.Value, numero, autorizacion, mensaje: "", monto: monto.Value));
        }

        public ActionResult PagoCarritoTarjetas()
        {
            var usuario = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request);
            if (usuario == null)
            {
                return RedirectToAction("CarritoLogin", "Home", new { redirect = 1 });
            }
            var carrito = Startup.GetCarritoDeCompra(Request.Cookies);
            if (carrito.Paquetes.Count <= 0)
            {
                return RedirectToAction("CarritoDeCompra", "Home");
            }
            PagoCarritoTarjetasViewModel model = new PagoCarritoTarjetasViewModel(usuario, carrito.TotalCarrito);
            return View(model);
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
            await Startup.GetServicioEmail().SendAsync(mensaje, "Forma de contacto Mercampo", Startup.GetServicioEmail().MailContacto);
            return View(new ContactoViewModel());
        }

        public ActionResult EliminaPaqueteDeCarrito(int index)
        {
            var carrito = Startup.GetCarritoDeCompra(Request.Cookies);
            carrito.borraPaqueteDeCarrito(index);
            Startup.UpdateCarritoCookie(carrito, Response);
            return RedirectToAction("CarritoDeCompra");
        }

        public ActionResult EliminaBeneficioDePaquete(int index, int id)
        {
            var carrito = Startup.GetCarritoDeCompra(Request.Cookies);
            carrito.Paquetes.ElementAt(index).borraBeneficioDePaquete(id);
            Startup.UpdateCarritoCookie(carrito, Response);
            return RedirectToAction("CarritoDeCompra");
        }

        public ActionResult CarritoDeCompra()
        {
            var carrito = Startup.GetCarritoDeCompra(Request.Cookies);
            return View(carrito);
        }

        public async Task<ActionResult> AnunciosDestacadosPartial(int? index)
        {
            if (!index.HasValue)
            {
                index = 0;
            }
            using (var _dbContext = new MercampoEntities())
            {

                var anuncios = await ObtenerAnunciosDestacados(_dbContext);
                PortalAnunciosViewModel model = new PortalAnunciosViewModel(ObtenerSiguientesAnuncios(index.Value, anuncios), "", "", "", anuncios.Count, index.Value);
                return PartialView("_AnunciosPartial", model);
            }

        }

        public async Task<ActionResult> AnunciosDestacadosMovilPartial(int? index)
        {
            if (!index.HasValue)
            {
                index = 0;
            }
            using (var _dbContext = new MercampoEntities())
            {
                var anuncios = await ObtenerAnunciosDestacados(_dbContext);
                PortalAnunciosViewModel model = new PortalAnunciosViewModel(ObtenerSiguientesAnuncios(index.Value, anuncios), "", "", "", anuncios.Count, index.Value);
                return PartialView("_AnunciosMovil", model);
            }
        }

        public async Task<ActionResult> OfertasDelDiaPartial(int? index)
        {
            if (!index.HasValue)
            {
                index = 0;
            }
            using (var _dbContext = new MercampoEntities())
            {
                var anuncios = await ObtenerOfertasDelDia(_dbContext);
                PortalAnunciosViewModel model = new PortalAnunciosViewModel(ObtenerSiguientesAnuncios(index.Value, anuncios), "", "", "", anuncios.Count, index.Value);
                return PartialView("_OfertasPartial", model);
            }
        }

        public ICollection<PortalAnuncioViewModel> ObtenerSiguientesAnuncios(int index, List<Anuncio> anuncios)
        {
            int num = 20;
            int count = anuncios.Count;
            if (index + num > count)
            {
                num = count - index;
            }

            var result = CreaAnuncios(anuncios.GetRange(index, num));
            return result;
        }

        public async Task<List<Anuncio>> ObtenerAnunciosDestacados(MercampoEntities _dbContext)
        {
            return await Task.Run(() =>
            {

                Startup.OpenDatabaseConnection(_dbContext);
                if (_dbContext.Database.Connection.State != ConnectionState.Open)
                {
                    return null;
                }

                return _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado).OrderByDescending(a => a.clicks).ToList();

            });
        }

        public List<Anuncio> TempAnuncios()
        {
            List<Anuncio> lista = new List<Anuncio>();
            for (int i = 0; i < 342; i++)
            {
                Anuncio a = new Anuncio();
                a.titulo = i.ToString();
                a.Subcategoria = new Subcategoria() { nombre = "Subcategoria", Categoria = new Categoria() { nombre = "Categoria" } };
                List<Fotos_Anuncio> listaF = new List<Fotos_Anuncio>();
                listaF.Add(new Fotos_Anuncio() { principal = true, ruta = "img" });
                a.Fotos_Anuncio = listaF;
                a.Estado1 = new Estado() { nombre = "Estado", Pai = new Pai() { nombre = "Pais" } };
                lista.Add(a);
            }
            return lista;
        }

        [HttpPost]
        public async Task<ActionResult> DestacadosFiltradosPartial(int? idCategoria, int? idSubcategoria, int? idPais, int? idEstado, double? precioBajo, double? precioAlto)
        {
            PortalAnunciosViewModel model = new PortalAnunciosViewModel(await ObtenerDestacadosFiltrados(idCategoria, idSubcategoria, idPais, idEstado, precioBajo, precioAlto), "", "", "");
            return PartialView("_AnunciosPartial", model);
        }

        [HttpPost]
        public async Task<ActionResult> DestacadosFiltradosMovilPartial(int? idCategoria, int? idSubcategoria, int? idPais, int? idEstado, double? precioBajo, double? precioAlto)
        {
            PortalAnunciosViewModel model = new PortalAnunciosViewModel(await ObtenerDestacadosFiltrados(idCategoria, idSubcategoria, idPais, idEstado, precioBajo, precioAlto), "", "", "");
            return PartialView("_AnunciosMovil", model);
        }

        public ActionResult ResultadosBusqueda(string query)
        {
            return View(new QueryViewModel(query));
        }

        public async Task<ActionResult> ObtenerDestacadosFiltradosTexto(string query)
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

                    IQueryable<Anuncio> anuncios = null;

                    if (query == null)
                    {
                        return null;
                    }

                    anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && (
                    a.Subcategoria.nombre.Contains(query) == true || a.Subcategoria.Categoria.nombre.Contains(query) == true || a.Estado1.nombre.Contains(query) == true
                    || a.Estado1.Pai.nombre.Contains(query) == true || a.titulo.Contains(query) == true)).Take(20);

                    _dbContext.Database.Connection.Close();
                    PortalAnunciosBusquedaViewModel model = new PortalAnunciosBusquedaViewModel(CreaAnuncios(anuncios.ToList()), query);
                    return PartialView("_BusquedaPartial", model);
                }
            });

        }


        public async Task<ICollection<PortalAnuncioViewModel>> ObtenerDestacadosFiltrados(int? idCategoria, int? idSubcategoria, int? idPais, int? idEstado, double? precioBajo, double? precioAlto)
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

                    IQueryable<Anuncio> anuncios = null;

                    if (idCategoria == -1 && idSubcategoria == -1 && idPais == -1 && idEstado == -1)
                    {
                        anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.precio <= precioAlto && a.precio >= precioBajo).OrderByDescending(a => a.clicks).Take(20);
                        _dbContext.Database.Connection.Close();
                        return CreaAnuncios(anuncios.ToList());
                    }

                    if (idCategoria != -1)
                    {
                        if (idSubcategoria != -1)
                        {
                            if (idPais != -1)
                            {
                                if (idEstado != -1)
                                {
                                    anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.idSubcategoria == idSubcategoria && a.idEstado == idEstado && a.precio <= precioAlto && a.precio >= precioBajo).OrderByDescending(a => a.clicks).Take(20);
                                    _dbContext.Database.Connection.Close();
                                    return CreaAnuncios(anuncios.ToList());
                                }
                                else
                                {
                                    anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.idSubcategoria == idSubcategoria && a.Estado1.idPais == idPais && a.precio <= precioAlto && a.precio >= precioBajo).OrderByDescending(a => a.clicks).Take(20);
                                    _dbContext.Database.Connection.Close();
                                    return CreaAnuncios(anuncios.ToList());
                                }
                            }
                            else
                            {
                                anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.idSubcategoria == idSubcategoria && a.precio <= precioAlto && a.precio >= precioBajo).OrderByDescending(a => a.clicks).Take(20);
                                _dbContext.Database.Connection.Close();
                                return CreaAnuncios(anuncios.ToList());
                            }
                        }
                        else
                        {
                            anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.Subcategoria.idCategoria == idCategoria && a.precio <= precioAlto && a.precio >= precioBajo).OrderByDescending(a => a.clicks).Take(20);
                            _dbContext.Database.Connection.Close();
                            return CreaAnuncios(anuncios.ToList());
                        }
                    }
                    else
                    {
                        if (idPais != -1)
                        {
                            if (idEstado != -1)
                            {
                                anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.idEstado == idEstado && a.precio <= precioAlto && a.precio >= precioBajo).OrderByDescending(a => a.clicks).Take(20);
                                _dbContext.Database.Connection.Close();
                                return CreaAnuncios(anuncios.ToList());
                            }
                            else
                            {
                                anuncios = _dbContext.Anuncios.Where(a => a.activo == true && a.estado == (int)EstadoAnuncio.Aprobado && a.Estado1.idPais == idPais).OrderByDescending(a => a.clicks).Take(20);
                                _dbContext.Database.Connection.Close();
                                return CreaAnuncios(anuncios.ToList());
                            }
                        }
                    }

                    var result = CreaAnuncios(anuncios.ToList());
                    _dbContext.Database.Connection.Close();

                    return result;
                }
            });
        }

        public async Task<List<Anuncio>> ObtenerOfertasDelDia(MercampoEntities _dbContext)
        {
            return await Task.Run(() =>
            {
                Startup.OpenDatabaseConnection(_dbContext);
                if (_dbContext.Database.Connection.State != ConnectionState.Open)
                {
                    return null;
                }
                var anuncios = _dbContext.Anuncios.Where(a => a.activo == true &&
                   a.estado == (int)EstadoAnuncio.Aprobado && a.Anuncio_Beneficio.Where(ab => ab.idAnuncio == a.id &&
                   ab.Beneficio.tipo == (int)BeneficiosExtraTipo.OfertaDelDia).FirstOrDefault() != null);
                return Shuffle(anuncios.ToList());
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

        public async Task<ActionResult> GaleriaAnuncio(int? id)
        {
            if (id == null)
            {
                return HttpNotFound("Parámetro inválido se espera un id de un anuncio");
            }

            HttpNotFoundResult result = null;
            GaleriaAnuncioViewModel model = null;

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

                            List<PaginaFotoViewModel> fotos = new List<PaginaFotoViewModel>();

                            foreach (var foto in anuncio.Fotos_Anuncio)
                            {
                                fotos.Add(new PaginaFotoViewModel(foto.principal, foto.ruta));
                            }

                            model = new GaleriaAnuncioViewModel(fotos);
                        }
                    }
                }
            });

            if (result != null)
            {
                return result;
            }

            return PartialView("GaleriaAnuncio", model);
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
                                anuncio.Estado1.nombre, anuncio.Fotos_Anuncio.Where(f => f.principal == true).FirstOrDefault()?.ruta);

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

            Random r = new Random((int)DateTime.Now.Ticks & (0x0000FFFF + new Random().Next(0, anuncios.Count)));
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

        private List<PortalAnuncioViewModel> CreaAnuncios(ICollection<Anuncio> anuncios)
        {
            List<PortalAnuncioViewModel> lista = new List<PortalAnuncioViewModel>();
            try
            {
                foreach (var item in anuncios)
                {
                    lista.Add(new PortalAnuncioViewModel(item.id, item.titulo, item.precio, item.Subcategoria.Categoria.nombre, item.Subcategoria.nombre, item.Estado1.nombre, item.Fotos_Anuncio.Where(f => f.principal == true).FirstOrDefault()?.ruta));
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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

        [HttpPost]
        public async Task<ActionResult> BannerCentralPartial()
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

                    var banner = _dbContext.Banners.Where(b => b.tipo == (int)BannerTipo.Central).FirstOrDefault();

                    if (banner.activo == false)
                    {
                        return PartialView("BannerCentralPartial", null);
                    }

                    var model = new PaginaBannerCentralViewModel(banner.Id, banner.ruta, banner.link);

                    _dbContext.Database.Connection.Close();
                    return PartialView("BannerCentralPartial", model);
                }
            });

        }

        [HttpPost]
        public async Task<ActionResult> BannersLateralesPartial()
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

                    List<PaginaBannerCentralViewModel> lista = new List<PaginaBannerCentralViewModel>();
                    var banners = _dbContext.Banners.Where(b => b.tipo != (int)BannerTipo.Central && b.ruta != null && b.activo == true);

                    foreach (var banner in banners)
                    {
                        lista.Add(new PaginaBannerCentralViewModel(banner.Id, banner.ruta, banner.link));
                    }

                    _dbContext.Database.Connection.Close();

                    var model = new PaginaBannersLateralesViewModel(lista);
                    return PartialView("BannersLateralesPartial", model);
                }
            });

        }

    }
}