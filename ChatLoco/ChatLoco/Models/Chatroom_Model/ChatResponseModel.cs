

using ChatLoco.Models.Base_Model;
using ChatLoco.Models.Response_Model;
using System.Web.Mvc;

namespace ChatLoco.Models.Chatroom_Model
{
    public class ChatResponseModel : ResponseModel
    {
        public string Data { get; set; }
    }
}