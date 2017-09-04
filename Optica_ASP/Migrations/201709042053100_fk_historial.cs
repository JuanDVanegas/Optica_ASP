namespace Optica_ASP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fk_historial : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AspNetHistorial", name: "Medico_MedicoId", newName: "MedicoId");
            RenameColumn(table: "dbo.AspNetHistorial", name: "Paciente_PacienteId", newName: "PacienteId");
            RenameIndex(table: "dbo.AspNetHistorial", name: "IX_Medico_MedicoId", newName: "IX_MedicoId");
            RenameIndex(table: "dbo.AspNetHistorial", name: "IX_Paciente_PacienteId", newName: "IX_PacienteId");
        }
        
        public override void Down()
        {
            RenameIndex(table: "dbo.AspNetHistorial", name: "IX_PacienteId", newName: "IX_Paciente_PacienteId");
            RenameIndex(table: "dbo.AspNetHistorial", name: "IX_MedicoId", newName: "IX_Medico_MedicoId");
            RenameColumn(table: "dbo.AspNetHistorial", name: "PacienteId", newName: "Paciente_PacienteId");
            RenameColumn(table: "dbo.AspNetHistorial", name: "MedicoId", newName: "Medico_MedicoId");
        }
    }
}
