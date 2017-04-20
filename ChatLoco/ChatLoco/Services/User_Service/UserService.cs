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
            if(db.Settings.FirstOrDefault(s => s.Email == email) != null)
            {
                errors.Add(new ErrorModel("Email already in use."));
                return errors;
            }

            string passwordHash = SecurityService.GetStringSha256Hash(password);

            UserDTO user = new UserDTO()
            {
                JoinDate = DateTime.Now,
                LastLoginDate = null,
                PasswordHash = passwordHash,
                Username = username,
                Role = RoleLevel.User
            };

            db.Users.Add(user);
            db.SaveChanges();

            SettingDTO settings = SettingService.CreateSettings(user.Id, user.Username, email);//the default handle is the users username
            if (settings == null) {
                //somehow failed to create user settings
                errors.Add(new ErrorModel("Failure to create User Setting!."));
                return errors;
            }

            return errors;
        }

        public static List<ErrorModel> RemoveUser(RemoveUserRequestModel request)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            if (!DoesUserExist(request.Username))
            {
                errors.Add(new ErrorModel("user.no.exist"));
                return errors;
            }
            ChatLocoContext db = new ChatLocoContext();
            UserDTO user = db.Users.FirstOrDefault(u => u.Username == request.Username);
            
            if(user == null){
                errors.Add(new ErrorModel("system.database.error"));
                return errors;
            }

            SettingDTO settings = db.Settings.FirstOrDefault(s => s.UserId == user.Id);

            if (settings != null)
            {
                db.Settings.Remove(settings);
            }
            db.Users.Remove(user);
            db.SaveChanges();
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
            if(user.Role==RoleLevel.Blocked)
            {
                response.LoginErrors.Add(new ErrorModel("This account has been blocked.<br />Please reach out to us via our contact page if you want to be unblocked."));
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

            //var userModel = Mapper.Map<UserDTO, UserModel>(user);
            var userModel = new UserModel()
            {
                Id = user.Id,
                Username = user.Username,
                Role = user.Role
            };

            var userSettings = SettingService.GetSettings(userModel.Id);

            userModel.Settings = new UserSettingsModel()
            {
                DefaultHandle = userSettings.DefaultHandle,
                Email = userSettings.Email
            };

            response.User = userModel;

            return response;
        }
        public static UserInfoResponseModel GetUserInfoResponseModel(UserInfoRequestModel request)
        {
            var response = new UserInfoResponseModel();

            var userSettings = SettingService.GetSettings(request.Id);

            ChatLocoContext DbContext = new ChatLocoContext();
            UserDTO user = DbContext.Users.FirstOrDefault(u => u.Id == request.Id);

            response.DefaultHandle = userSettings.DefaultHandle;
            response.Username = user.Username;
            response.Email = userSettings.Email;

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
            if (user.Role==RoleLevel.Admin)
            {
                errors.Add(new ErrorModel("User "+ uName +" is already an administrator."));
                return errors;
            }
            if (user.Role == RoleLevel.Blocked)
            {
                errors.Add(new ErrorModel("User " + uName + " is currently blocked."));
            }
            user.Role = RoleLevel.Admin;
            db.SaveChanges();
            return errors;
        }

        public static List<ErrorModel> blockUser(string uName)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            ChatLocoContext db = new ChatLocoContext();
            UserDTO user = db.Users.FirstOrDefault(u => u.Username == uName);
            if (user == null)
            {
                errors.Add(new ErrorModel("User " + uName + " does not exist."));
                return errors;
            }
            if(user.Role == RoleLevel.Blocked)
            {
                errors.Add(new ErrorModel("User " + uName + " is already blocked."));
                return errors;
            }
            user.Role = RoleLevel.Blocked;
            db.SaveChanges();
            return errors;
        }

        public static List<ErrorModel> deleteUserAccount(string uName)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            ChatLocoContext db = new ChatLocoContext();
            UserDTO user = db.Users.FirstOrDefault(u => u.Username == uName);
            if(user==null)
            {
                errors.Add(new ErrorModel("User " + uName + " does not exist."));
            }
            db.Users.Remove(user);
            db.SaveChanges();
            return errors;
        }

        public static List<ErrorModel> unblockUser(string uName)
        {
            List<ErrorModel> errors = new List<ErrorModel>();
            ChatLocoContext db = new ChatLocoContext();
            UserDTO user = db.Users.FirstOrDefault(u => u.Username == uName);
            if (user == null)
            {
                errors.Add(new ErrorModel("User " + uName + " does not exist."));
                return errors;
            }
            if(user.Role!=RoleLevel.Blocked)
            {
                errors.Add(new ErrorModel("User is not currently blocked."));
                return errors;
            }
            user.Role = RoleLevel.User;
            db.SaveChanges();
            return errors;
        }

    }
}