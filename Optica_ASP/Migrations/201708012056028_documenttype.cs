namespace Optica_ASP.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class documenttype : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUserData", "TipoDocumento", c => c.String());
            DropColumn("dbo.AspNetUserData", "TipoDocumetno");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUserData", "TipoDocumetno", c => c.String());
            DropColumn("dbo.AspNetUserData", "TipoDocumento");
        }
    }
}
