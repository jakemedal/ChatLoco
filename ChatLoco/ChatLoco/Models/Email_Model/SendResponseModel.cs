using ChatLoco.Models.Response_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Email_Model
{
    public class SendResponseModel : ResponseModel
    {
        public string Message { get; set; }
    }
}