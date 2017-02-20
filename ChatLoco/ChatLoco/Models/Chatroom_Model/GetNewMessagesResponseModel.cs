using ChatLoco.Models.Chatroom_Service;
using ChatLoco.Models.Response_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Chatroom_Model
{
    public class GetNewMessagesResponseModel : ResponseModel
    {
        public List<MessageInformationModel> MessagesInformation = new List<MessageInformationModel>();
    }
}