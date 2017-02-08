using ChatLoco.Models.Chatroom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models
{
    public class ChatroomModel
    {
        public string Name { get; set; }
        public string Error { get; set; }
        public string Username { get; set; }

        public ComposedMessageModel ComposedMessageModel { get; set; }
    }
}