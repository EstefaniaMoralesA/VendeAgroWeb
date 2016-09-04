using System;
using System.Collections.Generic;
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
    }
}