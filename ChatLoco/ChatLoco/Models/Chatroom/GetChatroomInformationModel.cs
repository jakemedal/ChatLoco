using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Chatroom
{
    public class GetChatroomInformationModel
    {
        public int UserId { get; set; }
        public int ChatroomId { get; set; }
        public int ParentChatroomId { get; set; }
    }
}