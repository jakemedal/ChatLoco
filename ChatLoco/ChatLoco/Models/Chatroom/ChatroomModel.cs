using ChatLoco.Models.Base;
using ChatLoco.Models.Chatroom;
using ChatLoco.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models
{
    public class ChatroomModel : BaseModel
    {
        public int ChatroomId { get; set; }
        public int ParentChatroomId { get; set; }
        public string ChatroomName { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}