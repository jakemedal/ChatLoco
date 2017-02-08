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

        static List<string> AllMessages = new List<string>();

        // URL Usage : /Chatroom/Index?ChatroomName=<chat name here>&Username=<username here>
        //The url takes two parameters, a chatroom name and a username
        //if your chatroom name is TestChatroom and your username is TestUsername1, then the URL would be
        ///Chatroom/Index?ChatroomName=<TestChatroom>&Username=<TestUsername1>
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
        public EmptyResult SendMessage(string Message, string Username)
        {
            if(AllMessages.Count > 100)
            {
                AllMessages.RemoveAt(0);
            }
            AllMessages.Add(Username + ": " + Message);
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult GetNewMessages(List<string> CurrentMessages)
        {
            if(CurrentMessages == null)
            {
                return Json(AllMessages);
            }

            List<string> NewMessages = new List<string>();

            foreach(var message in AllMessages)
            {
                if (!CurrentMessages.Contains(message))
                {
                    NewMessages.Add(message);
                }
            }

            return Json(NewMessages);
        }
    }
}