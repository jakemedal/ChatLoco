

using ChatLoco.Classes.Chatroom;
using ChatLoco.DAL;
using ChatLoco.Entities.MessageDTO;
using ChatLoco.Models.Chatroom_Service;
using ChatLoco.Models.Error_Model;
using ChatLoco.Services.Message_Service;
using ChatLoco.Services.User_Service;
using System;
using System.Collections.Generic;

namespace ChatLoco.Services.Chatroom_Service
{
    public static class ChatroomService
    {
        private static Dictionary<int, Chatroom> ChatroomsCache = new Dictionary<int, Chatroom>();

        private static ChatLocoContext DbContext = new ChatLocoContext();

        public static List<UserInformationModel> GetUsersInformation(int parentChatroomId, int chatroomId)
        {
            return GetChatroom(chatroomId, parentChatroomId).GetUsersInformation();
        }

        public static List<JoinChatroomResponseModel> GetPrivateChatroomsInformation(int parentChatroomId)
        {
            return GetChatroom(parentChatroomId).GetPrivateChatroomsInformation();
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

        public static List<MessageInformationModel> GetNewMessagesInformation(int parentChatroomId, int chatroomId, List<int> existingIds)
        {
            if(existingIds == null)
            {
                return GetChatroom(chatroomId, parentChatroomId).GetAllMessagesInformation();
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

        public static string GetChatroomName(int chatroomId, int parentId)
        {
            return GetChatroom(chatroomId, parentId).Name;
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

                var m = MessageService.CreateMessage(userId, chatroom.Id, message, chatroom.GetUserHandle(userId));

                chatroom.AddMessage(m);
                messageSend.IsSent = true;
                messageSend.MessageId = m.Id;
            }
            catch (Exception e)
            {
                messageSend.Errors.Add(new ErrorModel(e.ToString()));
                messageSend.IsSent = false;
            }

            return messageSend;
        }

        public static bool CreatePrivateChatroom(int parentChatroomId, int chatroomId, string chatroomName)
        {
            try
            {
                return GetChatroom(parentChatroomId).CreatePrivateChatroom(chatroomId, chatroomName); ;
            }
            catch(Exception e)
            {
                return false;
            }
        }

    }
}