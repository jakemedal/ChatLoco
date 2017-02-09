using ChatLoco.Models;
using ChatLoco.Models.Chatroom;
using ChatLoco.Models.Error;
using ChatLoco.Models.User;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Timers;
using System.Web;
using System.Web.Mvc;

namespace ChatLoco.Controllers
{
    public class ChatroomController : Controller
    {

        private class Chatroom
        {
            public Chatroom()
            {
                var myTimer = new Timer();
                myTimer.Elapsed += new ElapsedEventHandler(CheckUsers);
                myTimer.Interval = 2000;
                myTimer.Enabled = true;
            }

            private bool alternateCheck = true;
            private void CheckUsers(object source, ElapsedEventArgs e)
            {
                if (alternateCheck)
                {
                    foreach (var user in AllUsers)
                    {
                        var activeUser = user.Value;
                        activeUser.isActive = false;
                    }
                }
                else
                {
                    CleanUsers();
                }
                alternateCheck = !alternateCheck;
            }

            public bool HasUser(string Username)
            {
                try
                {
                    var User = AllUsers[Username];
                    return User.isActive;
                }
                catch (Exception e)
                {
                    return false;
                }
            }

            public void AddUser(string Username)
            {
                ActiveUser user = new ActiveUser() { Username = Username, isActive = true };
                AllUsers.Add(Username, user);
            }

            public void UpdateUsers(string Username)
            {
                AllUsers[Username].isActive = true;
            }

            public void CleanUsers()
            {
                List<string> removeUsers = new List<string>();
                foreach(var user in AllUsers)
                {
                    var activeUser = user.Value;
                    if (!activeUser.isActive)
                    {
                        removeUsers.Add(activeUser.Username);
                    }
                }

                foreach(var username in removeUsers)
                {
                    AllUsers.Remove(username);
                }
            }

            public int Id { get; set; }
            public List<string> AllMessages = new List<string>();
            public Dictionary<string, ActiveUser> AllUsers = new Dictionary<string, ActiveUser>();
            public string Name { get; set; }

            public class ActiveUser
            {
                public string Username { get; set; }
                public bool isActive { get; set; }
            }
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
                model.Errors.Add(new ErrorModel("Invalid chatroom parameters"));
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
            if(chatroom != null)
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
        public ActionResult GetNewMessages(UpdateChatroomModel RequestUpdate)
        {
            Chatroom chatroom = GetChatroom(RequestUpdate.ChatroomName);

            if(chatroom != null)
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
        public EmptyResult UpdateChatroomUser(string ChatroomName, string Username)
        {
            try
            {
                AllChatrooms[ChatroomName].UpdateUsers(Username);
            }
            catch(Exception e) { }

            return new EmptyResult();
        }

        [HttpPost]
        public ActionResult GetCurrentUsers(string ChatroomName)
        {
            Chatroom chatroom = GetChatroom(ChatroomName);
            if (chatroom != null)
            {
                return Json(chatroom.AllUsers.Select(user => user.Value).ToList());
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