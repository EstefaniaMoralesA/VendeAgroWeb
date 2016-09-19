using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace VendeAgroWeb.Models.Administrador
{
    public class AdministracionUsuarios
    {
        private int id;
        private string email;
        private string nombre;

        public string Email
        {
            get
            {
                return email;
            }
        }

        public string Nombre
        {
            get
            {
                return nombre;
            }
        }
    }
}