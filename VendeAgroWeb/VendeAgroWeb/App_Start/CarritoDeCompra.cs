using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Web.Http;

namespace VendeAgroWeb
{
    [DataContract]
    public class CarritoDeCompra
    {
        public CarritoDeCompra() {
            if (Paquetes == null)
            {
                Paquetes = new List<PaqueteCarrito>();
            }
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

        public PaqueteCarrito insertarPaqueteEnCarrito(int id, string nombre, int meses, double precio) {
            var paquete = new PaqueteCarrito(id, nombre, meses, precio);
            Paquetes.Add(paquete);
            return paquete;
        }

        public bool borraPaqueteDeCarrito(int index) {
            var paquete = Paquetes.ElementAt(index);
            if (paquete == null)
            {
                return false;
            }
            else {
                Paquetes.Remove(paquete);
            }
            return true;
        }

    }

    [DataContract]
    public class PaqueteCarrito
    {
        [DataMember]
        public int Id { get; private set; }

        [DataMember]
        public string Nombre { get; private set; }

        [DataMember]
        public int Meses { get; private set; }

        [DataMember]
        public double Precio { get; private set; }

        [DataMember]
        public List<BeneficioCarrito> Beneficios { get; private set; }

        [DataMember]
        public double TotalBeneficios { get;  private set;}

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

        public PaqueteCarrito(int id, string nombre, int meses, double precio) {
            Id = id;
            Nombre = nombre;
            Meses = meses;
            Precio = precio;
            Beneficios = new List<BeneficioCarrito>();

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
