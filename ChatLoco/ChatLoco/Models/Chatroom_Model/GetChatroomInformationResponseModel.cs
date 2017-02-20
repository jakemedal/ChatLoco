
using ChatLoco.Models.Base_Model;
using ChatLoco.Models.Response_Model;
using System.Collections.Generic;

namespace ChatLoco.Models.Chatroom_Service
{
    public class GetChatroomInformationResponseModel : ResponseModel
    {
        public List<UserInformationModel> UsersInformation { get; set; }
        public List<JoinChatroomResponseModel> PrivateChatroomsInformation { get; set; }
    }
}