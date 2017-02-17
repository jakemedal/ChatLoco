using ChatLoco.Entities.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Services
{
    public static class UserService
    {
        public static Dictionary<int, UserDTO> AllUsers = new Dictionary<int, UserDTO>()
        {
            { 0, new UserDTO { Id = 0, Username = "Test_User" } }
        };

        public static UserDTO GetUser(int id)
        {
            try
            {
                return AllUsers[id];
            }
            catch(Exception e)
            {
                return null;
            }
        }
        
        public static bool DoesUserExist(int id)
        {
            try
            {
                UserDTO u = AllUsers[id];
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static UserDTO CreateUser(int id, string username)
        {
            try
            {
                UserDTO u = new UserDTO()
                {
                    Id = id,
                    Username = username
                };
                AllUsers.Add(id, u);
                return u;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}