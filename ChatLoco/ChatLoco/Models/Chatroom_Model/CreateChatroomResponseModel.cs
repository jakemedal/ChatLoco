using ChatLoco.Models.Base_Model;
using ChatLoco.Models.Response_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Chatroom_Model
{
    public class CreateChatroomResponseModel : ResponseModel
    {
        public int ChatroomId { get; set; }
        public int ParentChatroomId { get; set; }
        public int UserId { get; set; }
        public string ChatroomName { get; set; }
    }
}