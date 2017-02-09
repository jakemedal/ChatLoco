using ChatLoco.Models.Base;
using ChatLoco.Models.Chatroom;
using ChatLoco.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models
{
    public class ChatroomModel : BaseModel
    {
        public int Id { get; set; }
        public string Name { get; set; }

        public UserModel UserModel { get; set; }
    }
}