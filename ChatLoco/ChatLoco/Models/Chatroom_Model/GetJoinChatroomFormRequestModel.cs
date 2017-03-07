using ChatLoco.Models.Request_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Chatroom_Model
{
    public class GetJoinChatroomFormRequestModel : RequestModel
    {
        public int ChatroomId { get; set; }
        public int ParentChatroomId { get; set; }
    }
}