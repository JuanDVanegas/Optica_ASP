namespace Optica_ASP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class userdata : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetUserData",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserId = c.String(nullable: false, maxLength: 128),
                        Nombre = c.String(),
                        Apellido = c.String(),
                        TipoDocumetno = c.String(),
                        Documento = c.String(),
                        FechaNacimiento = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUserData", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetUserData", new[] { "UserId" });
            DropTable("dbo.AspNetUserData");
        }
    }
}
