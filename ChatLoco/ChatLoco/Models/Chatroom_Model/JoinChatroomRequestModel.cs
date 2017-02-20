
using ChatLoco.Models.Request_Model;

namespace ChatLoco.Models.Chatroom_Service
{
    public class JoinChatroomRequestModel : RequestModel
    {
        public int UserId { get; set; }
        public int ChatroomId { get; set; }
        public int ParentChatroomId { get; set; }
        public int CurrentChatroomId { get; set; }
    }
}