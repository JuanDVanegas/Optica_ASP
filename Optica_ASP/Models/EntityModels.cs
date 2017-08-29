using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Optica_ASP.Models
{
    [Table("AspNetUserData")]
    public class UserData
    {
        [ForeignKey("User")]
        public string UserDataId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public string TipoDocumento { get; set; }
        public string Documento { get; set; }
        [Column(TypeName = "Date")]
        public DateTime FechaNacimiento { get; set; }
        public virtual ApplicationUser User { get; set; }
        public virtual ICollection<Historial> Historial { get; set; }
        public virtual Entity Entidad { get; set; }
        public UserData()
        {
            Historial = new List<Historial>();   
        }
    }

    [Table("AspNetDocumentTypes")]
    public class DocumentType
    {
        [Key]
        public string DocumentTypeId { get; set; } = Guid.NewGuid().ToString();
        [StringLength(100)]
        public string Nombre { get; set; }

    }

    [Table("AspNetEntitys")]
    public class Entity
    {
        [Key]
        public string EntityId { get; set; } = Guid.NewGuid().ToString();
        [StringLength(100)]
        public string Nombre { get; set; }
        [StringLength(200)]
        public string Direccion { get; set; }
        public string Codigo { get; set; } = Guid.NewGuid().ToString();
        public virtual ICollection<ApplicationUser> Medico { get; set; }
        public Entity()
        {
            Medico = new List<ApplicationUser>();   
        }
    }

    [Table("AspNetHistorial")]
    public class Historial
    {
        [Key]
        public string HistorialId { get; set; } = Guid.NewGuid().ToString();
        [Column(TypeName = "Date")]
        public DateTime Fecha { get; set; }
        public virtual ApplicationUser Medico { get; set; }
        public virtual ApplicationUser Paciente { get; set; }
        public virtual Registro Registro { get; set; }
    }

    [Table("AspNetRegistro")]
    public class Registro
    {
        [ForeignKey("Historial")]
        public string RegistroId { get; set; } = Guid.NewGuid().ToString();
        public string Descripcion { get; set; }
        public string Resultado { get; set; }
        public string Tratamiento { get; set; }
        public virtual Historial Historial { get; set; }
    }
}