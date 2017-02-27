using ChatLoco.Models.Base_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Chatroom_Model
{
    public class ChatroomModel : BaseModel
    {
        public int ChatroomId { get; set; }
        public int ParentChatroomId { get; set; }
        public string ChatroomName { get; set; }
        public int UserId { get; set; }
        public string UserHandle { get; set; }
    }
}