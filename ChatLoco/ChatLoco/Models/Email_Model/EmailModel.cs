using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.Email_Model
{
    public class EmailModel 
    {
        public string subject { get; set; }
        public string fromEmail { get; set; }
        public string toEmail { get; set; }
        public string Message { get; set; }
    }
}