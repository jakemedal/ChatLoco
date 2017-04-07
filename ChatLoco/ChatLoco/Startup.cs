
using ChatLoco.DAL;
using ChatLoco.Entities.UserDTO;
using ChatLoco.Services.Mapping_Service;
using ChatLoco.Services.Security_Service;
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
            if(DbContext.Users.FirstOrDefault(u => u.Role == "Admin")==null)
            {
                UserDTO adminUser = new UserDTO()
                {
                    Email = "admin@chatlo.co",
                    JoinDate = DateTime.Now,
                    LastLoginDate = null,
                    PasswordHash = SecurityService.GetStringSha256Hash("Admin"),
                    Username = "Admin",
                    Role = "Admin"
                };
                DbContext.Users.Add(adminUser);
                DbContext.SaveChanges();

            }
        }
    }
}
