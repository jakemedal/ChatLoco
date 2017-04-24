using ChatLoco.Entities.MessageDTO;
using ChatLoco.Entities.UserDTO;
using ChatLoco.Models.Chatroom_Service;
using ChatLoco.Services.Chatroom_Service;
using ChatLoco.Services.Message_Service;
using System;
using System.Collections.Generic;
using System.Timers;
using System.Web.Services.Description;

namespace ChatLoco.Classes.Chatroom
{
    public class Chatroom
    {
        public int Id { get; }
        private string[] Blacklist { get; set; }
        private string PasswordHash { get; set; }
        private int? Capacity { get; set; }

        private Dictionary<int, ChatroomMesssage> FormattedMessagesCache = new Dictionary<int, ChatroomMesssage>();
        private List<int> FormattedMessageOrder = new List<int>();
        private HashSet<string> UserHandles = new HashSet<string>();

        private Dictionary<int, ActiveUser> AllUsers = new Dictionary<int, ActiveUser>();
        public string Name { get; }
        private Dictionary<int, Chatroom> AllSubChatrooms = new Dictionary<int, Chatroom>(); 
        public bool IsPrivate { get; set; }
        private Chatroom Parent { get; set; }
        private Timer IdleTimer { get; set; }

        public Chatroom(int id, string name, PrivateChatroomOptions options = null)
        {
            Name = name;
            Id = id;
            
            IdleTimer = new Timer();
            IdleTimer.Elapsed += new ElapsedEventHandler(IdleCheck);
            IdleTimer.Interval = 60000;
            IdleTimer.Enabled = true;

            if(options != null)
            {
                PasswordHash = options.PasswordHash;
                if (options.Blacklist != null)
                {
                    Blacklist = options.Blacklist.Split(',');
                }
                Capacity = options.Capacity;

                Parent = options.Parent;

                IsPrivate = true;
            }
            else
            {
                var messages = MessageService.GetRecentChatroomMessages(Id, 1000);
                foreach (var message in messages)
                {
                    var m = new ChatroomMesssage()
                    {
                        RawMessage = message.FormattedMessage,
                        IntendedForUserId = message.IntendedForUserId
                    };
                    FormattedMessagesCache.Add(message.Id, m);
                    FormattedMessageOrder.Add(message.Id);
                }
            }
        }

        public void IdleCheck(object source, ElapsedEventArgs e)
        {
            if (!IsActive())
            {
                Destroy();
            }
        }

        //recursive way to check chatroom, subchatrooms for activity
        public bool IsActive()
        {
            if (AllUsers.Count != 0)
            {
                return true;
            }
            else
            {
                foreach (var kvPair in AllSubChatrooms)
                {
                    Chatroom c = kvPair.Value;
                    if (c.IsActive())
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public void Destroy()
        {
            IdleTimer.Enabled = false;
            IdleTimer.Stop();
            IdleTimer.Dispose();

            if(Parent != null)
            {
                Parent.AllSubChatrooms.Remove(Id);
            }

            ChatroomService.RemoveChatroomFromCache(Id);
        }

        public bool IsOnBlacklist(string username)
        {
            if(Blacklist == null)
            {
                return false;
            }

            foreach(string s in Blacklist)
            {
                if (s.Equals(username))
                {
                    return true;
                }
            }
            return false;
        }

        public bool CheckPasswordHash(string passwordHash)
        {
            return PasswordHash.Equals(passwordHash);
        }

        public bool HasPassword {
            get {
                return (PasswordHash != null);
            }
        }

        public bool IsAtCapacity
        {
            get
            {
                if (!Capacity.HasValue)
                {
                    return false;
                }
                else
                {
                    return (AllUsers.Count >= Capacity.Value);
                }
            }
        }

        public List<ChatroomMesssage> OrderedFormattedMessages
        {
            get
            {
                try
                {
                    List<ChatroomMesssage> orderedMessages = new List<ChatroomMesssage>();
                    foreach (int i in FormattedMessageOrder)
                    {
                        orderedMessages.Add(FormattedMessagesCache[i]);
                    }
                    return orderedMessages;
                }
                catch (Exception e)
                {
                    return null;
                }
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
                Chatroom c = new Chatroom(options.Id, options.Name, options);

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
        
        public List<UserInformationModel> UsersInformation
        {
            get
            {
                List<UserInformationModel> usersInformation = new List<UserInformationModel>();

                foreach (var a in AllUsers)
                {
                    UserInformationModel u = new UserInformationModel()
                    {
                        Id = a.Key,
                        Username = a.Value.UserHandle
                    };
                    usersInformation.Add(u);
                }

                return usersInformation;
            }
        }

        public List<JoinChatroomResponseModel> PrivateChatroomsInformation
        {
            get
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
        }

        public void UpdateUser(int id)
        {
            AllUsers[id].IsActive = true;
        }

        public List<MessageInformationModel> MessagesInformation
        {
            get
            {
                try
                {
                    List<MessageInformationModel> allMessagesInformation = new List<MessageInformationModel>();
                    foreach (int id in FormattedMessageOrder)
                    {
                        MessageInformationModel m = new MessageInformationModel()
                        {
                            Id = id,
                            FormattedMessage = FormattedMessagesCache[id].RawMessage,
                            IntendedForUserId = FormattedMessagesCache[id].IntendedForUserId,
                            MessageStyle = FormattedMessagesCache[id].StyleMessage
                        };
                        allMessagesInformation.Add(m);
                    }
                    return allMessagesInformation;
                }
                catch (Exception e)
                {
                    return null;
                }
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
                            FormattedMessage = formattedMessage.Value.RawMessage,
                            IntendedForUserId = formattedMessage.Value.IntendedForUserId,
                            MessageStyle = formattedMessage.Value.StyleMessage
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

        public int GetUserIdByHandle(string userHandle)
        {
            foreach(var dictionaryObject in AllUsers)
            {
                ActiveUser user = dictionaryObject.Value;
                if(user.UserHandle == userHandle)
                {
                    return user.Id;
                }
            }
            return -1;
        }

        public bool AddMessage(MessageDTO message)
        {
            try
            {
                var m = new ChatroomMesssage()
                {
                    RawMessage = message.FormattedMessage,
                    IntendedForUserId = message.IntendedForUserId,
                    StyleMessage = message.Style
                };
                FormattedMessagesCache.Add(message.Id, m);
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