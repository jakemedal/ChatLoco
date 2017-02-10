using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Classes.Chatroom
{
    public class Chatroom
    {
        public int Id { get; set; }
        public List<string> AllMessages = new List<string>(); //list of all formatted messages
        public Dictionary<string, ActiveUser> AllUsers = new Dictionary<string, ActiveUser>(); //list of currently active users
        public string Name { get; set; }
        public Dictionary<string, Chatroom> AllSubChatrooms = new Dictionary<string, Chatroom>(); //list of private chatrooms attached to this chatroom
        public bool IsPrivate { get; set; }
        public Chatroom Parent { get; set; }
        
        //force constructor to define name since that is the key that is used in the key value dictionary lists with chatrooms
        public Chatroom(string name)
        {
            Name = name;
        }

        public bool HasUser(string Username)
        {
            //try catch needed since referencing directly with key from a dictionary
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
            try //try catch needed here because the user might possibly already exist in the users list, throwing a exception
            {
                AllUsers.Add(Username, user);
            }
            catch (Exception e) { }
        }

        //This method is used to denote that the user is still active
        public void UpdateUsers(string Username)
        {
            try
            {
                AllUsers[Username].IsActive = true;
            }
            catch(Exception e)
            {
                AddUser(Username);
            }
        }


    }
}