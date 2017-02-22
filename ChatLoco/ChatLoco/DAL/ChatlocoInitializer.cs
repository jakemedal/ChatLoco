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
                new UserDTO { Id = 0, Username = "Test_User_0", JoinDate = null, PasswordHash = null},
                new UserDTO { Id = 1, Username = "Test_User_1", JoinDate = null, PasswordHash = null},
                new UserDTO { Id = 2, Username = "Test_User_2", JoinDate = null, PasswordHash = null},
                new UserDTO { Id = 3, Username = "Test_User_3", JoinDate = null, PasswordHash = null},
            };

            users.ForEach(u => context.Users.Add(u));
            context.SaveChanges();

            var messages = new List<MessageDTO>
            {
                MessageService.CreateMessage(0, 0, "Test Message 0"),
                MessageService.CreateMessage(1, 0, "Test Message 1"),
                MessageService.CreateMessage(2, 0, "Test Message 2"),
            };

            messages.ForEach(m => context.Messages.Add(m));
            context.SaveChanges();

        }

    }
}