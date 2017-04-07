namespace ChatLoco.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class user_roles : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.UserDTO", "Role", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.UserDTO", "Role");
        }
    }
}
