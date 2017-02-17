using ChatLoco.Entities.MessageDTO;
using ChatLoco.Entities.UserDTO;
using ChatLoco.Models.Chatroom;
using ChatLoco.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Classes.Chatroom
{
    public class Chatroom
    {
        public int Id { get; set; }

        private Dictionary<int, string> FormattedMessagesCache = new Dictionary<int, string>();
        private List<int> FormattedMessageOrder = new List<int>();

        private Dictionary<int, ActiveUser> AllUsers = new Dictionary<int, ActiveUser>();
        public string Name { get; set; }
        private Dictionary<int, Chatroom> AllSubChatrooms = new Dictionary<int, Chatroom>(); 
        public bool IsPrivate { get; set; }
        public Chatroom Parent { get; set; }
        
        public Chatroom(int id, string name)
        {
            Name = name;
            Id = id;
            AddMessage(MessageService.CreateMessage(0, Id, "Test Message 1"));
            AddMessage(MessageService.CreateMessage(0, Id, "Test Message 2"));
        }

        public List<string> GetOrderedFormattedMessages()
        {
            try
            {
                List<string> orderedMessages = new List<string>();
                foreach (int i in FormattedMessageOrder)
                {
                    orderedMessages.Add(FormattedMessagesCache[i]);
                }
                return orderedMessages;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public Chatroom GetPrivateChatroom(int chatroomId)
        {
            try
            {
                return AllSubChatrooms[chatroomId];
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public bool CreatePrivateChatroom(int chatroomId, string chatroomName)
        {
            try
            {
                Chatroom c = new Chatroom(chatroomId, chatroomName);
                AllSubChatrooms.Add(c.Id, c);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool HasUser(int id)
        {
            try
            {
                var User = AllUsers[id];
                return User.IsActive;
            }
            catch (Exception e)
            {
                return false;
            }
        }

        public bool AddUser(UserDTO user)
        {
            try
            {
                ActiveUser activeUser = new ActiveUser(user, true, AllUsers);
                AllUsers.Add(user.Id, activeUser);
                return true;
            }
            catch (Exception e)
            {
                return false;
            }
        }
        
        public List<UserInformationModel> GetUsersInformation()
        {
            List<UserInformationModel> usersInformation = new List<UserInformationModel>();

            foreach(var a in AllUsers)
            {
                UserInformationModel u = new UserInformationModel(){
                    Id = a.Key,
                    Username = a.Value.UserName
                };
                usersInformation.Add(u);
            }

            return usersInformation;
        }

        public List<PrivateChatroomInformationModel> GetPrivateChatroomsInformation()
        {
            List<PrivateChatroomInformationModel> chatroomsInformation = new List<PrivateChatroomInformationModel>();

            foreach (var privateChatroom in AllSubChatrooms)
            {
                PrivateChatroomInformationModel p = new PrivateChatroomInformationModel()
                {
                    Id = privateChatroom.Key,
                    Name = privateChatroom.Value.Name
                };
                chatroomsInformation.Add(p);
            }

            return chatroomsInformation;
        }

        public void UpdateUser(int id)
        {
            try
            {
                AllUsers[id].IsActive = true;
            }
            catch(Exception e)
            {
                UserDTO u = UserService.GetUser(id);
                AddUser(u);
            }
        }

        public List<MessageInformationModel> GetAllMessagesInformation()
        {
            try
            {
                List<MessageInformationModel> allMessagesInformation = new List<MessageInformationModel>();
                foreach (int id in FormattedMessageOrder)
                {
                    MessageInformationModel m = new MessageInformationModel()
                    {
                        Id = id,
                        FormattedMessage = FormattedMessagesCache[id]
                    };
                    allMessagesInformation.Add(m);
                }
                return allMessagesInformation;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        //TODO - Security features here, later on
        public bool CanUserJoinChatroom(int userId)
        {
            return true;
        }

        public List<MessageInformationModel> GetNewMessagesInformation(List<int> currentMessagesIds)
        {
            try
            {
                List<MessageInformationModel> newMessages = new List<MessageInformationModel>();

                foreach (var formattedMessage in FormattedMessagesCache)
                {
                    if (!currentMessagesIds.Contains(formattedMessage.Key))
                    {
                        MessageInformationModel m = new MessageInformationModel()
                        {
                            Id = formattedMessage.Key,
                            FormattedMessage = formattedMessage.Value
                        };
                        newMessages.Add(m);
                    }
                }

                return newMessages;
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public bool AddMessage(MessageDTO message)
        {
            try
            {
                FormattedMessagesCache.Add(message.Id, message.FormattedMessage);
                FormattedMessageOrder.Add(message.Id);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public bool RemoveUser(int id)
        {
            try
            {
                //we call the user's destroy method, it handles destruction of its internal lists and removal from this list
                AllUsers[id].Destroy();
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }


    }
}