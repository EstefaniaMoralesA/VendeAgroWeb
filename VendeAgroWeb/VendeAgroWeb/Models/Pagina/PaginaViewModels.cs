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

        public AnunciateViewModel(ICollection<PaginaPaqueteViewModel> paquetes) {
            _paquetes = paquetes;
        }
    }

    public class BeneficiosExtraViewModel {
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

        public BeneficiosExtraViewModel(PaqueteCarrito paquete, ICollection<PaginaBeneficioViewModel> beneficios, double totalCarrito) {
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

    public class PortalAnuncioViewModel
    {
        private int _id;
        private string _titulo;
        private string _usuario;
        private double? _precio;
        private string _categoria;
        private string _subcategoria;
        private string _estado;
        private string _ciudad;
        private string _fotoPrincipal;
        private int? _clicks;

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

        public string Ciudad
        {
            get
            {
                return _ciudad;
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

        public PortalAnuncioViewModel(int id, string titulo, string usuario, double? precio, string categoria, string subcategoria, string estado, string ciudad, int? clicks, string fotoPrincipal)
        {
            _id = id;
            _titulo = titulo;
            _usuario = usuario;
            _precio = precio;
            _categoria = categoria;
            _subcategoria = subcategoria;
            _estado = estado;
            _ciudad = ciudad;
            _clicks = clicks ?? 0;
            _fotoPrincipal = fotoPrincipal;
        }
    }

    public class PaginaBeneficioViewModel {
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

        public PaginaBeneficioViewModel(int id, string descripcion, double precio, int tipo, int? numero) {
            _id = id;
            _descripcion = descripcion;
            _precio = precio;
            _tipo = tipo;
            _numero = numero;
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

    public class PaginaCategoriasViewModel {
        private ICollection<PaginaCategoriaViewModel> _categorias;

        public ICollection<PaginaCategoriaViewModel> Categorias
        {
            get
            {
                return _categorias;
            }
        }

        public PaginaCategoriasViewModel(ICollection<PaginaCategoriaViewModel> categorias) {
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

        public PaginaCategoriaViewModel(int id, string nombre)
        {
            _id = id;
            _nombre = nombre;
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

        public PaginaSubcategoriaViewModel(int id, string nombre)
        {
            _id = id;
            _nombre = nombre;
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
        public TarjetaViewModel(int id, TarjetaTipo tipo, int digitos, string idConekta)
        {
            Id = id;
            Tipo = tipo;
            Digitos = digitos;
            IdConekta = idConekta;
        }

        public int Id { get; private set; }
        public TarjetaTipo Tipo { get; private set; }
        public int Digitos { get; private set; }
        public string IdConekta { get; private set; }
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