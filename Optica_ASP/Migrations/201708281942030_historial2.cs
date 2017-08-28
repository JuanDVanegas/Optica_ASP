namespace Optica_ASP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class historial2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetHistorial", "MedicoId", c => c.String());
            AddColumn("dbo.AspNetHistorial", "PacienteId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetHistorial", "PacienteId");
            DropColumn("dbo.AspNetHistorial", "MedicoId");
        }
    }
}
