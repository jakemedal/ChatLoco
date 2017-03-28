using ChatLoco.Models.Email_Model;
using ChatLoco.Services.Email_Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Net.Mail;

namespace ChatLoco.Controllers
{
    public class ContactController : Controller
    {
        
        [HttpPost]
        public ActionResult send(EmailModel contact)
        {
            //try to send email and make appropriate json object if not. 
            return ChatLoco.Services.Email_Service.EmailService.sendMail(contact)
                ? Json(new { status = "success", Message = "<p>Email sent successfully. We will respond ASAP!</p><br><p>You will be redirected to the home page shortly.</p><br>" })
                : Json(new { status = "error", Message = "<p>Error sending email.</p><br><p> Please make sure your information is correct.</p>" });
        }
    }

}