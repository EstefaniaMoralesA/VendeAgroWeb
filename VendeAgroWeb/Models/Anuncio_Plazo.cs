//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace VendeAgroWeb.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class Anuncio_Plazo
    {
        public int id { get; set; }
        public int idAnuncio { get; set; }
        public int idPlazo { get; set; }
        public System.DateTime fechaInicio { get; set; }
        public System.DateTime fechaFin { get; set; }
        public bool activo { get; set; }
    
        public virtual Anuncio Anuncio { get; set; }
        public virtual Plazo Plazo { get; set; }
    }
}
