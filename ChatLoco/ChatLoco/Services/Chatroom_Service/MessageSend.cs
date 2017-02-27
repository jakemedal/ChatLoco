
using ChatLoco.Models.Error_Model;
using System.Collections.Generic;

namespace ChatLoco.Services.Chatroom_Service
{
    public class MessageSend
    {
        public bool IsSent { get; set; }
        public List<ErrorModel> Errors = new List<ErrorModel>();
        public int MessageId { get; set; }
    }
}