namespace ChatLoco.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class privateMessaging : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MessageDTO", "IntendedForUserId", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.MessageDTO", "IntendedForUserId");
        }
    }
}
