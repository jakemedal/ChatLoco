using ChatLoco.Models.Response_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Chatroom_Model
{
    public class GetJoinChatroomFormResponseModel : ResponseModel
    {
        public bool HasPassword { get; set; }
        public int NewChatroomId { get; set; }
        public string ChatroomName { get; set; }
    }
}