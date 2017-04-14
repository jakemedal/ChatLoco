using ChatLoco.Models.AdminAction_Model;
using ChatLoco.Services.User_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatLoco.Controllers
{
    public class ManageUsersController : Controller
    {
        // GET: ManageUsers
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult HandleAdminAction(AdminActionRequestModel model)
        {
            var response = new AdminActionResponseModel();
            var action = model.Action;

            switch(action)
            {
                case ("MakeAdmin"):
                    response.Errors.AddRange(UserService.makeUserAdmin(model.Username));
                    break;
                case ("Block"):
                    response.Errors.AddRange(UserService.blockUser(model.Username));
                    break;
                case ("Unblock"):
                    response.Errors.AddRange(UserService.unblockUser(model.Username));
                    break;
            }
            if (!response.Errors.Any())
            {
                response.Message = "Action Succesfully Completed.";
            }
            return Json(response);
        }
    }
}