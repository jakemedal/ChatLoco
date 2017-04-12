namespace ChatLoco.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class dropAddEmail : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.SettingDTO", "Email", c => c.String());
            DropColumn("dbo.UserDTO", "Email");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserDTO", "Email", c => c.String());
            DropColumn("dbo.SettingDTO", "Email");
        }
    }
}
