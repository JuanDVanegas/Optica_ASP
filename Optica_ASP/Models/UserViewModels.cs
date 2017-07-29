using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Optica_ASP.Models
{
    public class UserViewModels
    {
        public string Id { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string Documento { get; set; }
        public DateTime FechaNacimiento { get; set; }       
    }
}