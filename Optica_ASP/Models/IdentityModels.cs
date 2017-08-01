using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System;

namespace Optica_ASP.Models
{
    // Puede agregar datos del perfil del usuario agregando más propiedades a la clase ApplicationUser. Para más información, visite http://go.microsoft.com/fwlink/?LinkID=317594.
    public class ApplicationUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ApplicationUser> manager)
        {
            // Tenga en cuenta que el valor de authenticationType debe coincidir con el definido en CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Agregar aquí notificaciones personalizadas de usuario
            return userIdentity;
        }

        public ApplicationUser()
        {
            UserData = new List<UserData>();
        }
        public virtual ICollection<UserData> UserData { get; private set; }
    }

    public class UserData
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Nombre { get; set; }
        public string Apellido { get; set; }
        public UserData()
        {
            TipoDocumento = new List<DocumentType>();
        }
        public virtual ICollection<DocumentType> TipoDocumento { get; private set; }
        public string Documento { get; set; }
        public DateTime FechaNacimiento { get; set; }

    }

    public class DocumentType
    {
        public string Id { get; set; }
        public string TipoDocumento { get; set; }
    }

    public class ApplicationRole : IdentityRole
    {
        public ApplicationRole() : base() { }
        public ApplicationRole(string roleName) : base(roleName) { }
    }

    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext()
            : base("DB_Optidca", throwIfV1Schema: false)
        {
        }
        public static ApplicationDbContext Create()
        {
            return new ApplicationDbContext();
        }

        public DbSet<UserData> UserData { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var userData = modelBuilder.Entity<UserData>();
            userData.ToTable("AspNetUserData");
            userData.HasKey(x => x.Id);
            userData.HasMany(x => x.TipoDocumento).WithRequired().HasForeignKey(x => x.Id);

            var document = modelBuilder.Entity<DocumentType>();
            document.ToTable("AspNetDocumentType");
            document.HasKey(x => x.Id);

            var user = modelBuilder.Entity<ApplicationUser>();
            user.HasMany(x => x.UserData).WithRequired().HasForeignKey(x => x.UserId);
        }
    }
}