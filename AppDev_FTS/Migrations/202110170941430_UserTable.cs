namespace AppDev_FTS.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserTable : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "FullName", c => c.String(maxLength: 225));
            AddColumn("dbo.AspNetUsers", "Age", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Address", c => c.String(maxLength: 255));
        }

        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Address");
            DropColumn("dbo.AspNetUsers", "Age");
            DropColumn("dbo.AspNetUsers", "FullName");
        }
    }
}
