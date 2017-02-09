using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Classes.Chatroom
{
    public class Chatroom
    {
        public int Id { get; set; }
        public List<string> AllMessages = new List<string>();
        public Dictionary<string, ActiveUser> AllUsers = new Dictionary<string, ActiveUser>();
        public string Name { get; set; }
        public Dictionary<string, Chatroom> AllSubChatrooms = new Dictionary<string, Chatroom>();

        public Chatroom(string name)
        {
            Name = name;
        }

        public bool HasUser(string Username)
        {
            try
            {
                var User = AllUsers[Username];
                return User.IsActive;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public void AddUser(string Username)
        {
            ActiveUser user = new ActiveUser(Username, true, AllUsers);
            try
            {
                AllUsers.Add(Username, user);
            }
            catch (Exception e) { }
        }

        public void UpdateUsers(string Username)
        {
            AllUsers[Username].IsActive = true;
        }


    }
}