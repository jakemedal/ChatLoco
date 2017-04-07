using ChatLoco.Models.Error_Model;
using ChatLoco.Models.User_Model;
using ChatLoco.Services.User_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using AutoMapper;
using ChatLoco.Services.Chatroom_Service;

namespace ChatLoco.Controllers
{
    public class UserController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Settings()
        {
            return View();
        }

        public ActionResult Create()
        {
            return View();
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult CreateUser(CreateUserRequestModel request)
        {
            //CreateUserResponseModel response = Mapper.Map<CreateUserRequestModel, CreateUserResponseModel>(request);
            var response = new CreateUserResponseModel()
            {
                Username = request.Username,
                Email = request.Email
            };

            response.Errors = UserService.CreateUser(request.Username, request.Email, request.Password); //this creates a user and returns errors if it cannot

            return Json(response);
        }

        //Is used when disconnects happen
        [HttpPost]
        public ActionResult DirtyLogout(LogoutRequestModel request)
        {
            var response = new LogoutResponseModel();

            if (request.ChatroomId != -1 && request.ParentChatroomId != -1)
            {
                ChatroomService.RemoveUserFromChatroom(request.ChatroomId, request.ParentChatroomId, request.User.Id);
            }

            UserService.Logout(request.User.Id);

            return Json(response);
        }

        [HttpPost]
        public ActionResult Logout(LogoutRequestModel request)
        {
            var response = new LogoutResponseModel();

            if(request.ChatroomId != -1 && request.ParentChatroomId != -1)
            {
                if (!ChatroomService.RemoveUserFromChatroom(request.ChatroomId, request.ParentChatroomId, request.User.Id))
                {
                    response.AddError("Could not remove user from chatroom.");
                }
            }

            if(!UserService.Logout(request.User.Id))
            {
                response.AddError("Could not logout user.");
            }

            return Json(response);
        }

        [HttpGet]
        public PartialViewResult GetLoginForm()
        {
            return PartialView("~/Views/User/_Login.cshtml");
        }
        
        [HttpPost]
        public ActionResult Login(LoginRequestModel request)
        {
            var response = UserService.GetLoginResponseModel(request);

            return Json(response);
        }
    }
}