namespace ChatLoco.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class REAladdStyleColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MessageDTO", "Style", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.MessageDTO", "Style");
        }
    }
}
