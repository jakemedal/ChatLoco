
using ChatLoco.Models.Error_Model;
using ChatLoco.Models.User_Model;
using System.Collections.Generic;

namespace ChatLoco.Models.Base_Model
{
    public class BaseModel : ErrorsModel
    {
        public UserModel User { get; set; }
    }
}