
using ChatLoco.Models.Request_Model;

namespace ChatLoco.Models.Chatroom_Service
{
    public class GetChatroomInformationRequestModel : RequestModel
    {
        public int UserId { get; set; }
        public int ChatroomId { get; set; }
        public int ParentChatroomId { get; set; }
    }
}