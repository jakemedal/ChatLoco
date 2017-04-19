using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Classes.Chatroom
{
    public class ChatroomMesssage
    {
        public string RawMessage { get; set; }
        public int IntendedForUserId { get; set; } = -1;
    }
}