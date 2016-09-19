using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VendeAgroWeb.Models
{
    public class AdministradorUsuario
    {
        private int _id;
        private string _email;
        private string _nombre;

        public AdministradorUsuario(int id, string email, string nombre){
            _id = id;
            _email = email;
            _nombre = nombre;
        }

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

        public string Email
        {
            get
            {
                return Email;
            }
        }
    }

    public class PortalUsuario
    {
        private int _id;
        private string _nombre;
        private string _email;
        private string _apellidos;
        private string _telefono;

        public PortalUsuario(int id, string email, string nombre, string apellidos, string telefono)
        {
            _id = id;
            _email = email;
            _nombre = nombre;
            _apellidos = apellidos;
            _telefono = telefono;
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public string Email
        {
            get
            {
                return _email;
            }
        }

        public string Nombre
        {
            get
            {
                return _nombre;
            }
        }

        public string Apellidos
        {
            get
            {
                return _apellidos;
            }
        }

        public string Telefono
        {
            get
            {
                return _telefono;
            }
        }
    }


}