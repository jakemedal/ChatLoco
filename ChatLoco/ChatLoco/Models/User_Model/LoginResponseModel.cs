using ChatLoco.Models.Error_Model;
using ChatLoco.Models.Response_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.User_Model
{
    public class LoginResponseModel : ResponseModel
    {
        public List<ErrorModel> LoginErrors = new List<ErrorModel>();
    }
}