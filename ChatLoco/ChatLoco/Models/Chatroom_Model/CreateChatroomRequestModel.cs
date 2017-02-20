using ChatLoco.Models.Request_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChatLoco.Models.Response_Model;

namespace ChatLoco.Models.Chatroom_Model
{
    public class CreateChatroomRequestModel : RequestModel
    {
        public string ChatroomName { get; set; }
        public int ParentChatroomId { get; set; }
        public int UserId { get; set; }
    }
}