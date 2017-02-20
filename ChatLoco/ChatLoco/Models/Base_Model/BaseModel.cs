
using ChatLoco.Models.Error_Model;
using System.Collections.Generic;

namespace ChatLoco.Models.Base_Model
{
    public class BaseModel
    {
        public List<ErrorModel> Errors = new List<ErrorModel>();

        public void AddError(string msg)
        {
            Errors.Add(new ErrorModel(msg));
        }
    }
}