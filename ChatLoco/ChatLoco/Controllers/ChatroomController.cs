using ChatLoco.Models;
using ChatLoco.Models.Chatroom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace ChatLoco.Controllers
{
    public class ChatroomController : Controller
    {
        // GET: Chatroom
        public ActionResult Index(string ChatroomName, string Username)
        {
            if(ChatroomName == null)
            {
                return View(new ChatroomModel() { Error = "Invalid chatroom parameter" });
            }
            ChatroomModel model = new ChatroomModel()
            {
                Name = ChatroomName,
                Username = Username
            };
            return View(model);
        }

        [HttpPost]
        public EmptyResult SendMessage(ComposedMessageModel ComposedMessage)
        {
            return new EmptyResult();
        }
    }
}