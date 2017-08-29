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
            RenameColumn(table: "dbo.AspNetRegistro", name: "HistorialId", newName: "RegistroId");
            RenameColumn(table: "dbo.AspNetUserData", name: "UserId", newName: "UserDataId");
            RenameIndex(table: "dbo.AspNetUserData", name: "IX_UserId", newName: "IX_UserDataId");
            RenameIndex(table: "dbo.AspNetRegistro", name: "IX_HistorialId", newName: "IX_RegistroId");
            DropPrimaryKey("dbo.AspNetDocumentTypes");
            DropPrimaryKey("dbo.AspNetEntitys");
            DropPrimaryKey("dbo.AspNetHistorial");
            DropPrimaryKey("dbo.AspNetRegistro");
            DropPrimaryKey("dbo.AspNetUserData");
            AddColumn("dbo.AspNetDocumentTypes", "DocumentTypeId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AspNetEntitys", "EntityId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AspNetHistorial", "HistorialId", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AspNetHistorial", "Medico_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetHistorial", "Paciente_Id", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetHistorial", "UserData_UserDataId", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUserData", "Entidad_EntityId", c => c.String(maxLength: 128));
            AddColumn("dbo.AspNetUsers", "Entity_EntityId", c => c.String(maxLength: 128));
            AddPrimaryKey("dbo.AspNetDocumentTypes", "DocumentTypeId");
            AddPrimaryKey("dbo.AspNetEntitys", "EntityId");
            AddPrimaryKey("dbo.AspNetHistorial", "HistorialId");
            AddPrimaryKey("dbo.AspNetRegistro", "RegistroId");
            AddPrimaryKey("dbo.AspNetUserData", "UserDataId");
            CreateIndex("dbo.AspNetUsers", "Entity_EntityId");
            CreateIndex("dbo.AspNetUserData", "Entidad_EntityId");
            CreateIndex("dbo.AspNetHistorial", "Medico_Id");
            CreateIndex("dbo.AspNetHistorial", "Paciente_Id");
            CreateIndex("dbo.AspNetHistorial", "UserData_UserDataId");
            AddForeignKey("dbo.AspNetUserData", "Entidad_EntityId", "dbo.AspNetEntitys", "EntityId");
            AddForeignKey("dbo.AspNetHistorial", "Medico_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetHistorial", "Paciente_Id", "dbo.AspNetUsers", "Id");
            AddForeignKey("dbo.AspNetHistorial", "UserData_UserDataId", "dbo.AspNetUserData", "UserDataId");
            AddForeignKey("dbo.AspNetUsers", "Entity_EntityId", "dbo.AspNetEntitys", "EntityId");
            AddForeignKey("dbo.AspNetRegistro", "RegistroId", "dbo.AspNetHistorial", "HistorialId");
            AddForeignKey("dbo.AspNetUserData", "UserDataId", "dbo.AspNetUsers", "Id");
            DropColumn("dbo.AspNetDocumentTypes", "Id");
            DropColumn("dbo.AspNetEntitys", "Id");
            DropColumn("dbo.AspNetHistorial", "Id");
            DropColumn("dbo.AspNetHistorial", "MedicoId");
            DropColumn("dbo.AspNetHistorial", "PacienteId");
            DropColumn("dbo.AspNetHistorial", "EntidadId");
            DropColumn("dbo.AspNetRegistro", "Id");
            DropColumn("dbo.AspNetUserData", "Id");
            DropColumn("dbo.AspNetUserData", "EntidadId");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUserData", "EntidadId", c => c.String());
            AddColumn("dbo.AspNetUserData", "Id", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AspNetRegistro", "Id", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AspNetHistorial", "EntidadId", c => c.String());
            AddColumn("dbo.AspNetHistorial", "PacienteId", c => c.String());
            AddColumn("dbo.AspNetHistorial", "MedicoId", c => c.String());
            AddColumn("dbo.AspNetHistorial", "Id", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AspNetEntitys", "Id", c => c.String(nullable: false, maxLength: 128));
            AddColumn("dbo.AspNetDocumentTypes", "Id", c => c.String(nullable: false, maxLength: 128));
            DropForeignKey("dbo.AspNetUserData", "UserDataId", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetRegistro", "RegistroId", "dbo.AspNetHistorial");
            DropForeignKey("dbo.AspNetUsers", "Entity_EntityId", "dbo.AspNetEntitys");
            DropForeignKey("dbo.AspNetHistorial", "UserData_UserDataId", "dbo.AspNetUserData");
            DropForeignKey("dbo.AspNetHistorial", "Paciente_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetHistorial", "Medico_Id", "dbo.AspNetUsers");
            DropForeignKey("dbo.AspNetUserData", "Entidad_EntityId", "dbo.AspNetEntitys");
            DropIndex("dbo.AspNetHistorial", new[] { "UserData_UserDataId" });
            DropIndex("dbo.AspNetHistorial", new[] { "Paciente_Id" });
            DropIndex("dbo.AspNetHistorial", new[] { "Medico_Id" });
            DropIndex("dbo.AspNetUserData", new[] { "Entidad_EntityId" });
            DropIndex("dbo.AspNetUsers", new[] { "Entity_EntityId" });
            DropPrimaryKey("dbo.AspNetUserData");
            DropPrimaryKey("dbo.AspNetRegistro");
            DropPrimaryKey("dbo.AspNetHistorial");
            DropPrimaryKey("dbo.AspNetEntitys");
            DropPrimaryKey("dbo.AspNetDocumentTypes");
            DropColumn("dbo.AspNetUsers", "Entity_EntityId");
            DropColumn("dbo.AspNetUserData", "Entidad_EntityId");
            DropColumn("dbo.AspNetHistorial", "UserData_UserDataId");
            DropColumn("dbo.AspNetHistorial", "Paciente_Id");
            DropColumn("dbo.AspNetHistorial", "Medico_Id");
            DropColumn("dbo.AspNetHistorial", "HistorialId");
            DropColumn("dbo.AspNetEntitys", "EntityId");
            DropColumn("dbo.AspNetDocumentTypes", "DocumentTypeId");
            AddPrimaryKey("dbo.AspNetUserData", "Id");
            AddPrimaryKey("dbo.AspNetRegistro", "Id");
            AddPrimaryKey("dbo.AspNetHistorial", "Id");
            AddPrimaryKey("dbo.AspNetEntitys", "Id");
            AddPrimaryKey("dbo.AspNetDocumentTypes", "Id");
            RenameIndex(table: "dbo.AspNetRegistro", name: "IX_RegistroId", newName: "IX_HistorialId");
            RenameIndex(table: "dbo.AspNetUserData", name: "IX_UserDataId", newName: "IX_UserId");
            RenameColumn(table: "dbo.AspNetUserData", name: "UserDataId", newName: "UserId");
            RenameColumn(table: "dbo.AspNetRegistro", name: "RegistroId", newName: "HistorialId");
            AddForeignKey("dbo.AspNetUserData", "UserId", "dbo.AspNetUsers", "Id", cascadeDelete: true);
            AddForeignKey("dbo.AspNetRegistro", "HistorialId", "dbo.AspNetHistorial", "Id", cascadeDelete: true);
        }
    }
}
