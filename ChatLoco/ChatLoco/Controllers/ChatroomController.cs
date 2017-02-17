using ChatLoco.Models;
using ChatLoco.Models.Chatroom;
using ChatLoco.Models.Error;
using ChatLoco.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using ChatLoco.Services;

namespace ChatLoco.Controllers
{
    public class ChatroomController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Chat()
        {
            return View("Index");
        }
        
        [HttpPost]
        public ActionResult Chat(FindChatroomModel ChatroomInformation)
        {
            ChatroomModel model = new ChatroomModel();

            if (ChatroomInformation.ChatroomName == null || ChatroomInformation.Username == null) 
            {
                model.Errors.Add(new ErrorModel("Invalid chatroom parameters"));
            }
            else
            {
                int userId = ChatroomInformation.Username.GetHashCode();
                int chatroomId = ChatroomInformation.ChatroomName.GetHashCode();
                int parentChatroomId = chatroomId; //temporary during initial testing

                string chatroomName = ChatroomInformation.ChatroomName;

                if (!ChatroomService.DoesChatroomExist(chatroomId))
                {
                    ChatroomService.CreateChatroom(chatroomId, chatroomName);
                }

                if(UserService.GetUser(userId) == null)
                {
                    UserService.CreateUser(userId, ChatroomInformation.Username);
                }

                if (!ChatroomService.AddUserToChatroom(chatroomId, parentChatroomId, userId))
                {
                    model.Errors.Add(new ErrorModel("Invalid user or chatroom provided."));
                }

                model.ChatroomId = chatroomId;
                model.ChatroomName = chatroomName;
                model.ParentChatroomId = parentChatroomId;
                model.Username = ChatroomInformation.Username;
                model.UserId = userId;

            }
            
            return View(model);
        }
        
        [HttpPost]
        public ActionResult SendMessage(ComposedMessageModel MessageModel)
        {
            try
            {
                int userId = MessageModel.UserId;
                int chatroomId = MessageModel.ChatroomId;
                int parentChatroomId = MessageModel.ParentChatroomId;
                string rawMessage = MessageModel.Message;

                ChatroomService.SendMessage(MessageService.CreateMessage(userId, chatroomId, rawMessage), MessageModel.ChatroomId, MessageModel.ParentChatroomId);
                return new EmptyResult();
            }
            catch(Exception e)
            {
                return Json(e);
            }
        }

        public ActionResult JoinChatroom(JoinChatroomModel joinChatroomRequest)
        {
            int chatroomId = joinChatroomRequest.ChatroomId;
            int parentChatroomId = joinChatroomRequest.ParentChatroomId;
            int userId = joinChatroomRequest.UserId;
            int currentChatroomId = joinChatroomRequest.CurrentChatroomId;

            PrivateChatroomInformationModel model = new PrivateChatroomInformationModel();

            if (ChatroomService.CanUserJoinChatroom(chatroomId, parentChatroomId, userId))
            {
                ChatroomService.RemoveUserFromChatroom(currentChatroomId, parentChatroomId, userId);
                ChatroomService.AddUserToChatroom(chatroomId, parentChatroomId, userId);
                model.Name = ChatroomService.GetChatroomName(chatroomId, parentChatroomId);
                model.Id = chatroomId;
            }
            else
            {
                model.AddError("Cannot access room.");
            }
            return Json(model);
        }
        
        [HttpPost]
        public ActionResult CreateSubChatroom(string subChatroomName, int parentChatroomId, int userId)
        {
            int subChatroomId = subChatroomName.GetHashCode();

            bool wasCreated = ChatroomService.CreatePrivateChatroom(parentChatroomId, subChatroomId, subChatroomName);

            return wasCreated ? Json(subChatroomId) : Json(-1) ;
        }
        
        [HttpPost]
        public ActionResult GetNewMessages(GetNewMessagesModel RequestUpdate)
        {
            int parentChatroomId = RequestUpdate.ParentChatroomId;
            int chatroomId = RequestUpdate.ChatroomId;
            List<int> existingIds = RequestUpdate.ExistingMessageIds;

            return Json(ChatroomService.GetNewMessagesInformation(parentChatroomId, chatroomId, existingIds));
        }
        
        [HttpPost]
        public ActionResult GetChatroomInformation(GetChatroomInformationModel ChatroomInformation)
        {
            int parentChatroomId = ChatroomInformation.ParentChatroomId;
            int chatroomId = ChatroomInformation.ChatroomId;
            int userId = ChatroomInformation.UserId;

            ChatroomService.UpdateUserInChatroom(parentChatroomId, chatroomId, userId);

            UpdateChatroomInformationModel UpdateInformation = new UpdateChatroomInformationModel()
            {
                UsersInformation = ChatroomService.GetUsersInformation(parentChatroomId, chatroomId),
                PrivateChatroomsInformation = ChatroomService.GetPrivateChatroomsInformation(parentChatroomId)
            };

            return Json(UpdateInformation);
        }

    }
}