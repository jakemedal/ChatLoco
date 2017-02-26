using AutoMapper;
using ChatLoco.Models.Error_Model;
using ChatLoco.Models.User_Model;
using ChatLoco.Services.User_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace ChatLoco.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        public ActionResult Index()
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
            CreateUserResponseModel response = Mapper.Map<CreateUserRequestModel, CreateUserResponseModel>(request);

            response.Errors = UserService.CreateUser(request.Username, request.Email, request.Password); //this creates a user and returns errors if it cannot

            return Json(response);
        }

        [HttpPost]
        public ActionResult Logout(LogoutRequestModel request)
        {

            return View();
        }

        [HttpGet]
        public PartialViewResult GetLoginForm()
        {
            return PartialView("~/Views/User/_Login.cshtml");
        }

        [ValidateAntiForgeryToken]
        [HttpPost]
        public ActionResult Login(LoginRequestModel request)
        {
            UserService.Login(request.Username, request.Password);
            return View();
        }
    }
}