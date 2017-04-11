namespace ChatLoco.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rolesEnum : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.UserDTO", "Role");
            AddColumn("dbo.UserDTO", "Role", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserDTO", "Role");
            AddColumn("dbo.UserDTO", "Role", c => c.String());
        }
    }
}
