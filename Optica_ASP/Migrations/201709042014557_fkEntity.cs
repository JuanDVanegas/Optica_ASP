namespace Optica_ASP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fkEntity : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AspNetMedico", name: "Entidad_EntityId", newName: "EntidaId");
            RenameIndex(table: "dbo.AspNetMedico", name: "IX_Entidad_EntityId", newName: "IX_EntidaId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.AspNetMedico", name: "IX_EntidaId", newName: "IX_Entidad_EntityId");
            RenameColumn(table: "dbo.AspNetMedico", name: "EntidaId", newName: "Entidad_EntityId");
        }
    }
}
