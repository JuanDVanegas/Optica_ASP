using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
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
        public UpdateViewModel(ApplicationUser user, string roleName)
        {
            RoleName = roleName;
            Nombre = user.UserData.Nombre;
            Apellido = user.UserData.Apellido;
            TipoDocumento = user.UserData.TipoDocumento;
            Documento = user.UserData.Documento;
            FechaNacimiento = user.UserData.FechaNacimiento;
            Email = user.Email;
        }

        [Required]
        [Display(Name = "Rol de Usuario")]
        public string RoleName { get; set; }

        [Required]
        [Display(Name = "Nombre")]
        public string Nombre { get; set; }

        [Required]
        [Display(Name = "Apellido")]
        public string Apellido { get; set; }

        [Required]
        [Display(Name = "Tipo de Documento")]
        public string TipoDocumento { get; set; }

        [Required]
        [Display(Name = "Numero de Documento")]
        public string Documento { get; set; }

        [Required]
        [Display(Name = "Fecha de Nacimiento")]
        [DataType(DataType.Date), DisplayFormat(DataFormatString = "{0:yyyy-MM-dd}", ApplyFormatInEditMode = true)]
        public DateTime FechaNacimiento { get; set; }

        [Required]
        [EmailAddress]
        [Display(Name = "Correo electrónico")]
        public string Email { get; set; }
    }
}