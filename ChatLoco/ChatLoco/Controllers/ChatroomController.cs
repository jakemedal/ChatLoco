
using System;
using System.Collections.Generic;
using System.Web.Mvc;
using ChatLoco.Entities.MessageDTO;
using ChatLoco.Models.Chatroom_Service;
using ChatLoco.Services.Chatroom_Service;
using ChatLoco.Services.User_Service;
using ChatLoco.Models.Chatroom_Model;
using ChatLoco.Models.Error_Model;
using ChatLoco.Services.Message_Service;
using AutoMapper;
using ChatLoco.Entities.UserDTO;

namespace ChatLoco.Controllers
{
    public class ChatroomController : Controller
    {
        private ChatroomModelService _chatroomModelService = new ChatroomModelService();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Chat()
        {
            return View("Index");
        }
        
        [HttpPost]
        public ActionResult Chat(ChatRequestModel request)
        {
            ChatResponseModel response = new ChatResponseModel();

            if (request.ChatroomName == null || request.Username == null) 
            {
                response.Errors.Add(new ErrorModel("Invalid chatroom parameters"));
            }
            else
            {
                int chatroomId = request.ChatroomName.GetHashCode();
                int parentChatroomId = chatroomId; //temporary during initial testing

                string chatroomName = request.ChatroomName;

                if (!ChatroomService.DoesChatroomExist(chatroomId))
                {
                    ChatroomService.CreateChatroom(chatroomId, chatroomName);
                }
                
                UserDTO user = UserService.GetUser(request.Username);
                if (user == null)
                {
                    int userId = UserService.GetUniqueId();
                    user = UserService.CreateUser(userId, request.Username);
                }

                if (!ChatroomService.AddUserToChatroom(chatroomId, parentChatroomId, user.Id))
                {
                    response.AddError("Invalid user or chatroom provided.");
                }

                response.ChatroomId = chatroomId;
                response.ChatroomName = chatroomName;
                response.ParentChatroomId = parentChatroomId;
                response.Username = request.Username;
                response.UserId = user.Id;

            }
            
            return View(response);
        }
        
        [HttpPost]
        public ActionResult ComposeMessage(ComposeMessageRequestModel request)
        {
            ComposeMessageResponseModel response = new ComposeMessageResponseModel();

            try
            {
                int chatroomId = request.ChatroomId;

                MessageDTO message = MessageService.CreateMessage(request.UserId, chatroomId, request.Message);

                ChatroomService.SendMessage(message, chatroomId, request.ParentChatroomId);

                response.Id = message.Id;
            }
            catch(Exception e)
            {
                response.AddError(e.ToString());
            }

            return Json(response);
        }

        public ActionResult JoinChatroom(JoinChatroomRequestModel request)
        {
            int chatroomId = request.ChatroomId;
            int parentChatroomId = request.ParentChatroomId;
            int userId = request.UserId;

            JoinChatroomResponseModel response = new JoinChatroomResponseModel();

            if (ChatroomService.CanUserJoinChatroom(chatroomId, parentChatroomId, userId))
            {
                ChatroomService.RemoveUserFromChatroom(request.CurrentChatroomId, parentChatroomId, userId);
                ChatroomService.AddUserToChatroom(chatroomId, parentChatroomId, userId);
                response.Name = ChatroomService.GetChatroomName(chatroomId, parentChatroomId);
                response.Id = chatroomId;
            }
            else
            {
                response.AddError("Cannot access room.");
            }
            return Json(response);
        }
        
        [HttpPost]
        public ActionResult CreateChatroom(CreateChatroomRequestModel request)
        {
            CreateChatroomResponseModel response = Mapper.Map<CreateChatroomRequestModel, CreateChatroomResponseModel>(request);

            response.ChatroomId = request.ChatroomName.GetHashCode(); //TODO temporary until DB is linked up

            bool wasCreated = ChatroomService.CreatePrivateChatroom(request.ParentChatroomId, response.ChatroomId, request.ChatroomName);
            if (!wasCreated)
            {
                response.AddError("Chatroom could not be created.");
            }

            return Json(response);
        }
        
        [HttpPost]
        public ActionResult GetNewMessages(GetNewMessagesRequestModel request)
        {
            int parentChatroomId = request.ParentChatroomId;
            int chatroomId = request.ChatroomId;
            List<int> existingIds = request.ExistingMessageIds;

            GetNewMessagesResponseModel response = new GetNewMessagesResponseModel();

            response.MessagesInformation.AddRange(ChatroomService.GetNewMessagesInformation(parentChatroomId, chatroomId, existingIds));

            return Json(response);
        }
        
        [HttpPost]
        public ActionResult GetChatroomInformation(GetChatroomInformationRequestModel request)
        {
            int parentChatroomId = request.ParentChatroomId;
            int chatroomId = request.ChatroomId;
            int userId = request.UserId;

            ChatroomService.UpdateUserInChatroom(parentChatroomId, chatroomId, userId);

            GetChatroomInformationResponseModel response = new GetChatroomInformationResponseModel()
            {
                UsersInformation = ChatroomService.GetUsersInformation(parentChatroomId, chatroomId),
                PrivateChatroomsInformation = ChatroomService.GetPrivateChatroomsInformation(parentChatroomId)
            };

            return Json(response);
        }

    }
}