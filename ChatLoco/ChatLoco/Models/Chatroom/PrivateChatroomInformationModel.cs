using ChatLoco.Models.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Chatroom
{
    public class PrivateChatroomInformationModel : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}