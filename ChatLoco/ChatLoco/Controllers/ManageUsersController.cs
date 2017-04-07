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
            //currently only have make admin ability. User Locking is not functional.
            
                response.Errors.AddRange(UserService.makeUserAdmin(model.Username));
            

            if (!response.Errors.Any())
            {
                response.Message = "Action Succesfully Completed.";
            }

            return Json(response);
        }
    }
}