using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;
using Microsoft.AspNet.Identity;

namespace Optica_ASP.Models
{
    public class UpdateViewModel
    {
        public UpdateViewModel()
        {
            
        }
        public UpdateViewModel(ApplicationUser user)
        {
            Nombre = user.UserData.First().Nombre;
            Apellido = user.UserData.First().Apellido;
            TipoDocumento = user.UserData.First().TipoDocumento;
            Documento = user.UserData.First().Documento;
            FechaNacimiento = user.UserData.First().FechaNacimiento;
            Email = user.Email;
        }

        [Display(Name = "Rol de Usuario")]
        public string RoleName { get; set; }

        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Display(Name = "Tipo de Documento")]
        public string TipoDocumento { get; set; }

        [Display(Name = "Numero de Documento")]
        public string Documento { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Fecha de Nacimiento")]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }
}