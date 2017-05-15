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
using Newtonsoft.Json.Linq;
using VendeAgroWeb.Models.Pagina;

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

        public async Task<ActionResult> ConfirmarMail(string token)
        {
            if (token == null)
            {
                ViewData["ResultadoMail"] = ConfirmacionMailStatus.TokenInvalido;
                return View();
            }
            var result = await Startup.GetAplicacionUsuariosManager().ConfirmarMailPortalAsync(token);
            ViewData["ResultadoMail"] = result;
            return View();
        }

        public async Task<ActionResult> RenovarAnuncio(int? id)
        {
            var usuario = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request);
            if (usuario == null)
            {
                return RedirectToAction("Index", "Home");
            }

            if (!id.HasValue || !ValidateAnuncio(id.Value, usuario.Id))
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Invalid url parameters, please include a valid id.");
            }

            AnunciateViewModel model = new AnunciateViewModel(await ObtenerPaquetes());
            model.IdAnuncio = id.Value;
            return View(model);
        }

        private bool ValidateAnuncio(int idAnuncio, int idUsuario)
        {
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
            }
            return true;
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

        public async Task<ActionResult> DetallesAnuncio(int? id)
        {
            if (Startup.GetAplicacionUsuariosManager().VerificarPortalSesion() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Index", "Home");
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
                            var tiempoRestante = (anuncio.fecha_fin.Value - DateTime.Now).Days;
                            var imagenPrincipal = anuncio.Fotos_Anuncio.Where(foto => foto.principal == true).FirstOrDefault()?.ruta ?? string.Empty;

                            var paquete = _dbContext.Paquetes.Where(p => p.id == anuncio.idPaquete).FirstOrDefault();

                            PaqueteViewModel paqueteViewModel = null;
                            if (paquete != null)
                            {
                                paqueteViewModel = new PaqueteViewModel(paquete.id, paquete.nombre, paquete.meses);
                            }

                            var beneficios = _dbContext.Anuncio_Beneficio.Where(b => b.idAnuncio == anuncio.id);

                            List<BeneficioViewModel> listaBeneficios = new List<BeneficioViewModel>();
                            foreach (var beneficio in beneficios)
                            {
                                listaBeneficios.Add(new BeneficioViewModel(beneficio.idBeneficio, beneficio.Beneficio.descripcion, beneficio.Beneficio.precio));
                            }

                            var anuncioModel = new AnuncioViewModel(anuncio.id, anuncio.titulo, anuncio.estado, tiempoRestante, imagenPrincipal, paqueteViewModel, listaBeneficios);

                            var rutaVideo = _dbContext.Videos_Anuncio.Where(v => v.idAnuncio == id).FirstOrDefault()?.ruta;

                            List<FotoViewModel> fotos = new List<FotoViewModel>();

                            foreach (var foto in anuncio.Fotos_Anuncio)
                            {
                                fotos.Add(new FotoViewModel(foto.principal, foto.ruta));
                            }
                            model = new AnuncioDetallesViewModel(anuncioModel, anuncio.precio, anuncio.descripcion, anuncio.Subcategoria.Categoria.nombre, anuncio.Subcategoria.nombre, anuncio.Estado1.Pai.nombre, anuncio.Estado1.nombre, fotos, anuncio.fecha_inicio, anuncio.fecha_fin, rutaVideo, anuncio.razonRechazo);
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

        [HttpPost]
        public async Task<bool> ModificarAnuncio(string json)
        {
            var anuncio = JObject.Parse(json);
            var id = (int)anuncio["jid"];
            var titulo = (string)anuncio["jtitulo"];
            var descripcion = (string)anuncio["jdescripcion"];
            var precio = (double)anuncio["jprecio"];
            var idSubcategoria = (int)anuncio["jidSubcategoria"];
            var idEstado = (int)anuncio["jestado"];
            var fotoDisplayId = (int)anuncio["jfotoDisplayId"];
            var fotoDisplay = (string)anuncio["jfotoDisplay"];
            var fotos = (JArray)anuncio["jfotos"];
            var fotosEliminadas = (JArray)anuncio["jfotos"];
            var fotosEliminadasRutas = (JArray)anuncio["jfotos"];
            var video = (string)anuncio["jvideo"];
            bool estado = true;

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
                        var anuncioDb = _dbContext.Anuncios.FirstOrDefault(a => a.id == id);

                        if (anuncioDb == null)
                        {
                            ModelState.AddModelError("", "Error anuncio no encontrado, vuelva a intentarlo");
                            estado = false;
                        }
                        else
                        {
                            anuncioDb.titulo = titulo;
                            anuncioDb.descripcion = descripcion;
                            anuncioDb.precio = precio;
                            anuncioDb.idSubcategoria = idSubcategoria;
                            anuncioDb.idEstado = idEstado;
                            anuncioDb.estado = (int)EstadoAnuncio.PendientePorAprobar;

                            foreach (var foto in fotosEliminadas)
                            {
                                var idFoto = (int)foto;
                                var fotoActual = _dbContext.Fotos_Anuncio.FirstOrDefault(f => f.id == idFoto);
                                _dbContext.Fotos_Anuncio.Remove(fotoActual);
                            }

                            //borrarFotos(fotosEliminadasRutas);

                            var fotoDb = _dbContext.Fotos_Anuncio.FirstOrDefault(f => f.id == fotoDisplayId);

                            if (fotoDb == null)
                            {
                                ModelState.AddModelError("", "Error anuncio no encontrado, vuelva a intentarlo");
                                estado = false;
                            }

                            fotoDb.ruta = fotoDisplay;

                            foreach (var item in fotos)
                            {
                                var url = (string)item;
                                _dbContext.Fotos_Anuncio.Add(new Fotos_Anuncio
                                {
                                    ruta = url,
                                    idAnuncio = anuncioDb.id,
                                    principal = false
                                });
                            }

                            if (!string.IsNullOrEmpty(video))
                            {
                                _dbContext.Videos_Anuncio.Add(new Videos_Anuncio
                                {
                                    ruta = video,
                                    idAnuncio = anuncioDb.id
                                });
                            }

                            _dbContext.SaveChanges();
                        }

                        _dbContext.SaveChanges();
                    }

                    _dbContext.Database.Connection.Close();
                    return true;
                }
            });
        }

        public async Task<ActionResult> ModificarAnuncio(int? id)
        {
            if (id == null)
            {
                return RedirectToAction("MisAnuncios", "Portal");
            }

            bool estado = true;
            ModificarAnuncioViewModel model = null;

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

                        var anuncio = _dbContext.Anuncios.Where(a => a.id == id).FirstOrDefault();

                        List<FotoViewModel> fotos = new List<FotoViewModel>();
                        FotoViewModel fotoPrincipal = null;

                        foreach (var foto in anuncio.Fotos_Anuncio)
                        {
                            if (foto.principal == true)
                            {
                                fotoPrincipal = new FotoViewModel(foto.id, true, foto.ruta);
                                continue;
                            }
                            fotos.Add(new FotoViewModel(foto.id, false, foto.ruta));
                        }

                        model = new ModificarAnuncioViewModel(anuncio.id, anuncio.titulo, anuncio.Usuario.nombre + " " + anuncio.Usuario.apellidos, anuncio.precio, new CategoriaModificarAnuncioViewModel(anuncio.Subcategoria.idCategoria, anuncio.Subcategoria.Categoria.nombre),
                            new SubcategoriaModificarAnuncioViewModel(anuncio.idSubcategoria, anuncio.Subcategoria.nombre), new PaisModificarAnuncioViewModel(anuncio.Estado1.idPais, anuncio.Estado1.Pai.nombre), new EstadoModificarAnuncioViewModel(anuncio.idEstado, anuncio.Estado1.nombre),
                            anuncio.descripcion, fotoPrincipal, fotos, anuncio.Videos_Anuncio.Where(v => v.idAnuncio == id).FirstOrDefault()?.ruta, anuncio.razonRechazo);


                        _dbContext.Database.Connection.Close();
                    }
                }

            });
            if (!estado)
            {
                return RedirectToAction("MisAnuncios", "Portal");
            }
            return View(model);
        }


        public async Task<ActionResult> AnunciosActivosPartial()
        {
            var id = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request).Id;
            MisAnunciosViewModel model = new MisAnunciosViewModel(await ObtenerAnunciosActivos(id));
            return PartialView("AnunciosActivosPartial", model);
        }

        public async Task<ActionResult> AnunciosVencidosPartial()
        {
            var id = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request).Id;
            MisAnunciosViewModel model = new MisAnunciosViewModel(await ObtenerAnunciosVencidos(id));
            return PartialView("AnunciosVencidosPartial", model);
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
                    
                    var anuncios = _dbContext.Anuncios.Where(a => ((EstadoAnuncio)a.estado != EstadoAnuncio.Vencido && a.idUsuario == id) || ((EstadoAnuncio)a.estado == EstadoAnuncio.Vacio && a.idUsuario == id));
                    foreach (var item in anuncios)
                    {
                        var paquete = _dbContext.Paquetes.Where(p => p.id == item.idPaquete).FirstOrDefault();

                        PaqueteViewModel paqueteViewModel = null;
                        if (paquete != null)
                        {
                            paqueteViewModel = new PaqueteViewModel(paquete.id, paquete.nombre, paquete.meses);
                        }

                        var beneficios = _dbContext.Anuncio_Beneficio.Where(b => b.idAnuncio == item.id);

                        List<BeneficioViewModel> listaBeneficios = new List<BeneficioViewModel>();
                        foreach (var beneficio in beneficios)
                        {
                            listaBeneficios.Add(new BeneficioViewModel(beneficio.idBeneficio, beneficio.Beneficio.descripcion, beneficio.Beneficio.precio));
                        }

                        if ((EstadoAnuncio)item.estado == EstadoAnuncio.Vacio)
                        {
                            lista.Add(new AnuncioViewModel(item.id, (EstadoAnuncio)item.estado, paqueteViewModel, listaBeneficios));
                            continue;
                        }
                        int tiempoRestante = (item.fecha_fin.Value - DateTime.Now).Days;
                        var duracion = (item.fecha_fin.Value - item.fecha_inicio.Value).Days;
                        var porcentajeDuracion = (int)((tiempoRestante * 100.0) / duracion);
                        if (porcentajeDuracion < 0) porcentajeDuracion = 0;
                        var imagenPrincipal = item.Fotos_Anuncio.Where(foto => foto.principal == true).FirstOrDefault()?.ruta ?? string.Empty;

                        lista.Add(new AnuncioViewModel(item.id, item.titulo, item.estado, porcentajeDuracion, imagenPrincipal, paqueteViewModel, listaBeneficios));
                    }

                    _dbContext.Database.Connection.Close();
                    var fixedOrder = new List<EstadoAnuncio>() {
                            EstadoAnuncio.NoAprobado,
                            EstadoAnuncio.Vacio,
                            EstadoAnuncio.PendientePorAprobar,
                            EstadoAnuncio.Aprobado
                    };

                    List<AnuncioViewModel> listaOrdenada = lista.OrderBy(a => fixedOrder.IndexOf(a.Estado)).ThenBy(a => a.TiempoRestante).ToList();
                    return listaOrdenada;
                }
            });
        }

        public async Task<ICollection<AnuncioViewModel>> ObtenerAnunciosVencidos(int id)
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

                    var anuncios = _dbContext.Anuncios.Where(a => a.activo == false && a.idUsuario == id && (EstadoAnuncio)a.estado == EstadoAnuncio.Vencido);

                    foreach (var item in anuncios)
                    {
                        var imagenPrincipal = item.Fotos_Anuncio.Where(foto => foto.principal == true).FirstOrDefault()?.ruta ?? string.Empty;
                        var paquete = _dbContext.Paquetes.Where(p => p.id == item.idPaquete).FirstOrDefault();

                        PaqueteViewModel paqueteViewModel = null;
                        if (paquete != null)
                        {
                            paqueteViewModel = new PaqueteViewModel(paquete.id, paquete.nombre, paquete.meses);
                        }

                        var beneficios = _dbContext.Anuncio_Beneficio.Where(b => b.idAnuncio == item.id);

                        List<BeneficioViewModel> listaBeneficios = new List<BeneficioViewModel>();
                        foreach (var beneficio in beneficios)
                        {
                            listaBeneficios.Add(new BeneficioViewModel(beneficio.idBeneficio, beneficio.Beneficio.descripcion, beneficio.Beneficio.precio));
                        }

                        lista.Add(new AnuncioViewModel(item.id, item.titulo, item.estado, null, imagenPrincipal, paqueteViewModel, listaBeneficios));
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
            if (id == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if (Startup.GetAplicacionUsuariosManager().VerificarPortalSesion() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Index", "Home");
            }
            NuevaTarjetaViewModel model = new NuevaTarjetaViewModel(id.Value);
            return View(model);
        }

        [HttpPost]
        public ActionResult LoginPartial()
        {
            return PartialView("_Login");

        }

        public ActionResult Registro()
        {
            RegistroViewModel model = new RegistroViewModel();
            return View("_Registro", model);
        }

        public async Task<ActionResult> OlvidasteContrasenaPartial()
        {
            OlvidasteContrasenaViewModel model = new OlvidasteContrasenaViewModel();
            return PartialView("_OlvidasteContrasena", model);
        }

        [HttpPost]
        public async Task<ActionResult> OlvidasteContrasena(string email)
        {
            var resultado = await Startup.GetAplicacionUsuariosManager().OlvidoContrasenaPortalAsync(email);
            ViewData["ResultadoMail"] = resultado.ToString();
            return PartialView("_OlvidasteContrasena");
        }

        public async Task<ICollection<PagoViewModel>> ObtenerPagos(int id)
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

                    List<PagoViewModel> lista = new List<PagoViewModel>();
                    var pagos = _dbContext.Pagoes.Where(p => p.idUsuario == id);

                    foreach (var item in pagos)
                    {
                        lista.Add(new PagoViewModel(item.id, item.total, item.fecha, item.digitosTarjeta, item.Referencia));
                    }

                    _dbContext.Database.Connection.Close();
                    return lista;
                }
            });
        }

        public async Task<ActionResult> DetallesPago(int? id)
        {
            if (Startup.GetAplicacionUsuariosManager().VerificarPortalSesion() == LoginStatus.Incorrecto)
            {
                return RedirectToAction("Index", "Home");
            }

            if (id == null)
            {
                return HttpNotFound("Parámetro inválido se espera un id de un pago");
            }

            HttpNotFoundResult result = null;
            DetallesPagoViewModel model = null;
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
                        var conceptos = _dbContext.Pago_Concepto.Where(p => p.id == id);
                        if (conceptos == null)
                        {
                            result = HttpNotFound("No se encontro el pago con el id solicitado");
                        }
                        else
                        {
                            List<PagoConceptoViewModel> listaConceptos = new List<PagoConceptoViewModel>();
                            foreach (var concepto in conceptos)
                            {
                                if (concepto.descripcionBeneficio == null)
                                {
                                    listaConceptos.Add(new PagoConceptoViewModel(concepto.tipo, concepto.nombrePaquete, concepto.mesesPaquete, concepto.precioPaquete));
                                    continue;
                                }
                                listaConceptos.Add(new PagoConceptoViewModel(concepto.tipo, concepto.nombrePaquete, concepto.mesesPaquete, concepto.precioPaquete, concepto.descripcionBeneficio, concepto.precioBeneficio));
                            }

                            model = new DetallesPagoViewModel(listaConceptos);
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


        public async Task<ActionResult> MisPagos()
        {
            var id = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request).Id;
            MisPagosViewModel model = new MisPagosViewModel(await ObtenerPagos(id));
            return View(model);
        }

        public async Task<ActionResult> Perfil()
        {
            var usuario = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request);
            PerfilViewModel model = new PerfilViewModel(usuario);
            return View(model);
        }

        public async Task<bool> CambiarNombre(string nombre, string apellidos)
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
        public string AgregarTarjeta(int? id, string tokenTarjeta, string sessionId)
        {
            return Startup.GetAplicacionUsuariosManager().AgregarTarjetaAsync(id.Value, tokenTarjeta, sessionId);
        }

        [HttpPost]
        public string RealizarCargo(int? id, string tokenTarjeta, string sessionId)
        {
            CarritoDeCompra carrito = Startup.GetCarritoDeCompra(Request.Cookies);
            string resultado;
            if (Startup.GetAplicacionUsuariosManager().RealizarCargoTarjeta(id.Value, tokenTarjeta, sessionId, carrito, out resultado))
            {
                carrito.Clear();
                Startup.UpdateCarritoCookie(carrito, Response);
            }
            return resultado;
        }

        public async Task<ActionResult> CrearAnuncio(int? id)
        {
            var usuario = Startup.GetAplicacionUsuariosManager().getUsuarioPortalActual(Request);
            if(usuario == null)
            {
                return RedirectToAction("Index", "Home");
            }
            if(!id.HasValue)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest,"Url invalido, debe de contener id del anuncio.");
            }
            CrearAnuncioViewModel model = new CrearAnuncioViewModel();
            int result = 0;
            int idAnuncio = id.Value;
            await Task.Run(() =>
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != ConnectionState.Open)
                    {
                        result = 0;
                    }
                    else
                    {
                        Anuncio anuncio = _dbContext.Anuncios.Where(a => a.id == id).FirstOrDefault();
                        if (anuncio == null || anuncio.idUsuario != usuario.Id)
                        {
                            result = 1;
                        }
                        else
                        {
                            int numFotos = 3;
                            bool video = false;

                            var beneficios = _dbContext.Anuncio_Beneficio.Where(a => a.idAnuncio == id);

                            foreach (var beneficio in beneficios)
                            {
                                if (beneficio.idBeneficio == 2)
                                {
                                    numFotos += 5;
                                    continue;
                                }
                                if (beneficio.idBeneficio == 3)
                                {
                                    video = true;
                                }
                            }

                            _dbContext.Database.Connection.Close();
                            model._numFotos = numFotos;
                            model._video = video;
                            model.IdUsuario = usuario.Id;
                            model.IdAnuncio = idAnuncio;
                        }
                    }
                }
            });
            if (result != 0)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, "Usuario no tiene un anuncio con el id recibido");
            }
            return View(model);
        }

        [HttpPost]
        public async Task<bool> CrearAnuncio(string json)
        {
            var anuncio = JObject.Parse(json);
            var titulo = (string)anuncio["jtitulo"];
            var descripcion = (string)anuncio["jdescripcion"];
            var precio = (double)anuncio["jprecio"];
            var idUsuario = (int)anuncio["jidUsuario"];
            var idSubcategoria = (int)anuncio["jidSubcategoria"];
            var idEstado = (int)anuncio["jestado"];
            var fotoDisplay = (string)anuncio["jfotoDisplay"];
            var fotos = (JArray)anuncio["jfotos"];
            var video = (string)anuncio["jvideo"];
            var idAnuncio = (int)anuncio["idAnuncio"];

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
                        Anuncio anuncioACrear = _dbContext.Anuncios.Where(a => a.id == idAnuncio).FirstOrDefault();
                        if(anuncioACrear == null || anuncioACrear.estado != (int)EstadoAnuncio.Vacio)
                        {
                            ModelState.AddModelError("", "Error el id del anuncio a crear es incorrecto, vuelva a intentarlo");
                            return false;
                        }
                        
                        anuncioACrear.titulo = titulo;
                        anuncioACrear.descripcion = descripcion;
                        anuncioACrear.precio = precio;
                        anuncioACrear.activo = false;
                        anuncioACrear.idUsuario = idUsuario;
                        anuncioACrear.idSubcategoria = idSubcategoria;
                        anuncioACrear.idEstado = idEstado;
                        anuncioACrear.estado = (int)EstadoAnuncio.PendientePorAprobar;
                        anuncioACrear.clicks = 0;
                        anuncioACrear.vistas = 0;
                        anuncioACrear.fecha_inicio = DateTime.Now;
                        anuncioACrear.fecha_fin = DateTime.Now.AddMonths(anuncioACrear.Paquete.meses);

                        _dbContext.Fotos_Anuncio.Add(new Fotos_Anuncio
                        {
                            ruta = fotoDisplay,
                            idAnuncio = anuncioACrear.id,
                            principal = true
                        });

                        foreach (var item in fotos)
                        {
                            var url = (string)item;
                            _dbContext.Fotos_Anuncio.Add(new Fotos_Anuncio
                            {
                                ruta = url,
                                idAnuncio = anuncioACrear.id,
                                principal = false
                            });
                        }

                        if (!string.IsNullOrEmpty(video))
                        {
                            _dbContext.Videos_Anuncio.Add(new Videos_Anuncio
                            {
                                ruta = video,
                                idAnuncio = anuncioACrear.id
                            });
                        }

                        _dbContext.SaveChanges();
                    }

                    _dbContext.Database.Connection.Close();
                    return true;
                }
            });
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
