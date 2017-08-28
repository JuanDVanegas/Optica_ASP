namespace Optica_ASP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class historial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetHistorial",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Fecha = c.DateTime(nullable: false, storeType: "date"),
                        EntidadId = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetRegistro",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        HistorialId = c.String(nullable: false, maxLength: 128),
                        Descripcion = c.String(),
                        Resultado = c.String(),
                        Tratamiento = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetHistorial", t => t.HistorialId, cascadeDelete: true)
                .Index(t => t.HistorialId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetRegistro", "HistorialId", "dbo.AspNetHistorial");
            DropIndex("dbo.AspNetRegistro", new[] { "HistorialId" });
            DropTable("dbo.AspNetRegistro");
            DropTable("dbo.AspNetHistorial");
        }
    }
}
