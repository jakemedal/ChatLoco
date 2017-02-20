
using ChatLoco.Models.Request_Model;
using System.Collections.Generic;

namespace ChatLoco.Models.Chatroom_Service
{
    public class GetNewMessagesRequestModel : RequestModel
    {
        public int ChatroomId { get; set; }
        public int UserId { get; set; }
        public List<int> ExistingMessageIds { get; set; }
        public int ParentChatroomId { get; set; }
    }
}