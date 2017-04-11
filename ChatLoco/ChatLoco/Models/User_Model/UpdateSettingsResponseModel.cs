using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ChatLoco.Models.Error_Model;
using ChatLoco.Models.Response_Model;

namespace ChatLoco.Models.User_Model
{
    public class UpdateSettingsResponseModel : ResponseModel
    {
        public List<ErrorModel> SettingsErrors = new List<ErrorModel>();
    }
}