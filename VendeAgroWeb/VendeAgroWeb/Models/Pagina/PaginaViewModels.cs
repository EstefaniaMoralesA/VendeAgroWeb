using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace VendeAgroWeb.Models.Pagina
{
    public class HomeViewModel
    {
        public IList<Anuncio> AnunciosDestacados { get; set; }

        public HomeViewModel()
        {
        }
    }

    public class BusquedaViewModel
    {
        public IList<Anuncio> FiltroTextoAnuncios { get; set; }
    }

    public class OfertasDelDiaViewModel
    {
    }

    public class AnunciateViewModel
    {
        private ICollection<PaginaPaqueteViewModel> _paquetes;
        private List<int> _paquetesSeleccionados;


        public ICollection<PaginaPaqueteViewModel> Paquetes
        {
            get
            {
                return _paquetes;
            }
        }

        public int IdAnuncio { get; set; }

        public List<int> PaquetesSeleccionados
        {
            get
            {
                return _paquetesSeleccionados;
            }

            set
            {
                _paquetesSeleccionados = value;
            }
        }

        public AnunciateViewModel(ICollection<PaginaPaqueteViewModel> paquetes)
        {
            _paquetes = paquetes;
        }
    }

    public class BeneficiosExtraViewModel
    {
        private double _totalCarrito;
        private PaqueteCarrito _paquete;
        private ICollection<PaginaBeneficioViewModel> _beneficios;

        public ICollection<PaginaBeneficioViewModel> Beneficios
        {
            get
            {
                return _beneficios;
            }
        }

        public PaqueteCarrito Paquete
        {
            get
            {
                return _paquete;
            }
        }

        public double TotalCarrito
        {
            get
            {
                return _totalCarrito;
            }
        }

        public BeneficiosExtraViewModel(PaqueteCarrito paquete, ICollection<PaginaBeneficioViewModel> beneficios, double totalCarrito)
        {
            _paquete = paquete;
            _beneficios = beneficios;
            _totalCarrito = totalCarrito;
        }
    }

    public class ContactoViewModel
    {
        [Required(ErrorMessage = "Campo requerido")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [EmailAddress(ErrorMessage = "Introduzca un email válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El número de caracteres no puede ser mayor a 200")]
        public string Mensaje { get; set; }
    }

    public class CarritoDeCompraViewModel
    {

    }

    public class PaginaFotoViewModel
    {
        private bool _principal;
        private string _ruta;

        public PaginaFotoViewModel(bool principal, string ruta)
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

    public class PaginaOwnerAnuncioViewModel
    {
        private int _idUsuario;
        private string _nombreUsuario;
        private string _telefonoContacto;
        private string _emailContacto;

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

        public string TelefonoContacto
        {
            get
            {
                return _telefonoContacto;
            }
        }

        public string EmailContacto
        {
            get
            {
                return _emailContacto;
            }
        }

        public PaginaOwnerAnuncioViewModel(int idUsuario, string nombreUsuario, string telefonoContacto, string emailContacto)
        {
            _idUsuario = idUsuario;
            _nombreUsuario = nombreUsuario;
            _telefonoContacto = telefonoContacto;
            _emailContacto = emailContacto;
        }
    }

    public class QueryViewModel
    {
        private string _query;

        public string Query
        {
            get
            {
                return _query;
            }
        }

        public QueryViewModel(string query) {
            _query = query;
        }
    }

    public class GaleriaAnuncioViewModel {
        private ICollection<PaginaFotoViewModel> _fotos;

        public ICollection<PaginaFotoViewModel> Fotos
        {
            get
            {
                return _fotos;
            }
        }

        public GaleriaAnuncioViewModel(ICollection<PaginaFotoViewModel> fotos)
        {
            _fotos = fotos;
        }

    }

    public class PortalDetallesAnuncioViewModel
    {
        private PortalAnuncioViewModel _anuncio;
        private string _descripcion;
        private ICollection<PaginaFotoViewModel> _fotos;
        private string _video;
        private PaginaOwnerAnuncioViewModel _owner;
        private int? _clicks;
        private ConsultarDetalles _consulta;
        private string _query;

        public PortalDetallesAnuncioViewModel(PortalAnuncioViewModel anuncio, string descripcion, 
            List<PaginaFotoViewModel> fotos, string video, PaginaOwnerAnuncioViewModel owner, int? clicks, ConsultarDetalles consulta, string query)
        {
            _anuncio = anuncio;
            _descripcion = descripcion;
            _fotos = fotos;
            _video = video;
            _owner = owner;
            _clicks = clicks;
            _consulta = consulta;
            _query = query;
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

        public PortalAnuncioViewModel Anuncio
        {
            get
            {
                return _anuncio;
            }
        }

        public ICollection<PaginaFotoViewModel> Fotos
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

        public PaginaOwnerAnuncioViewModel Owner
        {
            get
            {
                return _owner;
            }
        }

        public int? Clicks
        {
            get
            {
                return _clicks;
            }
        }

        public ConsultarDetalles Consulta
        {
            get
            {
                return _consulta;
            }
        }

        public string Query
        {
            get
            {
                return _query;
            }
        }
    }

    public class PortalAnuncioViewModel
    {
        private int _id;
        private EstadoAnuncio _estadoAnuncio;
        private string _titulo;
        private double? _precio;
        private string _categoria;
        private string _subcategoria;
        private string _estado;
        private string _fotoPrincipal;

        public string Titulo
        {
            get
            {
                return _titulo;
            }
        }

        public string FotoPrincipal
        {
            get
            {
                return _fotoPrincipal;
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

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public EstadoAnuncio EstadoAnuncio
        {
            get
            {
                return _estadoAnuncio;
            }
        }

        public PortalAnuncioViewModel(int id, string titulo, double? precio, string categoria, string subcategoria, string estado, string fotoPrincipal)
        {
            _id = id;
            _titulo = titulo;
            _precio = precio;
            _categoria = categoria;
            _subcategoria = subcategoria;
            _estado = estado;
            _fotoPrincipal = fotoPrincipal;
        }
    }

    public class PaginaBeneficioViewModel
    {
        private int _id;
        private string _descripcion;
        private double _precio;
        private int _tipo;
        private int? _numero;

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

        public int Tipo
        {
            get
            {
                return _tipo;
            }
        }

        public int? Numero
        {
            get
            {
                return _numero;
            }
        }

        public PaginaBeneficioViewModel(int id, string descripcion, double precio, int tipo, int? numero)
        {
            _id = id;
            _descripcion = descripcion;
            _precio = precio;
            _tipo = tipo;
            _numero = numero;
        }
    }

    public class PaginaPaisesViewModel
    {
        private ICollection<PaginaPaisViewModel> _paises;

        public ICollection<PaginaPaisViewModel> Paises
        {
            get
            {
                return _paises;
            }
        }

        public PaginaPaisesViewModel(ICollection<PaginaPaisViewModel> paises)
        {
            _paises = paises;
        }

    }

    public class PaginaPaisViewModel
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

        public PaginaPaisViewModel(string nombre)
        {
            _id = -1;
            _nombre = nombre;
        }

        public PaginaPaisViewModel(int id, string nombre)
        {
            _id = id;
            _nombre = nombre;
        }
    }

    public class PaginaEstadosViewModel
    {
        private ICollection<PaginaEstadoViewModel> _estados;

        public ICollection<PaginaEstadoViewModel> Estados
        {
            get
            {
                return _estados;
            }
        }

        public PaginaEstadosViewModel(ICollection<PaginaEstadoViewModel> estados)
        {
            _estados = estados;
        }

    }

    public class PaginaEstadoViewModel
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

        public PaginaEstadoViewModel(string nombre)
        {
            _id = -1;
            _nombre = nombre;
        }

        public PaginaEstadoViewModel(int id, string nombre)
        {
            _id = id;
            _nombre = nombre;
        }
    }

    public class PaginaPaqueteViewModel
    {
        private int _id;
        private string _nombre;
        private int _meses;
        private double _precio;
        private string _descripcion;
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

        public double Ahorro
        {
            get
            {
                return _ahorro;
            }
        }

        public PaginaPaqueteViewModel(int id, string nombre, int meses, double precio, string descripcion, double ahorro)
        {
            _id = id;
            _nombre = nombre;
            _meses = meses;
            _precio = precio;
            _descripcion = descripcion;
            _ahorro = ahorro;
        }
    }

    public class PaginaCategoriasViewModel
    {
        private ICollection<PaginaCategoriaViewModel> _categorias;

        public ICollection<PaginaCategoriaViewModel> Categorias
        {
            get
            {
                return _categorias;
            }
        }

        public PaginaCategoriasViewModel(ICollection<PaginaCategoriaViewModel> categorias)
        {
            _categorias = categorias;
        }
    }

    public class PaginaCategoriaViewModel
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

        public PaginaCategoriaViewModel(string nombre)
        {
            _id = -1;
            _nombre = nombre;
        }

        public PaginaCategoriaViewModel(int id, string nombre)
        {
            _id = id;
            _nombre = nombre;
        }
    }

    public class PaginaSubcategoriasViewModel
    {
        private ICollection<PaginaSubcategoriaViewModel> _subcategorias;

        public ICollection<PaginaSubcategoriaViewModel> Subcategorias
        {
            get
            {
                return _subcategorias;
            }
        }

        public PaginaSubcategoriasViewModel(ICollection<PaginaSubcategoriaViewModel> subcategorias)
        {
            _subcategorias = subcategorias;
        }
    }

    public class PaginaSubcategoriaViewModel
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

        public PaginaSubcategoriaViewModel(string nombre)
        {
            _id = -1;
            _nombre = nombre;
        }

        public PaginaSubcategoriaViewModel(int id, string nombre)
        {
            _id = id;
            _nombre = nombre;
        }
    }

    public class PortalAnunciosBusquedaViewModel
    {
        private ICollection<PortalAnuncioViewModel> _anuncios;
        private string _query;

        public PortalAnunciosBusquedaViewModel(ICollection<PortalAnuncioViewModel> anuncios, string query)
        {
            _anuncios = anuncios;
            _query = query;
        }

        public ICollection<PortalAnuncioViewModel> Anuncios
        {
            get
            {
                return _anuncios;
            }
        }

        public string Query
        {
            get
            {
                return _query;
            }
        }
    }

    public class PortalAnunciosViewModel
    {
        private ICollection<PortalAnuncioViewModel> _anuncios;
        private string _nombreCategoria;
        private string _nombreSubcategoria;
        private string _nombreUsuario;

        public PortalAnunciosViewModel(ICollection<PortalAnuncioViewModel> anuncios, string nombreCategoria, string nombreSubcategoria, string nombreUsuario)
        {
            _anuncios = anuncios;
            _nombreCategoria = nombreCategoria;
            _nombreSubcategoria = nombreSubcategoria;
            _nombreUsuario = nombreUsuario;
        }

        public PortalAnunciosViewModel(ICollection<PortalAnuncioViewModel> anuncios, string nombreCategoria, string nombreSubcategoria, string nombreUsuario, int total, int index)
        {
            _anuncios = anuncios;
            _nombreCategoria = nombreCategoria;
            _nombreSubcategoria = nombreSubcategoria;
            _nombreUsuario = nombreUsuario;
            Total = total;
            Index = index;
        }

        public int Total { get; private set; }

        public int Index { get; private set; }

        public ICollection<PortalAnuncioViewModel> Anuncios
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
    }

    public class TarjetaViewModel
    {
        public TarjetaViewModel(int id, TarjetaTipo tipo, string digitos, string idConekta)
        {
            Id = id;
            Tipo = tipo;
            Digitos = digitos;
            IdConekta = idConekta;
        }

        public int Id { get; private set; }
        public TarjetaTipo Tipo { get; private set; }
        public string Digitos { get; private set; }
        public string IdConekta { get; private set; }
    }

    public class PaginaBannersLateralesViewModel
    {
        private ICollection<PaginaBannerCentralViewModel> _banners;

        public ICollection<PaginaBannerCentralViewModel> Banners
        {
            get
            {
                return _banners;
            }
        }

        public PaginaBannersLateralesViewModel(ICollection<PaginaBannerCentralViewModel> banners)
        {
            _banners = banners;
        }

    }

    public class PaginaBannerCentralViewModel
    {
        private int _id;
        private string _ruta;
        private string _link;

        public string Ruta
        {
            get
            {
                return _ruta;
            }
        }

        public string Link
        {
            get
            {
                return _link;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public PaginaBannerCentralViewModel(int id, string ruta, string link) {
            _id = id;
            _ruta = ruta;
            _link = link;
        }
    }

    public class PagoCarritoTarjetasViewModel
    {
        private PortalUsuario _usuario;


        public PagoCarritoTarjetasViewModel(PortalUsuario usuario, double total)
        {
            _usuario = usuario;
            Total = total;
        }

        public PortalUsuario Usuario
        {
            get
            {
                return _usuario;
            }
        }

        public double Total { get; private set; }
    }


    public enum BeneficiosExtraTipo
    {
        Fotos = 1,
        Video = 2,
        OfertaDelDia = 3
    }

    public enum TarjetaTipo
    {
        MasterCard = 1,
        Visa = 2,
        Amex = 3
    }
}