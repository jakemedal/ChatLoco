using System;
using System.Collections.Generic;
using ChatLoco.DAL;
using ChatLoco.Entities.SettingDTO;
using System.Linq;
using System.Web;

namespace ChatLoco.Services.Setting_Service
{
    public class SettingService
    {
        public static Dictionary<int, SettingDTO> SettingsCache = new Dictionary<int, SettingDTO>();

        public static SettingDTO CreateSettings(int userId, string defaulthandle)
        {
            System.Diagnostics.Debug.WriteLine("creating settings for user!!!");
            ChatLocoContext DbContext = new ChatLocoContext();
            try
            {
                SettingDTO setting = new SettingDTO()
                {
                    UserId = userId,
                    DefaultHandle = defaulthandle
                };

                //add it to the table and commit the changes
                System.Diagnostics.Debug.WriteLine("adding setting for user!!");
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

    }
}