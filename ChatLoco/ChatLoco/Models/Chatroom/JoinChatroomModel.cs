using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Chatroom
{
    public class JoinChatroomModel
    {
        public int UserId { get; set; }
        public int ChatroomId { get; set; }
        public int ParentChatroomId { get; set; }
        public int CurrentChatroomId { get; set; }
    }
}