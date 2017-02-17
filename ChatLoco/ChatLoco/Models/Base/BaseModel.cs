using ChatLoco.Models.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Base
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