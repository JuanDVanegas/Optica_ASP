namespace Optica_ASP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class date : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUserData", "FechaNacimiento", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUserData", "FechaNacimiento", c => c.DateTime(nullable: false));
        }
    }
}
