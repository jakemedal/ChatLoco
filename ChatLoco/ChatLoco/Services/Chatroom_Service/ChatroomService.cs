

using ChatLoco.Classes.Chatroom;
using ChatLoco.DAL;
using ChatLoco.Entities.MessageDTO;
using ChatLoco.Models.Chatroom_Model;
using ChatLoco.Models.Chatroom_Service;
using ChatLoco.Models.Error_Model;
using ChatLoco.Services.Message_Service;
using ChatLoco.Services.Security_Service;
using ChatLoco.Services.User_Service;
using System;
using System.Collections.Generic;

namespace ChatLoco.Services.Chatroom_Service
{
    public static class ChatroomService
    {
        private static Dictionary<int, Chatroom> ChatroomsCache = new Dictionary<int, Chatroom>();

        private static ChatLocoContext DbContext = new ChatLocoContext();

        public static bool RemoveChatroomFromCache(int id)
        {
            ChatroomsCache.Remove(id);
            return true;
        }

        public static List<UserInformationModel> GetUsersInformation(int parentChatroomId, int chatroomId)
        {
            return GetChatroom(chatroomId, parentChatroomId).UsersInformation;
        }

        public static List<JoinChatroomResponseModel> GetPrivateChatroomsInformation(int parentChatroomId)
        {
            return GetChatroom(parentChatroomId).PrivateChatroomsInformation;
        }

        public static bool UpdateUserInChatroom(int parentChatroomId, int chatroomId, int userId)
        {
            try
            {
                GetChatroom(chatroomId, parentChatroomId).UpdateUser(userId);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static string GetChatroomName(int parentChatroomId, int chatroomId)
        {
            return GetChatroom(chatroomId, parentChatroomId).Name;
        }

        public static bool HasPassword(int parentChatroomId, int chatroomId)
        {
            return GetChatroom(chatroomId, parentChatroomId).HasPassword;
        }

        public static List<MessageInformationModel> GetNewMessagesInformation(int parentChatroomId, int chatroomId, List<int> existingIds)
        {
            if(existingIds == null)
            {
                return GetChatroom(chatroomId, parentChatroomId).MessagesInformation;
            }
            else
            {
                return GetChatroom(chatroomId, parentChatroomId).GetNewMessagesInformation(existingIds);
            }
        }

        public static bool DoesChatroomExist(int id)
        {
            try
            {
                Chatroom c = ChatroomsCache[id];
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static bool RemoveUserFromChatroom(int chatroomId, int parentId, int userId)
        {
            try
            {
                GetChatroom(chatroomId, parentId).RemoveUser(userId);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static bool AddUserToChatroom(int chatroomId, int parentId, int userId, string userHandle)
        {
            try
            {
                return GetChatroom(chatroomId, parentId).AddUser(UserService.GetUser(userId), userHandle);
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static bool CreateChatroom(int id, string name)
        {
            try
            {
                Chatroom c = new Chatroom(id, name);
                ChatroomsCache.Add(id, c);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static Chatroom GetChatroom(int parentChatroomId)
        {
            try
            {
                return ChatroomsCache[parentChatroomId];
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public static Chatroom GetChatroom(int chatroomId, int parentId)
        {
            try
            {
                if(chatroomId != parentId)
                {
                    return ChatroomsCache[parentId].GetPrivateChatroom(chatroomId);
                }
                else
                {
                    return ChatroomsCache[parentId];
                }
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public static MessageSend SendMessage(string message, int userId, int chatroomId, int parentId)
        {
            MessageSend messageSend = new MessageSend();
            try
            {
                var chatroom = GetChatroom(chatroomId, parentId);

                var desiredUserId = -1;
                var desiredUserHandle = "";

                var args = message.Split(' ');
                if(args[0] == "/whisper")
                {
                    if(args.Length > 2)
                    {
                        desiredUserHandle = args[1];
                        desiredUserId = chatroom.GetUserIdByHandle(desiredUserHandle);
                        if(desiredUserId == -1)
                        {
                            messageSend.Errors.Add(new ErrorModel("Desired User Not Found."));
                            messageSend.IsSent = false;
                            return messageSend;
                        }
                        else
                        {
                            string s = "";
                            for(int i = 2; i < args.Length; i++)
                            {
                                s += args[i]+ " ";
                            }

                            message = s;
                        }
                    }
                    else
                    {
                        messageSend.Errors.Add(new ErrorModel("Invalid whisper format."));
                        messageSend.IsSent = false;
                        return messageSend;
                    }
                }

                var createdMessages = MessageService.CreateMessages(userId, chatroom.IsPrivate ? -1 : chatroomId, message, chatroom.GetUserHandle(userId), desiredUserId, desiredUserHandle);

                foreach(var m in createdMessages)
                {
                    chatroom.AddMessage(m);
                    messageSend.IsSent = true;
                    messageSend.MessageId = m.Id;
                }
            }
            catch (Exception e)
            {
                messageSend.Errors.Add(new ErrorModel(e.ToString()));
                messageSend.IsSent = false;
            }

            return messageSend;
        }

        public static List<ErrorModel> CreatePrivateChatroom(CreateChatroomRequestModel request)
        {
            var errors = new List<ErrorModel>();

            var chatroom = GetChatroom(request.ParentChatroomId);

            int privateChatroomId = request.ChatroomName.GetHashCode();

            if (chatroom.DoesPrivateChatroomExist(privateChatroomId))
            {
                errors.Add(new ErrorModel("A private chatroom with this name already exists."));
            }
            else
            {
                string passwordHash = null;
                if(request.Password != null && request.Password.Length > 0)
                {
                    passwordHash = SecurityService.GetStringSha256Hash(request.Password);
                }

                var options = new PrivateChatroomOptions()
                {
                    Id = privateChatroomId,
                    Blacklist = request.Blacklist,
                    PasswordHash = passwordHash,
                    Capacity = request.Capacity,
                    Name = request.ChatroomName,
                    Parent = chatroom
                };

                if (!chatroom.CreatePrivateChatroom(options))
                {
                    errors.Add(new ErrorModel("Could not create private chatroom."));
                }
            }

            return errors;
        }

    }
}