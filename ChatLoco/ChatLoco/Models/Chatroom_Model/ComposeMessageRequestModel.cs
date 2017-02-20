using ChatLoco.Models.Request_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Chatroom_Model
{
    public class ComposeMessageRequestModel : RequestModel
    {
        public string Message { get; set; }
        public int ChatroomId { get; set; }
        public int UserId { get; set; }
        public int ParentChatroomId { get; set; }
    }
}