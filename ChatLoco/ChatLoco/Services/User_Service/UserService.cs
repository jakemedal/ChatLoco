using AutoMapper;
using ChatLoco.DAL;
using ChatLoco.Entities.UserDTO;
using ChatLoco.Entities.SettingDTO;
using ChatLoco.Models.Error_Model;
using ChatLoco.Models.User_Model;
using ChatLoco.Services.Security_Service;
using ChatLoco.Services.Setting_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Services.User_Service
{
    public static class UserService
    {
        public static Dictionary<int, UserDTO> UsersCache = new Dictionary<int, UserDTO>();

        public static List<ErrorModel> CreateUser(string username, string email, string password)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            if (DoesUserExist(username))
            {
                errors.Add(new ErrorModel("Username already exists."));
                return errors;
            }

            ChatLocoContext db = new ChatLocoContext();
            if(db.Users.FirstOrDefault(u => u.Email == email) != null)
            {
                errors.Add(new ErrorModel("Email already in use."));
                return errors;
            }

            string passwordHash = SecurityService.GetStringSha256Hash(password);

            UserDTO user = new UserDTO()
            {
                Email = email,
                JoinDate = DateTime.Now,
                LastLoginDate = null,
                PasswordHash = passwordHash,
                Username = username,
                Role = "User"
            };

            db.Users.Add(user);
            db.SaveChanges();

            SettingDTO settings = SettingService.CreateSettings(user.Id, user.Username);//the default handle is the users username
            if (settings == null) {
                //somehow failed to create user settings
                errors.Add(new ErrorModel("Failure to create User Setting!."));
                return errors;
            }

            return errors;
        }

        public static bool Logout(int userId)
        {
            try
            {
                UsersCache.Remove(userId);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static LoginResponseModel GetLoginResponseModel(LoginRequestModel request)
        {
            var response = new LoginResponseModel();
            string passwordHash = SecurityService.GetStringSha256Hash(request.Password);

            var user = GetUser(request.Username);
            if (user == null)
            {
                response.LoginErrors.Add(new ErrorModel("Username not found."));
                return response;
            }

            if(!passwordHash.Equals(user.PasswordHash, StringComparison.Ordinal))
            {
                response.LoginErrors.Add(new ErrorModel("Incorrect password."));
                return response;
            }

            var db = new ChatLocoContext();

            user.LastLoginDate = DateTime.Now;

            db.SaveChanges();

            response.User = Mapper.Map<UserDTO, UserModel>(user);

            return response;
        }

        public static UserDTO GetUser(int id)
        {
            ChatLocoContext DbContext = new ChatLocoContext();
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
            ChatLocoContext DbContext = new ChatLocoContext();
            UserDTO user = DbContext.Users.FirstOrDefault(u => u.Username == username);
            if(user != null && !UsersCache.ContainsKey(user.Id))
            {
                UsersCache.Add(user.Id, user);
            }

            return user;
        }

        public static bool DoesUserExist(string username)
        {
            ChatLocoContext db = new ChatLocoContext();
            UserDTO user = db.Users.FirstOrDefault(u => u.Username == username);
            return (user != null);
        }
        
        public static bool DoesUserExist(int id)
        {
            ChatLocoContext DbContext = new ChatLocoContext();
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
        
        public static List<ErrorModel> makeUserAdmin(string uName)
        {
            List<ErrorModel> errors = new List<ErrorModel>();

            ChatLocoContext db = new ChatLocoContext();
            UserDTO user = db.Users.FirstOrDefault(u => u.Username == uName);
            //If user does not exist or they are an admin, fail. Otherwise make the user an admin.
            if(user == null)
            {
                errors.Add(new ErrorModel("User " + uName + " does not exist."));
                return errors;
            }
            if (user.Role=="Admin")
            {
                errors.Add(new ErrorModel("User "+ uName +" is already an administrator."));
                return errors;
            }
            user.Role = "Admin";
            db.SaveChanges();
            return errors;
        }
    }
}