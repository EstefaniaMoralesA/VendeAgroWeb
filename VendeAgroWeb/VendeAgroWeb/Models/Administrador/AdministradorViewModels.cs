﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
        [System.ComponentModel.DataAnnotations.Compare("Password", ErrorMessage = "La contraseña y su confirmacion no son iguales.")]
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

        public int NumAnuncios
        {
            get
            {
                return _numAnuncios;
            }
        }

        public UsuarioPortalViewModel(int id, string nombre, string apellidos, string telefono, string email, int numAnuncios)
        {
            _id = id;
            _nombre = nombre;
            _apellidos = apellidos;
            _telefono = telefono;
            _email = email;
            _numAnuncios = numAnuncios;
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

    public class NuevoPaqueteViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "NuevoNombre")]
        public string Nombre { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Meses")]
        [Range(0, int.MaxValue, ErrorMessage = "Introduzca un valor numérico")]
        public int Meses { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Anuncios permitidos")]
        [Range(0, int.MaxValue, ErrorMessage = "Introduzca un valor numérico")]
        public int Anuncios { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Precio del paquete")]
        [Range(0, float.MaxValue, ErrorMessage = "Introduzca un valor numérico")]
        public double Precio { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Descripción del paquete")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "El número de caracteres no puede ser mayor a 200")]
        public string Descripcion { get; set; }

    }

    public class PaqueteViewModel {
        private int _id;
        private string _nombre;
        private int _meses;
        private int _numAnuncios;
        private double _precio;
        private string _descripcion;
        private bool _paqueteBase;
        private DateTime _fechaModificacion;
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

        public int Meses
        {
            get
            {
                return _meses;
            }
        }

        public int NumAnuncios
        {
            get
            {
                return _numAnuncios;
            }
        }

        public double Precio
        {
            get
            {
                return _precio;
            }
        }

        public string Descripcion
        {
            get
            {
                return _descripcion;
            }
        }

        public bool PaqueteBase
        {
            get
            {
                return _paqueteBase;
            }
        }

        public DateTime FechaModificacion
        {
            get
            {
                return _fechaModificacion;
            }
        }

        public bool Activo
        {
            get
            {
                return _activo;
            }
        }

        public PaqueteViewModel(int id, string nombre, int meses, int numAnuncios, double precio, string descripcion, bool paqueteBase, DateTime fechaModificacion, bool activo) {
            _id = id;
            _nombre = nombre;
            _meses = meses;
            _numAnuncios = numAnuncios;
            _precio = precio;
            _descripcion = descripcion;
            _paqueteBase = paqueteBase;
            _fechaModificacion = fechaModificacion;
            _activo = activo;
        }
    }

    public class PaquetesViewModel {
        private ICollection<PaqueteViewModel> _paquetes;

        public PaquetesViewModel(ICollection<PaqueteViewModel> paquetes) {
            _paquetes = paquetes;
        }

        public ICollection<PaqueteViewModel> Paquetes
        {
            get
            {
                return _paquetes;
            }
        }
    }

    public class AnuncioViewModel {
        private int _id;
        private string _titulo;
        private string _usuario;
        private double _precio;
        private string _categoria;
        private string _subcategoria;
        private string _estado;
        private string _ciudad;
        private int? _clicks;

        public string Titulo
        {
            get
            {
                return _titulo;
            }
        }

        public double Precio
        {
            get
            {
                return _precio;
            }
        }

        public string Categoria
        {
            get
            {
                return _categoria;
            }
        }

        public string Subcategoria
        {
            get
            {
                return _subcategoria;
            }
        }

        public string Estado
        {
            get
            {
                return _estado;
            }
        }

        public string Ciudad
        {
            get
            {
                return _ciudad;
            }
        }

        public int? Clicks
        {
            get
            {
                return _clicks;
            }
        }

        public string Usuario
        {
            get
            {
                return _usuario;
            }
        }

        public int Id
        {
            get
            {
                return _id;
            }
        }

        public AnuncioViewModel(int id, string titulo, string usuario,  double precio, string categoria, string subcategoria, string estado, string ciudad, int? clicks) {
            _id = id;
            _titulo = titulo;
            _usuario = usuario;
            _precio = precio;
            _categoria = categoria;
            _subcategoria = subcategoria;
            _estado = estado;
            _ciudad = ciudad;
            _clicks = clicks;
        }
    }

    public class AnunciosPendientesViewModel {
    }

    public class AnunciosViewModel {
        private ICollection<AnuncioViewModel> _anuncios;
        private string _nombreCategoria;
        private string _nombreSubcategoria;
        private string _nombreUsuario;

        public AnunciosViewModel(ICollection<AnuncioViewModel> anuncios, string nombreCategoria, string nombreSubcategoria, string nombreUsuario)
        {
            _anuncios = anuncios;
            _nombreCategoria = nombreCategoria;
            _nombreSubcategoria = nombreSubcategoria;
            _nombreUsuario = nombreUsuario;
        }

        public ICollection<AnuncioViewModel> Anuncios
        {
            get
            {
                return _anuncios;
            }
        }

        public string NombreCategoria
        {
            get
            {
                return _nombreCategoria;
            }
        }

        public string NombreSubcategoria
        {
            get
            {
                return _nombreSubcategoria;
            }
        }

        public string NombreUsuario
        {
            get
            {
                return _nombreUsuario;
            }
        }
    }

    public class SubcategoriasViewModel {
        private ICollection<SubcategoriaViewModel> _subcategorias;

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
    }

    public class SubcategoriaViewModel {
        private int _id;
        private string _nombre;
        private bool _activo;
        private string _nombreCategoria;
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

        public int NumAnuncios
        {
            get
            {
                return _numAnuncios;
            }
        }

        public SubcategoriaViewModel(int id, string nombre, bool activo, string nombreCategoria, int numAnuncios) {
            _id = id;
            _nombre = nombre;
            _activo = activo;
            _nombreCategoria = nombreCategoria;
            _numAnuncios = numAnuncios;
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

        public CategoriaViewModel(int id, string nombre, bool activo, int numSubcategorias, int numAnuncios)
        {
            _id = id;
            _nombre = nombre;
            _activo = activo;
            _numSubcategorias = numSubcategorias;
            _numAnuncios = numAnuncios;
        }
    }

    public class NuevaSubcategoriaViewModel
    {
        public List<SelectListItem> Categorias{ get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "Categoria")]
        public int Categoria { get; set; }

        [Required(ErrorMessage = "Campo requerido")]
        [Display(Name = "NuevoNombre")]
        public string Nombre { get; set; }
    }

    public class BeneficiosViewModel
    {
    }

    public enum ModificarNombreCategoriaEstatus
    {
        CategoriaExistente,
        Error,
        Exitoso
    }
}