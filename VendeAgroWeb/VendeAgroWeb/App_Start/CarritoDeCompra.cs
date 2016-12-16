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
        public ICollection<PaqueteCarrito> Paquetes { get; private set; }

        public double TotalCarrito{ get; set; }


        public void insertarPaqueteEnCarrito(int id, string nombre, int meses, double precio) {
            Paquetes.Add(new PaqueteCarrito(id, nombre, meses, precio));
            TotalCarrito += precio;
        }

    }

    public class PaqueteCarrito
    {
        private int _id;
        private string _nombre;
        private int _meses;
        private double _precio;
        private int _totalBeneficios;
        private ICollection<BeneficioCarrito> _beneficios;

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

        public ICollection<BeneficioCarrito> Beneficios
        {
            get
            {
                return _beneficios;
            }
        }

        public int TotalBeneficios
        {
            get
            {
                return _totalBeneficios;
            }
        }

        public PaqueteCarrito(int id, string nombre, int meses, double precio) {
            _id = id;
            _nombre = nombre;
            _meses = meses;
            _precio = precio;
        }

        public PaqueteCarrito(int id, string nombre, int meses, double precio, ICollection<BeneficioCarrito> beneficios)
        {
            _id = id;
            _nombre = nombre;
            _meses = meses;
            _precio = precio;
            _beneficios = beneficios;
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
