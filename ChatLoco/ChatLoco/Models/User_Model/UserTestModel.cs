using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Models.User_Model
{
    public class UserTestModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public int Id { get; set; }
        public string DefaultHandle { get; set; }
    }
}