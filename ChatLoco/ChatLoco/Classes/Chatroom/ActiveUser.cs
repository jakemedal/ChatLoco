using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;

namespace ChatLoco.Classes.Chatroom
{
    public class ActiveUser
    {
        public string Username { get; set; }
        public bool IsActive { get; set; }
        private Dictionary<string, ActiveUser> BelongsToUsersList;
        Timer IdleTimer;

        public ActiveUser(string username, bool isActive, Dictionary<string, ActiveUser> usersList)
        {
            Username = username;
            IsActive = isActive;
            BelongsToUsersList = usersList;

            IdleTimer = new Timer();
            IdleTimer.Elapsed += new ElapsedEventHandler(IdleCheck);
            IdleTimer.Interval = 5000;
            IdleTimer.Enabled = true;

        }

        private void IdleCheck(object source, ElapsedEventArgs e)
        {
            if (!IsActive)
            {
                IdleTimer.Enabled = false;
                IdleTimer.Stop();
                IdleTimer.Dispose();
                BelongsToUsersList.Remove(Username);
                Username = null;
                BelongsToUsersList = null;
            }
            else
            {
                IsActive = false;
            }
        }
    }
}