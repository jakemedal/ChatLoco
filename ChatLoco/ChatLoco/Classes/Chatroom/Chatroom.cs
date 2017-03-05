using ChatLoco.Entities.MessageDTO;
using ChatLoco.Entities.UserDTO;
using ChatLoco.Models.Chatroom_Service;
using System;
using System.Collections.Generic;

namespace ChatLoco.Classes.Chatroom
{
    public class Chatroom
    {
        public int Id { get; set; }
        public string[] Blacklist { get; set; }
        public string PasswordHash { get; set; }
        public int? Capacity { get; set; }

        private Dictionary<int, string> FormattedMessagesCache = new Dictionary<int, string>();
        private List<int> FormattedMessageOrder = new List<int>();
        private HashSet<string> UserHandles = new HashSet<string>();

        private Dictionary<int, ActiveUser> AllUsers = new Dictionary<int, ActiveUser>();
        public string Name { get; set; }
        private Dictionary<int, Chatroom> AllSubChatrooms = new Dictionary<int, Chatroom>(); 
        public bool IsPrivate { get; set; }
        public Chatroom Parent { get; set; }
        
        public Chatroom(int id, string name)
        {
            Name = name;
            Id = id;
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

        public bool DoesPrivateChatroomExist(int chatroomId)
        {
            return AllSubChatrooms.ContainsKey(chatroomId);
        }

        public bool CreatePrivateChatroom(PrivateChatroomOptions options)
        {
            try
            {
                Chatroom c = new Chatroom(options.Id, options.Name);
                c.PasswordHash = options.PasswordHash;
                if(options.Blacklist != null)
                {
                    c.Blacklist = options.Blacklist.Split(',');
                }
                c.Capacity = options.Capacity;

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

        public bool AddUser(UserDTO user, string userHandle)
        {
            try
            {
                ActiveUser activeUser = new ActiveUser(user, userHandle, true, AllUsers, UserHandles);
                AllUsers.Add(user.Id, activeUser);
                UserHandles.Add(userHandle);
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
                    Username = a.Value.UserHandle
                };
                usersInformation.Add(u);
            }

            return usersInformation;
        }

        public List<JoinChatroomResponseModel> GetPrivateChatroomsInformation()
        {
            List<JoinChatroomResponseModel> chatroomsInformation = new List<JoinChatroomResponseModel>();

            foreach (var privateChatroom in AllSubChatrooms)
            {
                JoinChatroomResponseModel p = new JoinChatroomResponseModel()
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
            AllUsers[id].IsActive = true;
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

        public bool DoesHandleExist(string userHandle)
        {
            return UserHandles.Contains(userHandle);
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

        public string GetUserHandle(int userId)
        {
            return AllUsers[userId].UserHandle;
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
                //we call the user's destroy method, it handles destruction of its internal lists, removal from this users list, and removal from userhandles list
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