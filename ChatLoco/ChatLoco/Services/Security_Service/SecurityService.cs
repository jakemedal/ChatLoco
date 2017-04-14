using ChatLoco.Classes.Chatroom;
using ChatLoco.Models.Chatroom_Model;
using ChatLoco.Models.Chatroom_Service;
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
        
        public static List<ErrorModel> CanUserJoinChatroom(ChatRequestModel request)
        {
            var model = new JoinChatroomRequestModel();

            if(request.RawChatroomIdValue == null)
            {
                model.ChatroomId = 0;
                model.ParentChatroomId = 0;
            }
            else
            {
                model.ChatroomId = request.RawChatroomIdValue.GetHashCode();
                model.ParentChatroomId = request.RawChatroomIdValue.GetHashCode();
            }

            model.Password = null;
            model.UserHandle = request.UserHandle;
            model.User = request.User;

            return CanUserJoinChatroom(model);
        }

        public static List<ErrorModel> CanUserJoinChatroom(JoinChatroomRequestModel request)
        {
            var errors = new List<ErrorModel>();

            Chatroom c = ChatroomService.GetChatroom(request.ChatroomId, request.ParentChatroomId);

            if (c.HasPassword && !c.CheckPasswordHash(GetStringSha256Hash(request.Password)))
            {
                errors.Add(new ErrorModel("Incorrect Password."));
            }
            else if (c.IsAtCapacity)
            {
                errors.Add(new ErrorModel("Chatroom is full."));
            }
            else if (c.IsOnBlacklist(request.User.Username))
            {
                errors.Add(new ErrorModel("User blocked from chatroom."));
            }
            else if (c.DoesHandleExist(request.UserHandle))
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