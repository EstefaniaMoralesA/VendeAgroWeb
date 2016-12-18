using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace VendeAgroWeb
{
    public class CarritoDeCompra
    {
        private double _totalCarrito;
        
        public CarritoDeCompra() {
            if (Paquetes == null)
            {
                Paquetes = new List<PaqueteCarrito>();
            }
        }

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

    public class PaqueteCarrito
    {
        private int _id;
        private string _nombre;
        private int _meses;
        private double _precio;

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

        public List<BeneficioCarrito> Beneficios { get; private set; }

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
            _id = id;
            _nombre = nombre;
            _meses = meses;
            _precio = precio;
            Beneficios = new List<BeneficioCarrito>();

        }
    }

    public class BeneficioCarrito
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

        public BeneficioCarrito(int id, string descripcion, double precio, int tipo, int? numero)
        {
            _id = id;
            _descripcion = descripcion;
            _precio = precio;
            _tipo = tipo;
            _numero = numero;
        }
    }
}
