namespace ChatLoco.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;
    using ChatLoco.Entities.MessageDTO;
    using ChatLoco.Entities.UserDTO;
    using ChatLoco.Entities.SettingDTO;
    using ChatLoco.Services.Message_Service;
    using System.Collections.Generic;
    using System.Web;

    internal sealed class Configuration : DbMigrationsConfiguration<ChatLoco.DAL.ChatLocoContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(ChatLoco.DAL.ChatLocoContext context)
        {
            //  This method will be called after migrating to the latest version.

            var users = new List<UserDTO>
            {
                new UserDTO { Id = 0, Username = "Test_User_0", JoinDate = DateTime.Now, PasswordHash = null},
                new UserDTO { Id = 1, Username = "Test_User_1", JoinDate = DateTime.Now, PasswordHash = null},
                new UserDTO { Id = 2, Username = "Test_User_2", JoinDate = DateTime.Now, PasswordHash = null},
                new UserDTO { Id = 3, Username = "Test_User_3", JoinDate = DateTime.Now, PasswordHash = null},
            };

            users.ForEach(u => context.Users.Add(u));
            context.SaveChanges();

            var messages = new List<MessageDTO>
            {
                new MessageDTO { Id = 0, ChatroomId = 0, DateCreated = DateTime.Now, FormattedMessage = "Test: Test", RawMessage = "Test", UserId = 0 },
                new MessageDTO { Id = 1, ChatroomId = 0, DateCreated = DateTime.Now, FormattedMessage = "Test: Test", RawMessage = "Test", UserId = 0 },
                new MessageDTO { Id = 2, ChatroomId = 0, DateCreated = DateTime.Now, FormattedMessage = "Test: Test", RawMessage = "Test", UserId = 0 }
            };

            messages.ForEach(m => context.Messages.Add(m));
            context.SaveChanges();

            var settings = new List<SettingDTO>
            {
                new SettingDTO { Id = 0,  UserId = 0, DefaultHandle = "testUser0" },
                new SettingDTO { Id = 1,  UserId = 1, DefaultHandle = "testUser1" },
                new SettingDTO { Id = 2,  UserId = 2, DefaultHandle = "testUser2" },
                new SettingDTO { Id = 3,  UserId = 3, DefaultHandle = "testUser3" },
            };

            settings.ForEach(s => context.Settings.Add(s));
            context.SaveChanges();

        }
    }
}
