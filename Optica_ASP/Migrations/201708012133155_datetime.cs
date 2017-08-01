namespace Optica_ASP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUserData", "FechaNacimiento", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUserData", "FechaNacimiento", c => c.String());
        }
    }
}
