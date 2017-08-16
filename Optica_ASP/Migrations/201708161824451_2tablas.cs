namespace Optica_ASP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _2tablas : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AspNetDocumentTypes",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Nombre = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.AspNetEntitys",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        Nombre = c.String(maxLength: 100),
                        Direccion = c.String(maxLength: 200),
                        Codigo = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.AspNetEntitys");
            DropTable("dbo.AspNetDocumentTypes");
        }
    }
}
