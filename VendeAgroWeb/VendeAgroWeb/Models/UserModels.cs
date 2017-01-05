using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using VendeAgroWeb.Models.Pagina;

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
        private string _idConekta;

        public PortalUsuario(int id, string email, string nombre, string apellidos, string telefono, string idConekta)
        {
            _id = id;
            _email = email;
            _nombre = nombre;
            _apellidos = apellidos;
            _telefono = telefono;
            _idConekta = idConekta;
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

        public string IdConekta => _idConekta;

        private ICollection<TarjetaViewModel> Tarjetas
        {
            get
            {
                using (var _dbContext = new MercampoEntities())
                {
                    Startup.OpenDatabaseConnection(_dbContext);
                    if (_dbContext.Database.Connection.State != System.Data.ConnectionState.Open)
                    {
                        return new List<TarjetaViewModel>();
                    }

                    var tarjetas = _dbContext.Usuario_Tarjeta.Where(t => t.idUsuario == _id && t.activo == true);

                    List<TarjetaViewModel> tarjetasFinal = new List<TarjetaViewModel>();
                    foreach (var tarjeta in tarjetas)
                    {
                        tarjetasFinal.Add(new TarjetaViewModel(tarjeta.id, (TarjetaTipo)tarjeta.tipoTarjeta, tarjeta.digitosTarjeta, tarjeta.tokenTarjeta));
                    }

                    _dbContext.Database.Connection.Close();

                    return tarjetasFinal;
                }
            }
        }
    }


}