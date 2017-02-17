using ChatLoco.Classes.Chatroom;
using ChatLoco.Entities.MessageDTO;
using System;
using System.Collections.Generic;
using ChatLoco.Models.Chatroom;
using System.Linq;
using System.Web;

namespace ChatLoco.Services
{
    public static class ChatroomService
    {
        private static Dictionary<int, Chatroom> AllChatrooms = new Dictionary<int, Chatroom>();

        public static List<UserInformationModel> GetUsersInformation(int parentChatroomId, int chatroomId)
        {
            return GetChatroom(chatroomId, parentChatroomId).GetUsersInformation();
        }

        public static List<PrivateChatroomInformationModel> GetPrivateChatroomsInformation(int parentChatroomId)
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
                Chatroom c = AllChatrooms[id];
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static bool CanUserJoinChatroom(int chatroomId, int parentId, int userId)
        {
            Chatroom c = GetChatroom(chatroomId, parentId);

            return c == null ? false : c.CanUserJoinChatroom(userId);
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

        public static bool AddUserToChatroom(int chatroomId, int parentId, int userId)
        {
            try
            {
                GetChatroom(chatroomId, parentId).AddUser(UserService.GetUser(userId));
                return true;
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
                AllChatrooms.Add(id, c);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        private static Chatroom GetChatroom(int parentChatroomId)
        {
            try
            {
                return AllChatrooms[parentChatroomId];
            }
            catch(Exception e)
            {
                return null;
            }
        }

        private static Chatroom GetChatroom(int chatroomId, int parentId)
        {
            try
            {
                if(chatroomId != parentId)
                {
                    return AllChatrooms[parentId].GetPrivateChatroom(chatroomId);
                }
                else
                {
                    return AllChatrooms[parentId];
                }
            }
            catch(Exception e)
            {
                return null;
            }
        }

        public static bool SendMessage(MessageDTO message, int chatroomId, int parentId)
        {
            try
            {
                GetChatroom(chatroomId, parentId).AddMessage(message);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

        public static bool CreatePrivateChatroom(int parentChatroomId, int chatroomId, string chatroomName)
        {
            try
            {
                GetChatroom(parentChatroomId).CreatePrivateChatroom(chatroomId, chatroomName);
                return true;
            }
            catch(Exception e)
            {
                return false;
            }
        }

    }
}