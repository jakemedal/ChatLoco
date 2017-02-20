using System;

namespace ChatLoco.Entities.UserDTO
{
    public class UserDTO
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public DateTime JoinDate { get; set; }
        public long PasswordHash { get; set; }
    }
}