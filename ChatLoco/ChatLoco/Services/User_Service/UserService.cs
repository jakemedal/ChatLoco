using ChatLoco.DAL;
using ChatLoco.Entities.UserDTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Services.User_Service
{
    public static class UserService
    {

        private static ChatLocoContext DbContext = new ChatLocoContext();

        private static int UniqueId = 10;

        public static int GetUniqueId()
        {
            UniqueId += 1;
            return UniqueId;
        }

        public static Dictionary<int, UserDTO> UsersCache = new Dictionary<int, UserDTO>()
        {
            //{ 0, new UserDTO { Id = 0, Username = "Test_User" } }
        };

        public static UserDTO GetUser(int id)
        {
            try
            {
                return UsersCache[id];
            }
            catch(Exception e)
            {
                UserDTO user = DbContext.Users.FirstOrDefault(u => u.Id == id);
                if(user != null)
                {
                    UsersCache.Add(user.Id, user);
                }
                return user;
            }
        }

        public static UserDTO GetUser(string username)
        {
            UserDTO user = DbContext.Users.FirstOrDefault(u => u.Username == username);
            if(user != null && !UsersCache.ContainsKey(user.Id))
            {
                UsersCache.Add(user.Id, user);
            }

            return user;
        }
        
        public static bool DoesUserExist(int id)
        {
            try
            {
                UserDTO u = UsersCache[id];
                return true;
            }
            catch(Exception e)
            {
                UserDTO user = DbContext.Users.FirstOrDefault(u => u.Id == id);
                if (user != null)
                {
                    UsersCache.Add(user.Id, user);
                    return true;
                }
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

                DbContext.Users.Add(u);
                DbContext.SaveChanges();

                UsersCache.Add(id, u);
                return u;
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}