using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Classes.Chatroom
{
    public class PrivateChatroomOptions
    {
        public int Id { get; set; }
        public string PasswordHash { get; set; }
        public string Blacklist { get; set; }
        public int? Capacity { get; set; }
        public string Name { get; set; }
    }
}