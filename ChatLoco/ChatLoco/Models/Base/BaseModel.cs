using ChatLoco.Models.Error;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Base
{
    public class BaseModel
    {
        public BaseModel()
        {
            Errors = new List<ErrorModel>();
        }
        public List<ErrorModel> Errors { get; set; }
    }
}