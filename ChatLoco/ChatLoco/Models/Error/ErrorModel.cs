using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Error
{
    public class ErrorModel
    {
        public ErrorModel(string error)
        {
            ErrorMessage = error;
        }

        public string ErrorMessage { get; set; }
    }
}