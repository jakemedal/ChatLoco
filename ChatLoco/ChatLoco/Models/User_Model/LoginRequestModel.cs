using ChatLoco.Models.Request_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.User_Model
{
    public class LoginRequestModel : RequestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
    }
}