using ChatLoco.Classes.Chatroom;
using ChatLoco.Models;
using ChatLoco.Models.Chatroom;
using ChatLoco.Models.Error;
using ChatLoco.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web.Mvc;

namespace ChatLoco.Controllers
{
    public class ChatroomController : Controller
    {
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
                model.Errors.Add(new ErrorModel("Invalid chatroom parameters"));
            }
            else
            {
                Chatroom chatroom = new Chatroom("");
                AllChatrooms.TryGetValue(ChatroomInformation.ChatroomName, out chatroom);

                if (chatroom == null)
                {
                    chatroom = new Chatroom(ChatroomInformation.ChatroomName);
                    AllChatrooms.Add(chatroom.Name, chatroom);
                }

                if (!chatroom.HasUser(ChatroomInformation.Username))
                {
                    chatroom.AddUser(ChatroomInformation.Username);
                }

                model.Name = ChatroomInformation.ChatroomName;

                model.UserModel = new UserModel()
                {
                    Username = ChatroomInformation.Username,
                    Id = 0
                };

            }
            return View(model);
        }

        [HttpPost]
        public EmptyResult SendMessage(ComposedMessageModel MessageModel)
        {
            Chatroom chatroom = GetChatroom(MessageModel.ChatroomName);
            if (chatroom != null)
            {
                if (chatroom.AllMessages != null)
                {
                    if (chatroom.AllMessages.Count > 100)
                    {
                        chatroom.AllMessages.RemoveAt(0);
                    }
                    string currentTime = DateTime.Now.ToString("MM/dd [h:mm:ss tt]");
                    string message = string.Format("{0} [{1}] : {2}", currentTime, MessageModel.Username, MessageModel.Message);

                    chatroom.AllMessages.Add(message);
                }
            }
            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult CreateSubChatroom(string SubChatroomName, string ChatroomName, string Username)
        {
            try
            {
                AllChatrooms[ChatroomName].AllSubChatrooms.Add(SubChatroomName, new Chatroom(SubChatroomName));
                return Json(true);
            }
            catch(Exception e){ }
            return Json(false);
        }

        [HttpPost]
        public ActionResult GetNewMessages(GetNewMessagesModel RequestUpdate)
        {
            Chatroom chatroom = GetChatroom(RequestUpdate.ChatroomName);

            if (chatroom != null)
            {
                if (RequestUpdate.CurrentMessages == null)
                {
                    return Json(chatroom.AllMessages);
                }

                List<string> NewMessages = new List<string>();

                foreach (var message in chatroom.AllMessages)
                {
                    if (!RequestUpdate.CurrentMessages.Contains(message))
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

        [HttpPost]
        public ActionResult GetChatroomInformation(GetChatroomInformationModel ChatroomInformation)
        {
            UpdateChatroomInformationModel UpdateInformation = new UpdateChatroomInformationModel();

            try
            {
                Chatroom chatroom = AllChatrooms[ChatroomInformation.Chatroomname];
                chatroom.UpdateUsers(ChatroomInformation.Username);

                UpdateInformation.Users = chatroom.AllUsers.Select(user => user.Value).ToList();

                UpdateInformation.SubChatrooms = chatroom.AllSubChatrooms.Select(c => c.Value).ToList();
            }
            catch(Exception e)
            {
                UpdateInformation.Errors.Add(new ErrorModel(e.ToString()));
            }
            return Json(UpdateInformation);
        }

        private Chatroom GetChatroom(string Name)
        {
            if (Name == null)
            {
                return null;
            }

            Chatroom chatroom = new Chatroom("");
            AllChatrooms.TryGetValue(Name, out chatroom);

            return chatroom;
        }

    }
}