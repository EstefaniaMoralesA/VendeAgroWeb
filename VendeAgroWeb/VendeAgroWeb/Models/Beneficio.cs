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
    
    public partial class Beneficio
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Beneficio()
        {
            this.Anuncio_Beneficio = new HashSet<Anuncio_Beneficio>();
        }
    
        public int id { get; set; }
        public string descripcion { get; set; }
        public double precio { get; set; }
        public int tipo { get; set; }
        public Nullable<int> numero { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Anuncio_Beneficio> Anuncio_Beneficio { get; set; }
    }
}
