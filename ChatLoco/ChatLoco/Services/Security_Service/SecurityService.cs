using ChatLoco.Classes.Chatroom;
using ChatLoco.Models.Error_Model;
using ChatLoco.Services.Chatroom_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ChatLoco.Services.Security_Service
{
    public static class SecurityService
    {
        
        public static List<ErrorModel> CanUserJoinChatroom(int chatroomId, int parentId, int userId, string userHandle)
        {
            var errors = new List<ErrorModel>();

            Chatroom c = ChatroomService.GetChatroom(chatroomId, parentId);

            if (c.DoesHandleExist(userHandle))
            {
                errors.Add(new ErrorModel("User Handle already exists in Chatroom."));
            }

            return errors;
        }

        //Found at http://stackoverflow.com/questions/3984138/hash-string-in-c-sharp
        internal static string GetStringSha256Hash(string text)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            using (var sha = new System.Security.Cryptography.SHA256Managed())
            {
                byte[] textData = System.Text.Encoding.UTF8.GetBytes(text);
                byte[] hash = sha.ComputeHash(textData);
                return BitConverter.ToString(hash).Replace("-", String.Empty);
            }
        }
    }
}