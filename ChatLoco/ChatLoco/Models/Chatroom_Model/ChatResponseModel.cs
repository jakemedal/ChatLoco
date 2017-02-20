

using ChatLoco.Models.Base_Model;
using ChatLoco.Models.Response_Model;

namespace ChatLoco.Models.Chatroom_Model
{
    public class ChatResponseModel : ResponseModel
    {
        public int ChatroomId { get; set; }
        public int ParentChatroomId { get; set; }
        public string ChatroomName { get; set; }
        public int UserId { get; set; }
        public string Username { get; set; }
    }
}