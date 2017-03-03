using AutoMapper;
using ChatLoco.DAL;
using ChatLoco.Entities.UserDTO;
using ChatLoco.Models.Error_Model;
using ChatLoco.Models.User_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Services.User_Service
{
    public static class UserService
    {
        public static Dictionary<int, UserDTO> UsersCache = new Dictionary<int, UserDTO>();

        //Found at http://stackoverflow.com/questions/3984138/hash-string-in-c-sharp
        internal static string GetStringSha256Hash(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }

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

            string passwordHash = GetStringSha256Hash(password);

            UserDTO user = new UserDTO()
            {
                Email = email,
                JoinDate = DateTime.Now,
                LastLoginDate = null,
                PasswordHash = passwordHash,
                Username = username
            };

            db.Users.Add(user);
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
            string passwordHash = GetStringSha256Hash(request.Password);

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
    }
}