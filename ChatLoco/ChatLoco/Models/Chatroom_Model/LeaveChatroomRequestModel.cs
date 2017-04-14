using ChatLoco.Models.Request_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Chatroom_Model
{
    public class LeaveChatroomRequestModel : RequestModel
    {
        public int ChatroomId { get; set; }
        public int ParentId { get; set; }
        public int UserId { get; set; }
    }
}