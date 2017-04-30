
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
using ChatLoco.Services.Security_Service;
using ChatLoco.Models.PartialView_Model;

namespace ChatLoco.Controllers
{
    public class ChatroomController : Controller
    {
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
            var response = new PartialViewModel();

            var user = UserService.GetUser(request.User.Id);
            if(user == null || user.Role == Models.User_Model.RoleLevel.Blocked)
            {
                response.Logout = true;
                return Json(response);
            }

            int chatroomId = 0;
            if(request.RawChatroomIdValue != null)
            {
                chatroomId = request.RawChatroomIdValue.GetHashCode();
            }
            int parentChatroomId = chatroomId; //temporary during initial testing

            string chatroomName = request.ChatroomName;

            if (!ChatroomService.DoesChatroomExist(chatroomId))
            {
                ChatroomService.CreateChatroom(chatroomId, chatroomName);
            }

            var joinErrors = SecurityService.CanUserJoinChatroom(request);
            response.Errors.AddRange(joinErrors);

            if(joinErrors.Count == 0)
            {
                if (!ChatroomService.AddUserToChatroom(chatroomId, parentChatroomId, request.User.Id, request.UserHandle))
                {
                    response.AddError("Error adding user into chatroom.");
                }
            }

            var chatroomModel = new ChatroomModel()
            {
                ChatroomId = chatroomId,
                ChatroomName = chatroomName,
                ParentChatroomId = parentChatroomId,
                UserHandle = request.UserHandle,
                UserId = request.User.Id
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

            var user = UserService.GetUser(request.UserId);

            if (user == null || user.Role == Models.User_Model.RoleLevel.Blocked)
            {
                response.Logout = true;
            }
            else
            {
                try
                {
                    var m = ChatroomService.SendMessage(request);

                    response.MessageId = m.MessageId;
                    response.Errors.AddRange(m.Errors);
                }
                catch (Exception e)
                {
                    response.AddError(e.ToString());
                }
            }
            
            return Json(response);
        }

        [HttpPost]
        public ActionResult GetJoinChatroomForm(GetJoinChatroomFormRequestModel request)
        {
            var response = new PartialViewModel();

            var model = new GetJoinChatroomFormResponseModel();

            model.HasPassword = ChatroomService.HasPassword(request.ParentChatroomId, request.ChatroomId);
            model.ChatroomName = ChatroomService.GetChatroomName(request.ParentChatroomId, request.ChatroomId);

            model.NewChatroomId = request.ChatroomId;

            response.Data = RenderPartialViewToString(this.ControllerContext, "~/Views/Chatroom/_JoinChatroomForm.cshtml", model);

            return Json(response);
        }

        public ActionResult JoinChatroom(JoinChatroomRequestModel request)
        {
            int chatroomId = request.ChatroomId;
            int parentChatroomId = request.ParentChatroomId;
            int userId = request.UserId;
            string userHandle = request.UserHandle;

            JoinChatroomResponseModel response = new JoinChatroomResponseModel();

            var joinErrors = SecurityService.CanUserJoinChatroom(request);
            response.Errors.AddRange(joinErrors);

            if (joinErrors.Count == 0)
            {
                ChatroomService.RemoveUserFromChatroom(request.CurrentChatroomId, parentChatroomId, userId);
                ChatroomService.AddUserToChatroom(chatroomId, parentChatroomId, userId, userHandle);
                response.Name = ChatroomService.GetChatroomName(parentChatroomId, chatroomId);
                response.Id = chatroomId;
                response.UserHandle = userHandle;
            }

            return Json(response);
        }
        
        [HttpPost]
        public EmptyResult LeaveChatroom(LeaveChatroomRequestModel request)
        {
            ChatroomService.RemoveUserFromChatroom(request.ChatroomId, request.ParentId, request.UserId);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult CreateChatroom(CreateChatroomRequestModel request)
        {
            var response = new CreateChatroomResponseModel()
            {
                ChatroomName = request.ChatroomName,
                ParentChatroomId = request.ParentChatroomId,
                UserId = request.User.Id
            };

            response.ChatroomId = request.ChatroomName.GetHashCode(); //TODO temporary until DB is linked up

            var errors = ChatroomService.CreatePrivateChatroom(request);

            response.Errors.AddRange(errors);

            return Json(response);
        }
        
        [HttpPost]
        public ActionResult GetNewMessages(GetNewMessagesRequestModel request)
        {
            int parentChatroomId = request.ParentChatroomId;
            int chatroomId = request.ChatroomId;
            List<int> existingIds = request.ExistingMessageIds;

            GetNewMessagesResponseModel response = new GetNewMessagesResponseModel();

            var user = UserService.GetUser(request.UserId);

            if (user == null || user.Role == Models.User_Model.RoleLevel.Blocked)
            {
                response.Logout = true;
            }
            else
            {
                var messageInformationModels = ChatroomService.GetNewMessagesInformation(parentChatroomId, chatroomId, existingIds);

                foreach (var messageInformationModel in messageInformationModels)
                {
                    if (messageInformationModel.IntendedForUserId != -1)
                    {
                        if (messageInformationModel.IntendedForUserId == request.UserId)
                        {
                            response.MessagesInformation.Add(messageInformationModel);
                        }
                    }
                    else
                    {
                        response.MessagesInformation.Add(messageInformationModel);
                    }
                }
            }
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