
using System;
using ChatLoco.Models.Request_Model;

namespace ChatLoco.Models.Chatroom_Model
{
    public class ChatRequestModel : RequestModel
    {
        public string ChatroomName { get; set; }
        public string Username { get; set; }
    }
}