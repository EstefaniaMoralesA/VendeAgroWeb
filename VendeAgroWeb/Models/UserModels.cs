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
        private VendeAgroEntities _dbContext;

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


}