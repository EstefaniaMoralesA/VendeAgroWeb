using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using VendeAgroWeb.Models.Administrador;

namespace VendeAgroWeb.Models.Pagina
{
    public class HomeViewModel
    {
        public IList<Categoria> Categorias { get; set; }
        public IList<Subcategoria> Subcategorias { get; set; }
        public IList<Anuncio> AnunciosDestacados { get; set; }
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
        private double _precio;
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

        public double Precio
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

        public PortalAnuncioViewModel(int id, string titulo, string usuario, double precio, string categoria, string subcategoria, string estado, string ciudad, int? clicks, string fotoPrincipal)
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
}