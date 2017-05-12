using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace VendeAgroWeb.Models.Portal
{
    public class RegistroViewModel
    {
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La contraseña debe de ser mínimo de 6 caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y su confirmacion no son iguales.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Celular")]
        [StringLength(14, ErrorMessage = "El celular no es válido.", MinimumLength = 10)]
        public string Celular { get; set; }
    }

    public class LoginViewModel {

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La contraseña debe de ser mínimo de 6 caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

    }

    public class OlvidasteContrasenaViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }
    }

    public class CrearAnuncioViewModel
    {
        public CrearAnuncioViewModel() { }

        public int IdAnuncio { get; set; }

        public int IdUsuario { get; set; }

        public int _numFotos { get; set; }

        public bool _video { get; set; }
    }

    public class MisAnunciosViewModel
    {
        private ICollection<AnuncioViewModel> _anuncios;


        public ICollection<AnuncioViewModel> Anuncios
        {
            get
            {
                return _anuncios;
            }
        }

        public MisAnunciosViewModel(ICollection<AnuncioViewModel> anuncios) {
            _anuncios = anuncios;
        }
    }

    public class AnuncioViewModel
    {
        private string _titulo;
        private int _id;
        private EstadoAnuncio _estado;
        private int? _tiempoRestante;
        private string _imagenPrincipal;
        private PaqueteViewModel _paquete;
        private ICollection<BeneficioViewModel> _beneficios;

        public string Titulo
        {
            get
            {
                return _titulo;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public EstadoAnuncio Estado
        {
            get
            {
                return _estado;
            }
        }

        public int? TiempoRestante
        {
            get
            {
                return _tiempoRestante;
            }
        }

        public string ImagenPrincipal
        {
            get
            {
                return _imagenPrincipal;
            }
        }

        public PaqueteViewModel Paquete
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

        public AnuncioViewModel(int id, EstadoAnuncio estado, PaqueteViewModel paquete, ICollection<BeneficioViewModel> beneficios)
        {
            _id = id;
            _estado = estado;
            _paquete = paquete;
            _beneficios = beneficios;
        }

        public AnuncioViewModel(int id, string titulo, int estado, int? tiempoRestante, string imagenPrincipal, PaqueteViewModel paquete, ICollection<BeneficioViewModel> beneficios)
        {
            _id = id;
            _titulo = titulo;
            _estado = (EstadoAnuncio)estado;
            _tiempoRestante = tiempoRestante;
            _imagenPrincipal = imagenPrincipal;
            _paquete = paquete;
            _beneficios = beneficios;
        }
    }

    public class FotoViewModel
    {
        private int _id;
        private bool _principal;
        private string _ruta;

        public FotoViewModel(int id, bool principal, string ruta)
        {
            _id = id;
            _principal = principal;
            _ruta = ruta;
        }

        public FotoViewModel(bool principal, string ruta)
        {
            _principal = principal;
            _ruta = ruta;
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

        public int Id
        {
            get
            {
                return _id;
            }
        }
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

    public class AnuncioDetallesViewModel
    {
        private AnuncioViewModel _anuncio;
        private double? _precio;
        private string _descripcion;
        private string _categoria;
        private string _subcategoria;
        private string _pais;
        private string _estado;
        private ICollection<FotoViewModel> _fotos;
        private DateTime? _fechaInicio;
        private DateTime? _fechaFin;
        private string _video;
        private string _razonRechazo;

        public AnuncioDetallesViewModel(AnuncioViewModel anuncio, double? precio,
            string descripcion, string categoria, string subcategoria, string pais, string estado, 
            List<FotoViewModel> fotos, DateTime? fechaInicio, DateTime? fechaFin, string video, string razonRechazo)
        {
            _anuncio = anuncio;
            _precio = precio;
            _descripcion = descripcion;
            _categoria = categoria;
            _subcategoria = subcategoria;
            _pais = pais;
            _estado = estado;
            _fotos = fotos;
            _fechaInicio = fechaInicio;
            _fechaFin = fechaFin;
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

        public double? Precio
        {
            get
            {
                return _precio;
            }
        }

        public string RazonRechazo
        {
            get
            {
                return _razonRechazo;
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

        public string Pais
        {
            get
            {
                return _pais;
            }
        }

        public string Estado
        {
            get
            {
                return _estado;
            }
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
        private string _razonRechazo;


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

        public string RazonRechazo
        {
            get
            {
                return _razonRechazo;
            }
        }

        public ModificarAnuncioViewModel(int id, string titulo, string usuarioNombre, double? precio, CategoriaModificarAnuncioViewModel categoria, SubcategoriaModificarAnuncioViewModel subcategoria,
            PaisModificarAnuncioViewModel pais, EstadoModificarAnuncioViewModel estado, string descripcion, FotoViewModel fotoPrincipal, List<FotoViewModel> fotos, string video, string razonRechazo)
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
            _razonRechazo = razonRechazo;
        }
    }

    public class MisPagosViewModel {
        private ICollection<PagoViewModel> _pagos;

        public ICollection<PagoViewModel> Pagos
        {
            get
            {
                return _pagos;
            }
        }

        public MisPagosViewModel(ICollection<PagoViewModel> pagos) {
            _pagos = pagos;
        }
    }

    public class PagoViewModel
    {
        private int _id;
        private double _total;
        private DateTime _fecha;
        private int _digitosTarjeta;

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public double Total
        {
            get
            {
                return _total;
            }
        }

        public DateTime Fecha
        {
            get
            {
                return _fecha;
            }
        }

        public int DigitosTarjeta
        {
            get
            {
                return _digitosTarjeta;
            }
        }

        public PagoViewModel(int id, double total, DateTime fecha, int digitosTarjeta) {
            _id = id;
            _total = total;
            _fecha = fecha;
            _digitosTarjeta = digitosTarjeta;
        }
    }

    public class NuevaTarjetaViewModel
    {
        private int _id;

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public NuevaTarjetaViewModel(int id) {
            _id = id;
        }
    }

    public class PaqueteViewModel
    {
        private int _id;
        private string _nombre;
        private int _meses;

        

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

        public PaqueteViewModel(int id, string nombre, int meses)
        {
            _id = id;
            _nombre = nombre;
            _meses = meses;
        }
    }

    public class PerfilViewModel
    {
        private PortalUsuario _usuario;


        public PerfilViewModel(PortalUsuario usuario) {
            _usuario = usuario;
        }

        public PortalUsuario Usuario
        {
            get
            {
                return _usuario;
            }
        }
    }

    //[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    //public class CustomPhoneNumberAttribute : ValidationAttribute
    //{

    //    public CustomPhoneNumberAttribute()
    //    {

    //    }

    //    public override bool IsValid(object value)
    //    {
    //        string strValue = value as string;
    //        Debug.WriteLine("String value:" + value);
    //        if (!string.IsNullOrEmpty(strValue))
    //        {
    //            int len = strValue.Length;
    //            return len == 10;
    //        }
    //        return true;
    //    }
    //}
}