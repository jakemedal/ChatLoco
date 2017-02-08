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

        private class Chatroom
        {
            public List<string> AllMessages = new List<string>();
            public string Name { get; set; }
        }
       
        static Dictionary<string, Chatroom> AllChatrooms = new Dictionary<string, Chatroom>();
        
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Chat()
        {
            return View("Index");
        }

        [HttpPost]
        public ActionResult Chat(FindChatroomModel ChatroomInformation)
        {
            ChatroomModel model = new ChatroomModel();

            if (ChatroomInformation.ChatroomName == null || ChatroomInformation.Username == null)
            {
                model.Error = "Invalid chatroom parameters";
            }
            else
            {
                Chatroom chatroom = new Chatroom();
                AllChatrooms.TryGetValue(ChatroomInformation.ChatroomName, out chatroom);

                if(chatroom == null)
                {
                    chatroom = new Chatroom()
                    {
                        Name = ChatroomInformation.ChatroomName
                    };
                    AllChatrooms.Add(chatroom.Name, chatroom);
                }

                model.Name = ChatroomInformation.ChatroomName;
                model.Username = ChatroomInformation.Username;
            }
            return View(model);
        }

        [HttpPost]
        public EmptyResult SendMessage(ComposedMessageModel MessageModel)
        {
            Chatroom chatroom = GetChatroom(MessageModel.ChatroomName);
            if(chatroom != null)
            {
                if (chatroom.AllMessages != null)
                {
                    if (chatroom.AllMessages.Count > 100)
                    {
                        chatroom.AllMessages.RemoveAt(0);
                    }
                    chatroom.AllMessages.Add(MessageModel.Username + ": " + MessageModel.Message);
                }
            }
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult GetNewMessages(string ChatroomName, List<string> CurrentMessages)
        {
            Chatroom chatroom = GetChatroom(ChatroomName);

            if(chatroom != null)
            {
                if (CurrentMessages == null)
                {
                    return Json(chatroom.AllMessages);
                }

                List<string> NewMessages = new List<string>();

                foreach (var message in chatroom.AllMessages)
                {
                    if (!CurrentMessages.Contains(message))
                    {
                        NewMessages.Add(message);
                    }
                }

                return Json(NewMessages);
            }
            else
            {
                return null;
            }
        }

        private Chatroom GetChatroom(string Name)
        {
            if(Name == null)
            {
                return null;
            }

            Chatroom chatroom = new Chatroom();
            AllChatrooms.TryGetValue(Name, out chatroom);

            return chatroom;
        }

    }
}