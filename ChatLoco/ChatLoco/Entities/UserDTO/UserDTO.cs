using ChatLoco.Models.User_Model;
using System;

namespace ChatLoco.Entities.UserDTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime JoinDate { get; set; }
        public string PasswordHash { get; set; }
        public DateTime? LastLoginDate { get; set; }
        public RoleLevel Role { get; set; }
        public bool IsActivated { get; set; }
    }
}