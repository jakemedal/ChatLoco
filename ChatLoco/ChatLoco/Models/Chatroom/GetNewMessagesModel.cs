using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Chatroom
{
    public class GetNewMessagesModel
    {
        public string ChatroomName { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
        public List<string> CurrentMessages { get; set; }

    }
}