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
    }
}