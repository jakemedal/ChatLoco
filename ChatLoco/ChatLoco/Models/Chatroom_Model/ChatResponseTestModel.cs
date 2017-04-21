using ChatLoco.Models.Response_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Chatroom_Model
{
    public class ChatResponseTestModel : ResponseModel
    {
        public ChatroomModel ChatroomModel { get; set; }
    }
}