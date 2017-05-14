using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;
using VendeAgroWeb.Models;
using VendeAgroWeb.Models.Pagina;

namespace VendeAgroWeb
{
    [DataContract]
    public class CarritoDeCompra
    {
        public CarritoDeCompra() {
            Paquetes = new List<PaqueteCarrito>();
        }

        [DataMember]
        public List<PaqueteCarrito> Paquetes { get; private set; }

        public double TotalCarrito
        {
            get
            {
                return getTotalCarrito();
            }
        }

        public double getTotalCarrito() {
            double total = 0.0;
            foreach (var item in Paquetes) {
                total += item.Precio;
                total += item.TotalBeneficios;
            }
            return total;
        }

        public bool ActualizaRenovacionSiExiste(int idAnuncio, string nombreAnuncio, PaginaPaqueteViewModel paqueteNuevo, bool ofertaDelDia, out PaqueteCarrito outPaquete)
        {
            var paquete = Paquetes.Where(p => p.IdAnuncio == idAnuncio && string.Compare(nombreAnuncio, p.NombreAnuncio, StringComparison.CurrentCulture) == 0).FirstOrDefault();
            if(paquete == null)
            {
                outPaquete = null;
                return false;
            }

            if (!ofertaDelDia)
            {
                var copia = paquete.Beneficios.Where(b => b.Tipo == (int)BeneficiosExtraTipo.OfertaDelDia).FirstOrDefault();
                if (copia != null)
                {
                    paquete.borraBeneficioDePaquete(copia.Id);
                }
            }

            paquete.Nombre = paqueteNuevo.Nombre;
            paquete.Precio = paqueteNuevo.Precio;
            paquete.Id = paqueteNuevo.Id;
            paquete.Meses = paqueteNuevo.Meses;
            Paquetes[paquete.Index] = paquete;
            outPaquete = paquete;
            return true;
        }

        public PaqueteCarrito insertarPaqueteEnCarrito(int id, string nombre, int meses, double precio, int idAnuncio, string nombreAnuncio) {
            var paquete = new PaqueteCarrito(id, nombre, meses, precio, Paquetes.Count(), idAnuncio, nombreAnuncio);
            Paquetes.Add(paquete);
            return paquete;
        }

        public bool borraPaqueteDeCarrito(int index) {
            var paquete = Paquetes.ElementAt(index);
            if (paquete == null)
            {
                return false;
            }
            Paquetes.RemoveAt(index);
            UpdateIndexes();
            return true;
        }

        private void UpdateIndexes()
        {
            var i = 0;
            List<PaqueteCarrito> nuevaLista = new List<PaqueteCarrito>();
            foreach (var paquete in Paquetes)
            {
                paquete.Index = i;
                nuevaLista.Add(paquete);
                i++;
            }
            Paquetes = nuevaLista;
        }

        public void Clear()
        {
            Paquetes.Clear();
        }

    }

    [DataContract]
    public class PaqueteCarrito
    {
        [DataMember]
        public int Id { get; set; }

        [DataMember]
        public string Nombre { get; set; }

        [DataMember]
        public int Meses { get; set; }

        [DataMember]
        public double Precio { get; set; }

        [DataMember]
        public List<BeneficioCarrito> Beneficios { get; private set; }

        [DataMember]
        public double TotalBeneficios { get;  private set;}
        
        [DataMember]
        public int IdAnuncio { get; set; }

        [DataMember]
        public int Index { get; set; }

        [DataMember]
        public string NombreAnuncio { get; set; }

        public void agregaBeneficioAPaquete(BeneficioCarrito beneficio) {
            Beneficios.Add(beneficio);
            TotalBeneficios += beneficio.Precio;
        }

        public bool borraBeneficioDePaquete(int id)
        {
            var beneficio = Beneficios.Where(b=> b.Id == id).FirstOrDefault();
            if (beneficio == null)
            {
                return false;
            }
            else
            {
                Beneficios.Remove(beneficio);
                TotalBeneficios -= beneficio.Precio;
            }
            return true;
        }

        public bool EsRenovacion()
        {
            return IdAnuncio != -1;
        }

        public PaqueteCarrito(int id, string nombre, int meses, double precio, int index, int idAnuncio, string nombreAnuncio) {
            Id = id;
            Nombre = nombre;
            Meses = meses;
            Precio = precio;
            Beneficios = new List<BeneficioCarrito>();
            Index = index;
            IdAnuncio = idAnuncio;
            NombreAnuncio = nombreAnuncio;
        }
    }

    [DataContract]
    public class BeneficioCarrito
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public string Descripcion { get; private set; }

        [DataMember]
        public double Precio { get; private set; }

        [DataMember]
        public int Tipo { get; private set; }

        [DataMember]
        public int? Numero { get; private set; }

        public BeneficioCarrito(int id, string descripcion, double precio, int tipo, int? numero)
        {
            Id = id;
            Descripcion = descripcion;
            Precio = precio;
            Tipo = tipo;
            Numero = numero;
        }
    }
}
