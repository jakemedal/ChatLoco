using System;
using System.Collections.Generic;
using ChatLoco.DAL;
using ChatLoco.Entities.SettingDTO;
using System.Linq;
using System.Web;
using ChatLoco.Models.User_Model;
using AutoMapper;
using ChatLoco.Entities.UserDTO;
using ChatLoco.Models.Error_Model;
using ChatLoco.Services.User_Service;
using ChatLoco.Services.Setting_Service;


namespace ChatLoco.Services.Setting_Service
{
    public class SettingService
    {
        public static Dictionary<int, SettingDTO> SettingsCache = new Dictionary<int, SettingDTO>();

        public static SettingDTO CreateSettings(int userId, string defaulthandle, string email)
        {
            ChatLocoContext DbContext = new ChatLocoContext();
            try
            {
                SettingDTO setting = new SettingDTO()
                {
                    UserId = userId,
                    DefaultHandle = defaulthandle,
                    Email = email
                };

                //add it to the table and commit the changes
                DbContext.Settings.Add(setting); 
                DbContext.SaveChanges();

                SettingsCache.Add(setting.Id, setting);


                return setting;
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static UpdateSettingsResponseModel UpdateSettings(UpdateSettingsRequestModel request)
        {
            var response = new UpdateSettingsResponseModel();

            var user = UserService.GetUser(request.Username);
            if (user == null)
            {
                response.SettingsErrors.Add(new ErrorModel("Username not found."));
                return response;
            }
            //database connection
            var db = new ChatLocoContext();

            var updatedSettings = db.Settings.FirstOrDefault(x => x.UserId == user.Id);//get the settings entry from the database
            updatedSettings.DefaultHandle = request.Settings.DefaultHandle;//update the users default handle
            updatedSettings.Email = request.Settings.Email;//update the users email

            db.SaveChanges();

            //Construct the UserSettingsResponseModel
            var userModel = new UserModel()
            {
                Id = user.Id,
                Username = user.Username
            };
            userModel.Settings = new UserSettingsModel()
            {
                DefaultHandle = updatedSettings.DefaultHandle,
                Email = updatedSettings.Email
            };
            response.User = userModel;

            return response;
        }

        public static SettingDTO GetSettings(int userId)
        {
            ChatLocoContext DbContext = new ChatLocoContext();
            SettingDTO settings = DbContext.Settings.Where(s => s.UserId == userId).FirstOrDefault();
            return settings;
        }

    }
}