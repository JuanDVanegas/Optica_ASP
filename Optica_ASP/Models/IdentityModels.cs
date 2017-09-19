using System.Data.Entity;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Optica_ASP.Models;

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
        public virtual UserData UserData { get; set; }
        public bool Enabled { get; set; }
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
        public DbSet<DocumentType> DocumentType { get; set; }
        public DbSet<Entity> Entity { get; set; }
        public DbSet<Historial> Historial { get; set; }
        public DbSet<Registro> Registro { get; set; }
        public DbSet<Medico> Medico { get; set; }
        public DbSet<Paciente> Paciente { get; set; }
        //protected override void OnModelCreating(DbModelBuilder modelBuilder)
        //{
        //    base.OnModelCreating(modelBuilder);

        //    var userData = modelBuilder.Entity<UserData>();
        //    userData.HasKey(x => x.UserId);

        //    var user = modelBuilder.Entity<ApplicationUser>();
        //    user.HasRequired(x => x.UserData).WithRequiredPrincipal(x => x.User);

        //    var registro = modelBuilder.Entity<Registro>();
        //    registro.HasKey(x => x.HistorialId);

        //    var historial = modelBuilder.Entity<Historial>();
        //    historial.HasRequired(x => x.Registro).WithRequiredPrincipal(x => x.Historial);
        //}
    }
}