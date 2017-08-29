namespace Optica_ASP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class foreignkeys : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.AspNetRegistro", "HistorialId", "dbo.AspNetHistorial");
            DropForeignKey("dbo.AspNetUserData", "UserId", "dbo.AspNetUsers");
            DropIndex("dbo.AspNetRegistro", new[] { "HistorialId" });
            DropIndex("dbo.AspNetUserData", new[] { "UserId" });
            RenameColumn(table: "dbo.AspNetHistorial", name: "HistorialId", newName: "Registro_Id");
            RenameColumn(table: "dbo.AspNetUsers", name: "UserId", newName: "UserData_Id");
            AddColumn("dbo.AspNetHistorial", "Medico_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetHistorial", "Paciente_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetHistorial", "UserData_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUserData", "Entidad_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "Entity_Id", c => c.String(maxLength: 128));
            AlterColumn("dbo.AspNetRegistro", "HistorialId", c => c.String());
            AlterColumn("dbo.AspNetUserData", "UserId", c => c.String());
            CreateIndex("dbo.AspNetUsers", "UserData_Id");
            CreateIndex("dbo.AspNetUsers", "Entity_Id");
            CreateIndex("dbo.AspNetUserData", "Entidad_Id");
            CreateIndex("dbo.AspNetHistorial", "Medico_Id");
            CreateIndex("dbo.AspNetHistorial", "Paciente_Id");
            CreateIndex("dbo.AspNetHistorial", "Registro_Id");
            CreateIndex("dbo.AspNetHistorial", "UserData_Id");
            AddForeignKey("dbo.AspNetUserData", "Entidad_Id", "dbo.AspNetEntitys", "Id");
            AddForeignKey("dbo.AspNetHistorial", "Medico_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetHistorial", "Paciente_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetHistorial", "UserData_Id", "dbo.AspNetUserData", "Id");
            AddForeignKey("dbo.AspNetUsers", "Entity_Id", "dbo.AspNetEntitys", "Id");
            AddForeignKey("dbo.AspNetHistorial", "Registro_Id", "dbo.AspNetRegistro", "Id");
            AddForeignKey("dbo.AspNetUsers", "UserData_Id", "dbo.AspNetUserData", "Id");
            DropColumn("dbo.AspNetHistorial", "MedicoId");
            DropColumn("dbo.AspNetHistorial", "PacienteId");
            DropColumn("dbo.AspNetHistorial", "EntidadId");
            DropColumn("dbo.AspNetUserData", "EntidadId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUserData", "EntidadId", c => c.String());
            AddColumn("dbo.AspNetHistorial", "EntidadId", c => c.String());
            AddColumn("dbo.AspNetHistorial", "PacienteId", c => c.String());
            AddColumn("dbo.AspNetHistorial", "MedicoId", c => c.String());
            DropForeignKey("dbo.AspNetUsers", "UserData_Id", "dbo.AspNetUserData");
            DropForeignKey("dbo.AspNetHistorial", "Registro_Id", "dbo.AspNetRegistro");
            DropForeignKey("dbo.AspNetUsers", "Entity_Id", "dbo.AspNetEntitys");
            DropForeignKey("dbo.AspNetHistorial", "UserData_Id", "dbo.AspNetUserData");
            DropForeignKey("dbo.AspNetHistorial", "Paciente_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetHistorial", "Medico_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserData", "Entidad_Id", "dbo.AspNetEntitys");
            DropIndex("dbo.AspNetHistorial", new[] { "UserData_Id" });
            DropIndex("dbo.AspNetHistorial", new[] { "Registro_Id" });
            DropIndex("dbo.AspNetHistorial", new[] { "Paciente_Id" });
            DropIndex("dbo.AspNetHistorial", new[] { "Medico_Id" });
            DropIndex("dbo.AspNetUserData", new[] { "Entidad_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "Entity_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "UserData_Id" });
            AlterColumn("dbo.AspNetUserData", "UserId", c => c.String(nullable: false, maxLength: 128));
            AlterColumn("dbo.AspNetRegistro", "HistorialId", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.AspNetUsers", "Entity_Id");
            DropColumn("dbo.AspNetUserData", "Entidad_Id");
            DropColumn("dbo.AspNetHistorial", "UserData_Id");
            DropColumn("dbo.AspNetHistorial", "Paciente_Id");
            DropColumn("dbo.AspNetHistorial", "Medico_Id");
            RenameColumn(table: "dbo.AspNetUsers", name: "UserData_Id", newName: "UserId");
            RenameColumn(table: "dbo.AspNetHistorial", name: "Registro_Id", newName: "HistorialId");
            CreateIndex("dbo.AspNetUserData", "UserId");
            CreateIndex("dbo.AspNetRegistro", "HistorialId");
            AddForeignKey("dbo.AspNetUserData", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetRegistro", "HistorialId", "dbo.AspNetHistorial", "Id", cascadeDelete: true);
        }
    }
}
