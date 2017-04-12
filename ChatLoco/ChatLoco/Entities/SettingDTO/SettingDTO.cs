using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Entities.SettingDTO
{
    public class SettingDTO
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public String DefaultHandle { get; set; }
        public string Email { get; set; }
    }
}