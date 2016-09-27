using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace VendeAgroWeb.Models.Administrador
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Introduzca un email válido")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }
    }

    public class OlvidoContrasenaViewModel
    {
        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Email")]
        [EmailAddress(ErrorMessage = "Introduzca un email válido")]
        public string Email { get; set; }
    }

    public class CambiarContrasenaViewModel
    {
        [Required(ErrorMessage = "Campo requerido")]
        [StringLength(20, ErrorMessage = "La contraseña debe contener como mínimo 6 caracteres", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "La contraseña y su confirmacion no son iguales.")]
        [Display(Name = "ConfirmaContraseña")]
        public string ConfirmaPassword { get; set; }

        public string Token { get; set; }
    }

    public class UsuarioAdministradorViewModel
    {
        private int _id;
        private string _nombre;
        private string _email;
        private bool _activo;

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
                return _email;
            }

        }

        public bool Activo
        {
            get
            {
                return _activo;
            }
        }

        public UsuarioAdministradorViewModel(int id, string nombre, string email, bool activo)
        {
            _id = id;
            _nombre = nombre;
            _email = email;
            _activo = activo;
        }
    }

    public class UsuarioPortalViewModel
    {
        private int _id;
        private string _nombre;
        private string _apellidos;
        private string _telefono;
        private string _email;

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

        public string Email
        {
            get
            {
                return _email;
            }
        }

        public UsuarioPortalViewModel(int id, string nombre, string apellidos, string telefono, string email)
        {
            _id = id;
            _nombre = nombre;
            _apellidos = apellidos;
            _telefono = telefono;
            _email = email;
        }
    }


    public class UsuariosViewModel
    {

        private int _tipo;
        private ICollection<UsuarioAdministradorViewModel> _usuariosAdministrador;
        private ICollection<UsuarioPortalViewModel> _usuariosPortal;

        public UsuariosViewModel(int tipo, ICollection<UsuarioAdministradorViewModel> usuariosAdmin, ICollection<UsuarioPortalViewModel> usuariosPortal)
        {
            _tipo = tipo;
            _usuariosAdministrador = usuariosAdmin;
            _usuariosPortal = usuariosPortal;
        }

        public int Tipo
        {
            get
            {
                return _tipo;
            }
        }

        public ICollection<UsuarioAdministradorViewModel> UsuariosAdministrador
        {
            get
            {
                return _usuariosAdministrador;
            }
        }

        public ICollection<UsuarioPortalViewModel> UsuariosPortal
        {
            get
            {
                return _usuariosPortal;
            }
        }
    }

    public class AnunciosAprobadosViewModel {
    }

    public class AnunciosPendientesViewModel {
    }

    public class AnunciosViewModel {
        private ICollection<AnunciosAprobadosViewModel> _anunciosActivos;
        private ICollection<AnunciosAprobadosViewModel> _anunciosVencidos;
        private ICollection<AnunciosPendientesViewModel> _anunciosPendientes;

        public AnunciosViewModel(ICollection<AnunciosAprobadosViewModel> anunciosActivos, ICollection<AnunciosAprobadosViewModel> anunciosVencidos, ICollection<AnunciosPendientesViewModel> anunciosPendientes)
        {
            _anunciosActivos = anunciosActivos;
            _anunciosVencidos = anunciosVencidos;
            _anunciosPendientes = anunciosPendientes;
        }

        public ICollection<AnunciosAprobadosViewModel> AnunciosActivos
        {
            get
            {
                return _anunciosActivos;
            }
        }

        public ICollection<AnunciosAprobadosViewModel> AnunciosVencidos
        {
            get
            {
                return _anunciosVencidos;
            }
        }

        public ICollection<AnunciosPendientesViewModel> AnunciosPendientes
        {
            get
            {
                return _anunciosPendientes;
            }
        }
    }

    public class SubcategoriasViewModel {
        private ICollection<SubcategoriaViewModel> _subcategorias;
        private int _id;

        public SubcategoriasViewModel(ICollection<SubcategoriaViewModel> subcategorias) {
            _subcategorias = subcategorias;
        }  

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "ModificarNombre")]
        public string Titulo { get; set; }

        public ICollection<SubcategoriaViewModel> Subcategorias
        {
            get
            {
                return _subcategorias;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }
    }

    public class SubcategoriaViewModel {
        private int _id;
        private string _nombre;
        private bool _activo;
        private string _nombreCategoria;

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

        public string NombreCategoria
        {
            get
            {
                return _nombreCategoria;
            }
        }

        public bool Activo
        {
            get
            {
                return _activo;
            }
        }

        public SubcategoriaViewModel(int id, string nombre, bool activo, string nombreCategoria) {
            _id = id;
            _nombre = nombre;
            _activo = activo;
            _nombreCategoria = nombreCategoria;
        }
    }

    public class NuevaCategoriaViewModel {
        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "NuevoNombre")]
        public string Nombre { get; set; }
    }

    public class CategoriasViewModel
    {
        private ICollection<CategoriaViewModel> _categorias;
        private int _id;

        public CategoriasViewModel(ICollection<CategoriaViewModel> categorias)
        {
            _categorias = categorias;
        }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "ModificarNombre")]
        public string Titulo { get; set; }

        public ICollection<CategoriaViewModel> Categorias
        {
            get
            {
                return _categorias;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }
    }

    public class CategoriaViewModel
    {
        private int _id;
        private string _nombre;
        private bool _activo;
        private int _numSubcategorias;
        private int _numAnuncios;

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

        public bool Activo
        {
            get
            {
                return _activo;
            }
        }

        public int NumSubcategorias
        {
            get
            {
                return _numSubcategorias;
            }
        }

        public int NumAnuncios
        {
            get
            {
                return _numAnuncios;
            }
        }

        public CategoriaViewModel(int id, string nombre, bool activo, int numSubcategorias)
        {
            _id = id;
            _nombre = nombre;
            _activo = activo;
            _numSubcategorias = numSubcategorias;
        }
    }

    public class NuevaSubcategoriaViewModel
    {
        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "NuevoNombre")]
        public string Nombre { get; set; }
    }

    public class BeneficiosViewModel
    {
    }
}