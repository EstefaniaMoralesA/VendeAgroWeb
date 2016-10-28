using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Web;

namespace VendeAgroWeb.Models.Portal
{
    public class RegistroViewModel
    {
        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Apellidos")]
        public string Apellidos { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La contraseña debe de ser mínimo de 6 caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = "Confirmar Contraseña")]
        [Compare("Password", ErrorMessage = "La contraseña y su confirmacion no son iguales.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Celular")]
        [StringLength(14, ErrorMessage = "El celular no es válido.", MinimumLength = 10)]
        public string Celular { get; set; }
    }

    public class LoginViewModel {

        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "La contraseña debe de ser mínimo de 6 caracteres.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Contraseña")]
        public string Password { get; set; }

    }

    //[AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    //public class CustomPhoneNumberAttribute : ValidationAttribute
    //{

    //    public CustomPhoneNumberAttribute()
    //    {
          
    //    }

    //    public override bool IsValid(object value)
    //    {
    //        string strValue = value as string;
    //        Debug.WriteLine("String value:" + value);
    //        if (!string.IsNullOrEmpty(strValue))
    //        {
    //            int len = strValue.Length;
    //            return len == 10;
    //        }
    //        return true;
    //    }
    //}
}