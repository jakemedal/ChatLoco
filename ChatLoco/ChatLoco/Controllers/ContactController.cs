using ChatLoco.Models.Email_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;
using ChatLoco.Services.SendMail_Service;

namespace ChatLoco.Controllers
{
    public class ContactController : Controller
    {
        
        [HttpPost]
        public ActionResult Send(SendRequestModel request)
        {
            var response = new SendResponseModel();
            //try to send email and make appropriate json object if not. 

            response.Errors.AddRange(SendMailService.SendMail(request));

            if (!response.Errors.Any())
            {
                response.Message = "Email sent successfully. We will respond ASAP!</p><br><p>You will be redirected to the home page shortly.";
            }

            return Json(response);
                //? Json(new { status = "success", Message = "<p>Email sent successfully. We will respond ASAP!</p><br><p>You will be redirected to the home page shortly.</p><br>" })
                //: Json(new { status = "error", Message = "<p>Error sending email.</p><br><p> Please make sure your information is correct.</p>" });
        }
    }

}