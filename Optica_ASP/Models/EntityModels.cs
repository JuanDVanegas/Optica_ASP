using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Optica_ASP.Models
{
    public class UserData
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string UserId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }

        [Column(TypeName = "Date")]
        public DateTime FechaNacimiento { get; set; }
        public string EntidadId { get; set; }

    }

    [Table("AspNetDocumentTypes")]
    public class DocumentType
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [StringLength(100)]
        public string Nombre { get; set; }

    }

    [Table("AspNetEntitys")]
    public class Entity
    {
        [Key]
        public string Id { get; set; } = Guid.NewGuid().ToString();

        [StringLength(100)]
        public string Nombre { get; set; }

        [StringLength(200)]
        public string Direccion { get; set; }

        public string Codigo { get; set; } = Guid.NewGuid().ToString();
    }
}