using ChatLoco.Models.Response_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.User_Model
{
    public class CreateUserResponseModel : ResponseModel
    {
        public string Username { get; set; }
        public string Email { get; set; }
    }
}