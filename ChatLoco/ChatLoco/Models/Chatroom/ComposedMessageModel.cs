using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Chatroom
{
    public class ComposedMessageModel
    {
        public string Message { get; set; }
        public string Username { get; set; }
        public string ChatroomName { get; set; }
        public int UserId { get; set; }
    }
}