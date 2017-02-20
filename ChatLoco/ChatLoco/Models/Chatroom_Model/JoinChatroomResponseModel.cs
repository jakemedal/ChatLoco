
using ChatLoco.Models.Base_Model;
using ChatLoco.Models.Response_Model;

namespace ChatLoco.Models.Chatroom_Service
{
    public class JoinChatroomResponseModel : ResponseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}