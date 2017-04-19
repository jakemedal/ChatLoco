namespace ChatLoco.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddStylesColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.MessageDTO", "TextStyle", c => c.String(nullable: true));
        }

        public override void Down()
        {
            DropColumn("dbo.MessageDTO", "TextStyle");
        }
    }
}
