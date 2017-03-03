
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
using System.IO;

namespace ChatLoco.Controllers
{
    public class ChatroomController : Controller
    {
        private ChatroomModelService _chatroomModelService = new ChatroomModelService();

        public ActionResult Index()
        {
            return View(new ChatRequestModel());
        }

        [HttpGet]
        public ActionResult GetFindChatroom()
        {
            return PartialView("_FindChatroom");
        }

        public ActionResult Chat()
        {
            return View("Index");
        }
        
        [HttpPost]
        public ActionResult Chat(ChatRequestModel request)
        {
            ChatResponseModel response = new ChatResponseModel();

            int chatroomId = request.ChatroomId;
            int parentChatroomId = chatroomId; //temporary during initial testing

            string chatroomName = request.ChatroomName;

            if (!ChatroomService.DoesChatroomExist(chatroomId))
            {
                ChatroomService.CreateChatroom(chatroomId, chatroomName);
            }
                
            UserDTO user = UserService.GetUser(request.User.Id);
            if (user == null)
            {
                response.AddError("Could not find user.");
            }

            if (!ChatroomService.AddUserToChatroom(chatroomId, parentChatroomId, user.Id, request.UserHandle))
            {
                response.AddError("Invalid user or chatroom provided.");
            }

            var chatroomModel = new ChatroomModel()
            {
                ChatroomId = chatroomId,
                ChatroomName = chatroomName,
                ParentChatroomId = parentChatroomId,
                UserHandle = request.UserHandle,
                UserId = user.Id
            };

            //response.Data = PartialView("~/Views/Chatroom/_Chat.cshtml", chatroomModel);
            response.Data = RenderPartialViewToString(this.ControllerContext, "~/Views/Chatroom/_Chat.cshtml", chatroomModel);
            
            return Json(response);
        }

        //Taken from http://stackoverflow.com/questions/22098233/partialview-to-html-string
        protected string RenderPartialViewToString(ControllerContext context, string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = context.RouteData.GetRequiredString("action");

            ViewData.Model = model;

            using (StringWriter sw = new StringWriter())
            {
                ViewEngineResult viewResult = ViewEngines.Engines.FindPartialView(context, viewName);
                ViewContext viewContext = new ViewContext(context, viewResult.View, ViewData, TempData, sw);
                viewResult.View.Render(viewContext, sw);

                return sw.GetStringBuilder().ToString();
            }
        }

        [HttpPost]
        public ActionResult ComposeMessage(ComposeMessageRequestModel request)
        {
            ComposeMessageResponseModel response = new ComposeMessageResponseModel();

            try
            {
                var m = ChatroomService.SendMessage(request.Message, request.UserId, request.ChatroomId, request.ParentChatroomId);

                response.MessageId = m.MessageId;
                response.Errors.AddRange(m.Errors);
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
            string userHandle = request.UserHandle;

            JoinChatroomResponseModel response = new JoinChatroomResponseModel();

            if (ChatroomService.CanUserJoinChatroom(chatroomId, parentChatroomId, userId))
            {
                ChatroomService.RemoveUserFromChatroom(request.CurrentChatroomId, parentChatroomId, userId);
                ChatroomService.AddUserToChatroom(chatroomId, parentChatroomId, userId, userHandle);
                response.Name = ChatroomService.GetChatroomName(chatroomId, parentChatroomId);
                response.Id = chatroomId;
                response.UserHandle = userHandle;
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
            //CreateChatroomResponseModel response = Mapper.Map<CreateChatroomRequestModel, CreateChatroomResponseModel>(request);

            var response = new CreateChatroomResponseModel()
            {
                ChatroomName = request.ChatroomName,
                ParentChatroomId = request.ParentChatroomId,
                UserId = request.UserId
            };

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