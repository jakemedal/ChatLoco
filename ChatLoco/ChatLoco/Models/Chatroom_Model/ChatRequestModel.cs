
using System;
using ChatLoco.Models.Request_Model;

namespace ChatLoco.Models.Chatroom_Model
{
    public class ChatRequestModel : RequestModel
    {
        public int ChatroomId { get; set; }
        public string UserHandle { get; set; }
        public string ChatroomName { get; set; }
    }
}