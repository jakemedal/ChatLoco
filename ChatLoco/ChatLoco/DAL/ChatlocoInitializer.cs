using ChatLoco.Entities.MessageDTO;
using ChatLoco.Entities.UserDTO;
using ChatLoco.Services.Message_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.DAL
{
    public class ChatLocoInitializer : System.Data.Entity.DropCreateDatabaseIfModelChanges<ChatLocoContext>
    {

        protected override void Seed(ChatLocoContext context)
        {
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

        }

    }
}