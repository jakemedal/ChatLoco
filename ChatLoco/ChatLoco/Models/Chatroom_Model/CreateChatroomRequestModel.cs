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
        public string Blacklist { get; set; }
        public string Password { get; set; }
        public int? Capacity { get; set; }
    }
}