namespace ChatLoco.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class rolesEnum : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.UserDTO", "Role", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.UserDTO", "Role", c => c.String());
        }
    }
}
