using ChatLoco.Entities.MessageDTO;
using ChatLoco.Entities.UserDTO;
using ChatLoco.Entities.SettingDTO;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.ModelConfiguration.Conventions;
using System.Linq;
using System.Web;

namespace ChatLoco.DAL
{
    public class ChatLocoContext : DbContext
    {


        public ChatLocoContext() : base("ChatlocoContext")
        {
        }

        public DbSet<UserDTO> Users { get; set; }
        public DbSet<MessageDTO> Messages { get; set; }
        public DbSet<SettingDTO> Settings { get; set; }



        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Conventions.Remove<PluralizingTableNameConvention>();
        }

    }
}