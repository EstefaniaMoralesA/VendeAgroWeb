using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

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
}