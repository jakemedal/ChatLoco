using ChatLoco.Classes.Chatroom;
using ChatLoco.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Chatroom
{
    public class UpdateChatroomInformationModel : BaseModel
    {
        public List<ActiveUser> Users { get; set; }
        public List<string> SubChatroomsNames { get; set; }
    }
}