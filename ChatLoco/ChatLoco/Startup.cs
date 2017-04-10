
using ChatLoco.DAL;
using ChatLoco.Entities.SettingDTO;
using ChatLoco.Entities.UserDTO;
using ChatLoco.Models.User_Model;
using ChatLoco.Services.Mapping_Service;
using ChatLoco.Services.Security_Service;
using ChatLoco.Services.Setting_Service;
using Microsoft.Owin;
using Owin;
using System;
using System.Linq;

[assembly: OwinStartupAttribute(typeof(ChatLoco.Startup))]
namespace ChatLoco
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            MappingService.InitializeMaps();
            ConfigureAuth(app);
            CreateAdminIfNotExist();
        }
        private void CreateAdminIfNotExist()
        {

            ChatLocoContext DbContext = new ChatLocoContext();
            //if no admin exists then make the default one.
            if(DbContext.Users.FirstOrDefault(u => u.Role==RoleLevel.Admin)==null)
            {
                UserDTO adminUser = new UserDTO()
                {
                    Email = "admin@chatlo.co",
                    JoinDate = DateTime.Now,
                    LastLoginDate = null,
                    PasswordHash = SecurityService.GetStringSha256Hash("Admin"),
                    Username = "Admin",
                    Role = RoleLevel.Admin
                };
                DbContext.Users.Add(adminUser);
                DbContext.SaveChanges();

                SettingDTO settings = SettingService.CreateSettings(adminUser.Id, adminUser.Username);

            }
        }
    }
}
