namespace Optica_ASP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetodatetime : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUserData", "FechaNacimiento", c => c.DateTime(nullable: false, storeType: "date"));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUserData", "FechaNacimiento", c => c.DateTime(nullable: false));
        }
    }
}
