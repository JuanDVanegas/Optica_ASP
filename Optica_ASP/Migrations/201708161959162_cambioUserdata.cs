namespace Optica_ASP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class cambioUserdata : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUserData", "EntidadId", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUserData", "EntidadId");
        }
    }
}
