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
        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Titulo")]
        public string Titulo { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(250, MinimumLength = 3, ErrorMessage = "La descripción no puede ser mayor de 250 caracteres.")]
        [Display(Name = "Descripcion")]
        public string Descripcion { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Precio")]
        public double Precio { get; set; }
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
        public string Titulo { get; set; }

        public int Id { get; set; }

        public EstadoAnuncio Estado { get; set; }

        public int TiempoRestante { get; set; }
        public string ImagenPrincipal { get; set; }

        public AnuncioViewModel(int id, string titulo, int estado, int tiempoRestante, string imagenPrincipal)
        {
            Id = id;
            Titulo = titulo;
            Estado = (EstadoAnuncio)estado;
            TiempoRestante = tiempoRestante;
            ImagenPrincipal = imagenPrincipal;
        }
    }

    public class FotoViewModel
    {
        private bool _principal;
        private string _ruta;

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
        private ICollection<FotoViewModel> _fotos;
        private DateTime? _fechaInicio;
        private DateTime? _fechaFin;
        private AnuncioPaqueteViewModel _paquete;
        private ICollection<BeneficioViewModel> _beneficios;
        private string _video;

        public AnuncioDetallesViewModel(AnuncioViewModel anuncio, double? precio,
            string descripcion, List<FotoViewModel> fotos, DateTime? fechaInicio,
            DateTime? fechaFin, AnuncioPaqueteViewModel paquete, ICollection<BeneficioViewModel> beneficios, string video)
        {
            _anuncio = anuncio;
            _precio = precio;
            _descripcion = descripcion;
            _fotos = fotos;
            _paquete = paquete;
            _fechaInicio = fechaInicio;
            _fechaFin = fechaFin;
            _beneficios = beneficios;
            _video = video;
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

        public double? Precio
        {
            get
            {
                return _precio;
            }
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