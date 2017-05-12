using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace VendeAgroWeb.Models.Administrador
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Introduzca un email válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }

    public class OlvidoContrasenaViewModel
    {
        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Introduzca un email válido")]
        public string Email { get; set; }
    }

    public class CambiarContrasenaViewModel
    {
        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(20, ErrorMessage = "La contraseña debe contener como mínimo 6 caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [DataType(DataType.Password)]
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "La contraseña y su confirmacion no son iguales.")]
        [Display(Name = "ConfirmaContraseña")]
        public string ConfirmaPassword { get; set; }

        public string Token { get; set; }
    }

    public class UsuarioAdministradorViewModel
    {
        private int _id;
        private string _nombre;
        private string _email;
        private bool _activo;

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }

        }

        public bool Activo
        {
            get
            {
                return _activo;
            }
        }

        public UsuarioAdministradorViewModel(int id, string nombre, string email, bool activo)
        {
            _id = id;
            _nombre = nombre;
            _email = email;
            _activo = activo;
        }
    }

    public class UsuarioPortalViewModel
    {
        private int _id;
        private string _nombre;
        private string _apellidos;
        private string _telefono;
        private string _email;
        private int _numAnuncios;

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
        }

        public string Apellidos
        {
            get
            {
                return _apellidos;
            }
        }

        public string Telefono
        {
            get
            {
                return _telefono;
            }

        }

        public string Email
        {
            get
            {
                return _email;
            }
        }

        public int NumAnuncios
        {
            get
            {
                return _numAnuncios;
            }
        }

        public UsuarioPortalViewModel(int id, string nombre, string apellidos, string telefono, string email, int numAnuncios)
        {
            _id = id;
            _nombre = nombre;
            _apellidos = apellidos;
            _telefono = telefono;
            _email = email;
            _numAnuncios = numAnuncios;
        }
    }


    public class UsuariosViewModel
    {

        private int _tipo;
        private ICollection<UsuarioAdministradorViewModel> _usuariosAdministrador;
        private ICollection<UsuarioPortalViewModel> _usuariosPortal;

        public UsuariosViewModel(int tipo, ICollection<UsuarioAdministradorViewModel> usuariosAdmin, ICollection<UsuarioPortalViewModel> usuariosPortal)
        {
            _tipo = tipo;
            _usuariosAdministrador = usuariosAdmin;
            _usuariosPortal = usuariosPortal;
        }

        public int Tipo
        {
            get
            {
                return _tipo;
            }
        }

        public ICollection<UsuarioAdministradorViewModel> UsuariosAdministrador
        {
            get
            {
                return _usuariosAdministrador;
            }
        }

        public ICollection<UsuarioPortalViewModel> UsuariosPortal
        {
            get
            {
                return _usuariosPortal;
            }
        }
    }

    public class NuevoPaqueteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "NuevoNombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Meses")]
        [Range(0, int.MaxValue, ErrorMessage = "Introduzca un valor numérico")]
        public int Meses { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Precio del paquete")]
        [Range(0, float.MaxValue, ErrorMessage = "Introduzca un valor numérico")]
        public double Precio { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Descripción del paquete")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El número de caracteres no puede ser mayor a 200")]
        public string Descripcion { get; set; }
    }

    public class PaqueteViewModel {
        private int _id;
        private string _nombre;
        private int _meses;
        private double _precio;
        private string _descripcion;
        private bool _paqueteBase;
        private DateTime _fechaModificacion;
        private bool _activo;
        private double _ahorro;


        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
        }

        public int Meses
        {
            get
            {
                return _meses;
            }
        }

        public double Precio
        {
            get
            {
                return _precio;
            }
        }

        public string Descripcion
        {
            get
            {
                return _descripcion;
            }
        }

        public bool PaqueteBase
        {
            get
            {
                return _paqueteBase;
            }
        }

        public DateTime FechaModificacion
        {
            get
            {
                return _fechaModificacion;
            }
        }

        public bool Activo
        {
            get
            {
                return _activo;
            }
        }

        public double Ahorro
        {
            get
            {
                return _ahorro;
            }
        }

        public PaqueteViewModel(int id, string nombre, int meses, double precio, string descripcion, bool paqueteBase, DateTime fechaModificacion, bool activo, double ahorro) {
            _id = id;
            _nombre = nombre;
            _meses = meses;
            _precio = precio;
            _descripcion = descripcion;
            _paqueteBase = paqueteBase;
            _fechaModificacion = fechaModificacion;
            _activo = activo;
            _ahorro = ahorro;
        }
    }

    public class PaquetesViewModel {
        private ICollection<PaqueteViewModel> _paquetes;

        public PaquetesViewModel(ICollection<PaqueteViewModel> paquetes) {
            _paquetes = paquetes;
        }

        public ICollection<PaqueteViewModel> Paquetes
        {
            get
            {
                return _paquetes;
            }
        }
    }

    public class AnuncioViewModel {
        private int _id;
        private string _titulo;
        private string _usuario;
        private double? _precio;
        private string _categoria;
        private string _subcategoria;
        private string _estado;
        private int? _clicks;

        public EstadoAnuncio Status { get; set; }

        public bool Activo { get; set; }

        public string Titulo
        {
            get
            {
                return _titulo;
            }
        }

        public double? Precio
        {
            get
            {
                return _precio;
            }
        }

        public string Categoria
        {
            get
            {
                return _categoria;
            }
        }

        public string Subcategoria
        {
            get
            {
                return _subcategoria;
            }
        }

        public string Estado
        {
            get
            {
                return _estado;
            }
        }

        public int? Clicks
        {
            get
            {
                return _clicks;
            }
        }

        public string Usuario
        {
            get
            {
                return _usuario;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public AnuncioViewModel(int id, string titulo, string usuario, double? precio, string categoria, string subcategoria, string estado, int? clicks, EstadoAnuncio status, bool activo) {
            _id = id;
            _titulo = titulo;
            _usuario = usuario;
            _precio = precio ?? 0.0;
            _categoria = categoria;
            _subcategoria = subcategoria;
            _estado = estado;
            _clicks = clicks ?? 0;
            Status = status;
            Activo = activo;
        }
    }

    public class AnunciosPendientesViewModel {
    }

    public class AnuncioPaqueteViewModel
    {
        private string _nombre;
        private bool _activo;

        public AnuncioPaqueteViewModel(string nombre, bool activo)
        {
            _nombre = nombre;
            _activo = activo;
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }

        }

        public bool Activo
        {
            get
            {
                return _activo;
            }

        }

    }

    public class FotoViewModel
    {
        private int _id;
        private bool _principal;
        private string _ruta;

        public FotoViewModel(bool principal, string ruta)
        {
            _principal = principal;
            _ruta = ruta;
        }

        public FotoViewModel(int id, bool principal, string ruta)
        {
            _id = id;
            _principal = principal;
            _ruta = ruta;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public bool Principal
        {
            get
            {
                return _principal;
            }
        }

        public string Ruta
        {
            get
            {
                return _ruta;
            }
        }
    }

    public class NuevoAnuncioViewModel
    {
        private int _idUsuario;
        private string _nombreUsuario;

        public string NombreUsuario
        {
            get
            {
                return _nombreUsuario;
            }
        }

        public int IdUsuario
        {
            get
            {
                return _idUsuario;
            }
        }

        public NuevoAnuncioViewModel()
        {

        }

        public NuevoAnuncioViewModel(int idUsuario, string nombreUsuario) {
            _idUsuario = idUsuario;
            _nombreUsuario = nombreUsuario;
        }
    }

    public class RenovarAnuncioViewModel
    {
        public int IdAnuncio { get; set; }
        public string TituloAnuncio { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Meses")]
        [Range(0, int.MaxValue, ErrorMessage = "Introduzca un valor numérico")]
        public int Meses { get; set; }

        public RenovarAnuncioViewModel()
        {
        }
    }

    public class CategoriaModificarAnuncioViewModel
    {
        private int? _id;
        private string _nombre;

        public int? Id
        {
            get
            {
                return _id;
            }
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
        }

        public CategoriaModificarAnuncioViewModel(int? id, string nombre)
        {
            _id = id;
            _nombre = nombre;
        }
    }

    public class SubcategoriaModificarAnuncioViewModel
    {
        private int? _id;
        private string _nombre;

        public int? Id
        {
            get
            {
                return _id;
            }
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
        }

        public SubcategoriaModificarAnuncioViewModel(int? id, string nombre)
        {
            _id = id;
            _nombre = nombre;
        }
    }

    public class PaisModificarAnuncioViewModel
    {
        private int _id;
        private string _nombre;

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
        }

        public PaisModificarAnuncioViewModel(int id, string nombre)
        {
            _id = id;
            _nombre = nombre;
        }
    }

    public class EstadoModificarAnuncioViewModel
    {
        private int? _id;
        private string _nombre;

        public int? Id
        {
            get
            {
                return _id;
            }
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
        }

        public EstadoModificarAnuncioViewModel(int? id, string nombre)
        {
            _id = id;
            _nombre = nombre;
        }
    }

    public class ModificarAnuncioViewModel
    {
        private int _id;
        private string _titulo;
        private string _usuarioNombre;
        private double? _precio;
        private CategoriaModificarAnuncioViewModel _categoria;
        private SubcategoriaModificarAnuncioViewModel _subcategoria;
        private PaisModificarAnuncioViewModel _pais;
        private EstadoModificarAnuncioViewModel _estado;
        private string _descripcion;
        private FotoViewModel _fotoPrincipal;
        private List<FotoViewModel> _fotos;
        private string _video;


        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Titulo
        {
            get
            {
                return _titulo;
            }
        }

        public string UsuarioNombre
        {
            get
            {
                return _usuarioNombre;
            }
        }

        public double? Precio
        {
            get
            {
                return _precio;
            }
        }

        public CategoriaModificarAnuncioViewModel Categoria
        {
            get
            {
                return _categoria;
            }
        }

        public PaisModificarAnuncioViewModel Pais
        {
            get
            {
                return _pais;
            }
        }

        public SubcategoriaModificarAnuncioViewModel Subcategoria
        {
            get
            {
                return _subcategoria;
            }
        }

        public EstadoModificarAnuncioViewModel Estado
        {
            get
            {
                return _estado;
            }
        }

        public string Descripcion
        {
            get
            {
                return _descripcion;
            }
        }

        public List<FotoViewModel> Fotos
        {
            get
            {
                return _fotos;
            }
        }

        public string Video
        {
            get
            {
                return _video;
            }
        }

        public FotoViewModel FotoPrincipal
        {
            get
            {
                return _fotoPrincipal;
            }
        }

        public ModificarAnuncioViewModel(int id, string titulo, string usuarioNombre, double? precio, CategoriaModificarAnuncioViewModel categoria, SubcategoriaModificarAnuncioViewModel subcategoria, 
            PaisModificarAnuncioViewModel pais, EstadoModificarAnuncioViewModel estado, string descripcion, FotoViewModel fotoPrincipal, List<FotoViewModel> fotos, string video)
        {
            _id = id;
            _titulo = titulo;
            _usuarioNombre = usuarioNombre;
            _precio = precio;
            _categoria = categoria;
            _subcategoria = subcategoria;
            _pais = pais;
            _estado = estado;
            _descripcion = descripcion;
            _fotoPrincipal = fotoPrincipal;
            _fotos = fotos;
            _video = video;
        }
    }

    public class AnuncioDetallesViewModel
    {
        private AnuncioViewModel _anuncio;
        private string _descripcion;
        private ICollection<FotoViewModel> _fotos;
        private DateTime? _fechaInicio;
        private DateTime? _fechaFin;
        private AnuncioPaqueteViewModel _paquete;
        private ICollection<BeneficioViewModel> _beneficios;
        private string _video;
        private string _razonRechazo;

        public AnuncioDetallesViewModel(AnuncioViewModel anuncio,
            string descripcion, List<FotoViewModel> fotos, DateTime? fechaInicio, 
            DateTime? fechaFin, AnuncioPaqueteViewModel paquete, ICollection<BeneficioViewModel> beneficios, string video, string razonRechazo)
        {
            _anuncio = anuncio;
            _descripcion = descripcion;
            _fotos = fotos;
            _paquete = paquete;
            _fechaInicio = fechaInicio;
            _fechaFin = fechaFin;
            _beneficios = beneficios;
            _video = video;
            _razonRechazo = razonRechazo;
        }

        public bool TieneVideo
        {
            get
            {
                return _video != null;
            }
        }

        public string Video
        {
            get
            {
                return _video;
            }
        }

        public AnuncioViewModel Anuncio
        {
            get
            {
                return _anuncio;
            }
        }

        public ICollection<FotoViewModel> Fotos
        {
            get
            {
                return _fotos;
            }
        }

        public string Descripcion
        {
            get
            {
                return _descripcion;
            }
        }

        public DateTime? FechaInicio
        {
            get
            {
                return _fechaInicio;
            }
        }

        public DateTime? FechaFin
        {
            get
            {
                return _fechaFin;
            }
        }

        public AnuncioPaqueteViewModel Paquete
        {
            get
            {
                return _paquete;
            }
        }

        public ICollection<BeneficioViewModel> Beneficios
        {
            get
            {
                return _beneficios;
            }
        }

        public string RazonRechazo
        {
            get
            {
                return _razonRechazo;
            }
        }
    }

    public class AnunciosViewModel {
        private ICollection<AnuncioViewModel> _anuncios;
        private string _nombreCategoria;
        private string _nombreSubcategoria;
        private string _nombreUsuario;
        private EstadoAnuncio _estadoAnuncios;

        public AnunciosViewModel(ICollection<AnuncioViewModel> anuncios, string nombreCategoria, string nombreSubcategoria, string nombreUsuario, EstadoAnuncio estadoAnuncios)
        {
            _anuncios = anuncios;
            _nombreCategoria = nombreCategoria;
            _nombreSubcategoria = nombreSubcategoria;
            _nombreUsuario = nombreUsuario;
            _estadoAnuncios = estadoAnuncios;
        }

        public ICollection<AnuncioViewModel> Anuncios
        {
            get
            {
                return _anuncios;
            }
        }

        public string NombreCategoria
        {
            get
            {
                return _nombreCategoria;
            }
        }

        public string NombreSubcategoria
        {
            get
            {
                return _nombreSubcategoria;
            }
        }

        public string NombreUsuario
        {
            get
            {
                return _nombreUsuario;
            }
        }

        public EstadoAnuncio EstadoAnuncios
        {
            get
            {
                return _estadoAnuncios;
            }
        }
    }

    public class SubcategoriasViewModel {
        private ICollection<SubcategoriaViewModel> _subcategorias;

        public SubcategoriasViewModel(ICollection<SubcategoriaViewModel> subcategorias) {
            _subcategorias = subcategorias;
        }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "ModificarNombre")]
        public string Titulo { get; set; }

        public ICollection<SubcategoriaViewModel> Subcategorias
        {
            get
            {
                return _subcategorias;
            }
        }
    }

    public class SubcategoriaViewModel {
        private int _id;
        private string _nombre;
        private bool _activo;
        private string _nombreCategoria;
        private int _numAnuncios;

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
        }

        public string NombreCategoria
        {
            get
            {
                return _nombreCategoria;
            }
        }

        public bool Activo
        {
            get
            {
                return _activo;
            }
        }

        public int NumAnuncios
        {
            get
            {
                return _numAnuncios;
            }
        }

        public SubcategoriaViewModel(int id, string nombre)
        {
            _id = id;
            _nombre = nombre;
        }

        public SubcategoriaViewModel(int id, string nombre, bool activo, string nombreCategoria, int numAnuncios) {
            _id = id;
            _nombre = nombre;
            _activo = activo;
            _nombreCategoria = nombreCategoria;
            _numAnuncios = numAnuncios;
        }
    }

    public class NuevoBannerViewModel
    {
        public int Id { get; set; }
        public int Tipo { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "NuevaRuta")]
        public string Ruta { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Link")]
        public string Link { get; set; }

    }

    public class BannersViewModel
    {
        private ICollection<BannerViewModel> _banners;

        public ICollection<BannerViewModel> Banners
        {
            get
            {
                return _banners;
            }
        }

        public BannersViewModel(ICollection<BannerViewModel> banners) {
            _banners = banners;
        }
    }

    public class BannerViewModel
    {
        private int _id;
        private string _ruta;
        private string _link;
        private bool _activo;
        private int _tipo;

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Ruta
        {
            get
            {
                return _ruta;
            }
        }

        public bool Activo
        {
            get
            {
                return _activo;
            }
        }

        public int Tipo
        {
            get
            {
                return _tipo;
            }
        }

        public string Link
        {
            get
            {
                return _link;
            }
        }

        public BannerViewModel(int id, string ruta, string link, bool activo, int tipo)
        {
            _id = id;
            _ruta = ruta;
            _link = link;
            _activo = activo;
            _tipo = tipo;
        }
    }

    public class PaisesViewModel {
        private ICollection<PaisViewModel> _paises;

        public ICollection<PaisViewModel> Paises
        {
            get
            {
                return _paises;
            }
        }

        public PaisesViewModel(ICollection<PaisViewModel> paises)
        {
            _paises = paises;
        }

    }

    public class EstadosViewModel
    {
        private ICollection<EstadoViewModel> _estados;

        public ICollection<EstadoViewModel> Estados
        {
            get
            {
                return _estados;
            }
        }

        public EstadosViewModel(ICollection<EstadoViewModel> estados)
        {
            _estados = estados;
        }

    }

    public class EstadoViewModel
    {
        private int _id;
        private string _nombre;

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
        }

        public EstadoViewModel(int id, string nombre)
        {
            _id = id;
            _nombre = nombre;
        }
    }

    public class PaisViewModel
    {
        private int _id;
        private string _nombre;

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
        }

        public PaisViewModel(int id, string nombre)
        {
            _id = id;
            _nombre = nombre;
        }
    }

    public class NuevaCategoriaViewModel {

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "NuevoNombre")]
        public string Nombre { get; set; }
    }

    public class CategoriasViewModel
    {
        private ICollection<CategoriaViewModel> _categorias;

        public CategoriasViewModel(ICollection<CategoriaViewModel> categorias)
        {
            _categorias = categorias;
        }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "ModificarNombre")]
        public string Titulo { get; set; }

        public ICollection<CategoriaViewModel> Categorias
        {
            get
            {
                return _categorias;
            }
        }
    }

    public class CategoriaViewModel
    {
        private int _id;
        private string _nombre;
        private bool _activo;
        private int _numSubcategorias;
        private int _numAnuncios;

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
        }

        public bool Activo
        {
            get
            {
                return _activo;
            }
        }

        public int NumSubcategorias
        {
            get
            {
                return _numSubcategorias;
            }
        }

        public int NumAnuncios
        {
            get
            {
                return _numAnuncios;
            }
        }

        public CategoriaViewModel(int id, string nombre)
        {
            _id = id;
            _nombre = nombre;
        }

        public CategoriaViewModel(int id, string nombre, bool activo, int numSubcategorias, int numAnuncios)
        {
            _id = id;
            _nombre = nombre;
            _activo = activo;
            _numSubcategorias = numSubcategorias;
            _numAnuncios = numAnuncios;
        }
    }

    public class NuevaSubcategoriaViewModel
    {
        public List<SelectListItem> Categorias { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Categoria")]
        public int Categoria { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "NuevoNombre")]
        public string Nombre { get; set; }
    }

    public class BeneficiosViewModel
    {
        private ICollection<BeneficioViewModel> _beneficios;

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "ModificarPrecio")]
        public double Precio { get; set; }


        public BeneficiosViewModel(ICollection<BeneficioViewModel> beneficios)
        {
            _beneficios = beneficios;
        }

        public ICollection<BeneficioViewModel> Beneficios
        {
            get
            {
                return _beneficios;
            }
        }
    }

    public class BeneficioViewModel
    {
        private int _id;
        private string _descripcion;
        private double _precio;

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Descripcion
        {
            get
            {
                return _descripcion;
            }
        }

        public double Precio
        {
            get
            {
                return _precio;
            }
        }

        public BeneficioViewModel(int id, string descripcion, double precio)
        {
            _id = id;
            _descripcion = descripcion;
            _precio = precio;
        }
    }
    public enum ModificarNombreCategoriaEstatus
    {
        CategoriaExistente,
        Error,
        Exitoso
    }
}