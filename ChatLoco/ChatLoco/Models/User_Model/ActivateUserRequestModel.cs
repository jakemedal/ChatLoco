using ChatLoco.Models.Request_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.User_Model
{
    public class ActivateUserRequestModel : RequestModel
    {
        public string ActivationCode { get; set; }
        public int UserId { get; set; }
    }
}