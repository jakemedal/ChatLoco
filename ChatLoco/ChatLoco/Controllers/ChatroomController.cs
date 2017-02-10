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

        //This is the method that constructs the ChatroomModel to be used in the Chat.cshtml file
        //This method will also create a chatroom if it does not exist
        [HttpPost]
        public ActionResult Chat(FindChatroomModel ChatroomInformation)
        {
            ChatroomModel model = new ChatroomModel();

            if (ChatroomInformation.ChatroomName == null || ChatroomInformation.Username == null) //if passed invalid parameters
            {
                model.Errors.Add(new ErrorModel("Invalid chatroom parameters"));
            }
            else
            {
                Chatroom chatroom = GetChatroom(ChatroomInformation.ChatroomName); //gets chatroom, or returns null if chatroom does not exist

                if (chatroom == null)
                {
                    //if chatroom does not exist, create new one, giving it a name
                    chatroom = new Chatroom(ChatroomInformation.ChatroomName); 
                    //AllChatrooms is a dictionary, this mean it is a list constructed of key value pairs, it operates the same as a Hashtable essentially
                    AllChatrooms.Add(chatroom.Name, chatroom); //our key is the chatroom name, and our value is the chatroom object 
                }
                
                chatroom.AddUser(ChatroomInformation.Username); //add our user to the userlist in the chatroom

                model.Name = ChatroomInformation.ChatroomName; //the chatroom name is the name variable in our model, which is used in the cshtml to display the chatroom name

                //we construct a user model to contain information pertaining to specifically the user that we will need in the Chat.cshtml page
                //The intention of this is so that it scales well when we start having more variables connected to a user
                model.UserModel = new UserModel()
                {
                    Username = ChatroomInformation.Username,
                    Id = 0
                };

            }

            //the view we return is Chat.cshtml, and we pass it the ChatroomModel we constructed
            //The ChatroomModel basically contains strings that the cshtml can use
            return View(model);
        }

        //This is the method that handles sending a message to the chatroom
        //It is an emptyresult because we are not returning anything, simply updating a list
        [HttpPost]
        public EmptyResult SendMessage(ComposedMessageModel MessageModel)
        {
            Chatroom chatroom = GetChatroom(MessageModel.ChatroomName); //get our chatroom by name
            if (chatroom != null)
            {
                if (chatroom.AllMessages.Count > 100) //put a soft cap on the total number of messages a chatroom can hold
                {
                    chatroom.AllMessages.RemoveAt(0); //remove the last message in the list if we exceed that number
                }
                string currentTime = DateTime.Now.ToString("MM/dd [h:mm:ss tt]"); //formats a string with the current date and time
                //string.Format is a method where the first argument is a string, and the following arguments correspond to the curly bracket numbers
                string message = string.Format("{0} [{1}] : {2}", currentTime, MessageModel.Username, MessageModel.Message);

                chatroom.AllMessages.Add(message); //add the formatted message to the chatroom's messages list
            }
            return new EmptyResult();
        }

        //TODO
        //this isn't finished yet
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

        //This is the method that is called every one second from the chatroom JS file
        //it basically returns the difference between two sets
        //one set is the list of messages the user has on their end
        //the other set is the list of messages the chatroom object has
        [HttpPost]
        public ActionResult GetNewMessages(GetNewMessagesModel RequestUpdate)
        {
            Chatroom chatroom = GetChatroom(RequestUpdate.ChatroomName);

            if (chatroom != null)
            {
                //if the user just joined the chatroom, they dont have any messages in their JS
                //this means we just return all the messages in the chatroom currently
                if (RequestUpdate.CurrentMessages == null)
                {
                    return Json(chatroom.AllMessages);
                }

                List<string> NewMessages = new List<string>(); //list of new messages that the user does not have in their JS

                foreach (var message in chatroom.AllMessages) //for each message in the universal set of all messages
                {
                    if (!RequestUpdate.CurrentMessages.Contains(message)) //check if the user's JS messages contains that message
                    {
                        NewMessages.Add(message); //add the message to the new messages if they dont have it
                    }
                }

                return Json(NewMessages);
            }
            else
            {
                return null;
            }
        }

        //This method is called every five seconds from the chatroom JS file by ajax request
        //This method serves two purposes.
        //It returns chatroom user information and private chatroom information
        //it also informs the chatroom that the user is still active
        [HttpPost]
        public ActionResult GetChatroomInformation(GetChatroomInformationModel ChatroomInformation)
        {
            UpdateChatroomInformationModel UpdateInformation = new UpdateChatroomInformationModel(); //this is the model that will be returned to the user

            try
            {
                //when we lookup a dictionary value directly by key value, we need to surround it in a try catch statement
                //this is needed, since if the key does not exist in the dictionary, it throws an exception
                Chatroom chatroom = AllChatrooms[ChatroomInformation.Chatroomname]; //gets chatroom by key, very fast
                chatroom.UpdateUsers(ChatroomInformation.Username); //chatroom method that simply marks the IsActive flag in a ActiveUser object as true

                //Since AllUsers is a dictionary object, it needs to be converted into a list
                //The select statement selects each object's value only, which would be the actual user object, then assembles it into a list
                //The lambda expression (user => user.Value), on the lefthand side is the object, and on the righthand side is how we construct that object
                //The variable names are temporary, used more for a visual purpose, it is a temporary binding to work with the objects we are selecting
                //before they are constructed into a list.
                //In the next lambda expression, I use the value c to demonstrate that the variable name does not matter.
                UpdateInformation.Users = chatroom.AllUsers.Select(user => user.Value).ToList(); 

                UpdateInformation.SubChatrooms = chatroom.AllSubChatrooms.Select(c => c.Value).ToList();
            }
            catch(Exception e)
            {
                UpdateInformation.Errors.Add(new ErrorModel(e.ToString()));
            }
            return Json(UpdateInformation);
        }

        //private helper method to avoid redundant code
        private Chatroom GetChatroom(string Name)
        {
            if (Name == null)
            {
                return null;
            }

            //as mentioned earlier, when looking a value up in a dictionary directly by key, it can possibly throw a exception if the key does not exist
            try
            {
                return AllChatrooms[Name];
            }
            catch (Exception e)
            {
                return null;
            }
            
        }

    }
}