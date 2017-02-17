using ChatLoco.Classes.Chatroom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Entity.User
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        private Chatroom Chatroom { get; set; }
        private ActiveUser ActiveUser { get; set; }

        public User(int id, string username)
        {
            Id = id;
            Username = username;
        }

        public bool JoinChat(int chatroomId)
        {
            if(Chatroom != null)
            {
                if(Chatroom.Id == chatroomId)
                {
                    return true;
                }
                else
                {

                }
            }
            return true;
        }
    }
}