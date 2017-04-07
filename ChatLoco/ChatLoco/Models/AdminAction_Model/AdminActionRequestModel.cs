using ChatLoco.Models.Request_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.AdminAction_Model
{
    public class AdminActionRequestModel : RequestModel
    {
        public string Username { get; set; }
        public string Action { get; set; }
    }
}