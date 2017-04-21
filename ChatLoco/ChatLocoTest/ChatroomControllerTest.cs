using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChatLoco.DAL;
using ChatLoco.Models.User_Model;
using ChatLoco.Controllers;
using System.Collections.Generic;
using ChatLoco.Services.User_Service;
using ChatLoco.Models.Chatroom_Model;
using ChatLoco.Models.PartialView_Model;
using ChatLoco.Services.Chatroom_Service;
using ChatLoco.Services.Security_Service;
using ChatLoco.Models.Response_Model;
using System.IO;
using System.Web.Mvc;
using ChatLoco.Models.Chatroom_Service;
using ChatLoco.Services.Message_Service;

namespace ChatLocoTest
{
    [TestClass]
    public class ChatroomControllerTest
    {
        /* --This section is run everytime before any of the tests-- */
        /* --ensures the database doesn't have left over data from previous tests so that each time we run the tests the data is consistent--*/
        /*###########################################################################################################################################*/

        //Need a reference to the db context
        private ChatLocoContext _db = new ChatLocoContext();
        //TestHelper that will hold all of the created users
        private TestHelper _helper;

        /*constructor - create an instance of TestHelper passing in the users we want to use for testing*/
        public ChatroomControllerTest()
        {

            List<UserTestModel> newUsers = new List<UserTestModel>();
            newUsers.Add(new UserTestModel() { Username = "test_chatroomcontroller_00", Password = "testchatroomcontroller00", Email = "test_chatroomcontroller_00@test.com", Id = -1, DefaultHandle = "test_chatroomcontroller_00" });
            newUsers.Add(new UserTestModel() { Username = "test_chatroomcontroller_01", Password = "testchatroomcontroller01", Email = "test_chatroomcontroller_01@test.com", Id = -1, DefaultHandle = "test_chatroomcontroller_01" });

            _helper = new TestHelper(newUsers, new List<int>());
        }



        /* --This section includes default View functions-- */
/*###########################################################################################################################################*/
        
        /*Call the Index method: The Index view*/
        [TestMethod]
        public void TestIndexView()
        {
            ChatroomController chatControllerTest = new ChatroomController();
            var result = chatControllerTest.Index() as ViewResult;
            Assert.IsInstanceOfType(result.ViewData.Model, typeof(ChatRequestModel));
        }

        /*Call the Chat method: The Chat view with name Index*/
        [TestMethod]
        public void TestChatView()
        {
            ChatroomController chatControllerTest = new ChatroomController();
            var result = chatControllerTest.Chat() as ViewResult;
            Assert.AreEqual("Index", result.ViewName);
        }


        /* --This section includes Partial View functions-- */
/*###########################################################################################################################################*/

        /*Call the GetLoginForm method: The partial view _FindChatroom.cshtml*/
        [TestMethod]
        public void TestGetFindChatroom()
        {
            ChatroomController chatControllerTest = new ChatroomController();
            var result = chatControllerTest.GetFindChatroom() as PartialViewResult;
            Assert.AreEqual("_FindChatroom", result.ViewName);
        }


        /* --This section is the fun stuff. Begin testing database functions-- */
/*###########################################################################################################################################*/
        /*NOTE - cannot test GetJoinChatroomForm and Chat due to MVC creating Controller context's under the hood
         However, the logic that is done within these has been abstracted to the TestHelper class so fear not. Just call
         then through there*/


        //TODO: possibly extract chatroom data into helper just like testUsers
        /*Call the GetChatroomInformation method: given that a chatroom exists, it will return information on that chatroom.
         * since a chatroom ID is needed for a parameter its assumed the chatroom exists at this point */
        [TestMethod]
        public void TestGetChatroomInformation()
        {
            //Necessary to create the chatroom before requesting its information
            var USER = _helper.testUsers[1];
            ChatroomController chatControllerTest = new ChatroomController();
            ChatRequestModel model = new ChatRequestModel()
            {
                RawChatroomIdValue = "123400",
                UserHandle = USER.DefaultHandle,
                ChatroomName = "TestGetChatroomInformation",
                User = new UserModel() { Id = USER.Id, Username = USER.Username}
            };
            ChatResponseTestModel chatRoom = _helper.createChatroomAndAddUser(model);//for now don't do anything with the result

            //Create test data model
            GetChatroomInformationRequestModel model2 = new GetChatroomInformationRequestModel()
            {
                UserId = chatRoom.ChatroomModel.UserId,
                ChatroomId = chatRoom.ChatroomModel.ChatroomId,
                ParentChatroomId = chatRoom.ChatroomModel.ParentChatroomId
            };

            //Test getting the information from the chatroom success
            var result = chatControllerTest.GetChatroomInformation(model2) as JsonResult;
            Assert.AreEqual(1, ((GetChatroomInformationResponseModel)result.Data).UsersInformation.Count);
            Assert.AreEqual(USER.Id, ((GetChatroomInformationResponseModel)result.Data).UsersInformation[0].Id);
            Assert.AreEqual(USER.Username, ((GetChatroomInformationResponseModel)result.Data).UsersInformation[0].Username);
        }


        /*Call the CreateChatroom method (creates a private chatroom - misleading title lol): A new private chatroom will be created */
        [TestMethod]
        public void TestCreateChatroom()
        {
            //Necessary to create the child chatroom before requesting its information
            var USER = _helper.testUsers[1];
            ChatroomController chatControllerTest = new ChatroomController();
            ChatRequestModel model = new ChatRequestModel()
            {
                RawChatroomIdValue = "123401",
                UserHandle = USER.DefaultHandle,
                ChatroomName = "TestCreateChatroom",
                User = new UserModel() { Id = USER.Id, Username = USER.Username }
            };
            ChatResponseTestModel chatRoom = _helper.createChatroomAndAddUser(model);//for now don't do anything with the result

            //Create test data model - with all options possible since without them is easier. Do hard mode xD
            CreateChatroomRequestModel model2 = new CreateChatroomRequestModel()
            {
                ChatroomName = ("PRIVATE" + chatRoom.ChatroomModel.ChatroomName),
                ParentChatroomId = chatRoom.ChatroomModel.ChatroomId,
                Blacklist = "test1,test2",
                Password = "test",
                Capacity = 30,
                User = new UserModel() { Id = USER.Id}
            };

            //Test creating a private chatroom success
            var result = chatControllerTest.CreateChatroom(model2) as JsonResult;
            Assert.AreEqual(0, ((CreateChatroomResponseModel)result.Data).Errors.Count);
            Assert.AreEqual(("PRIVATE" + chatRoom.ChatroomModel.ChatroomName), ((CreateChatroomResponseModel)result.Data).ChatroomName);
            Assert.AreEqual(chatRoom.ChatroomModel.ChatroomId, ((CreateChatroomResponseModel)result.Data).ParentChatroomId);
            Assert.AreEqual(USER.Id, ((CreateChatroomResponseModel)result.Data).UserId);

            //Test trying to create duplicate named private chatroom fail
            result = chatControllerTest.CreateChatroom(model2) as JsonResult;
            Assert.AreNotEqual(0, ((CreateChatroomResponseModel)result.Data).Errors.Count);
            Assert.AreEqual("A private chatroom with this name already exists.", ((CreateChatroomResponseModel)result.Data).Errors[0].ErrorMessage);
        }


        /*Call the JoinChatroom method: The user will be removed from the current chatroom and added to the new one */
        [TestMethod]
        public void TestJoinChatroom()
        {
            //Necessary to create the child chatroom before requesting its information
            var USER = _helper.testUsers[1];
            var JOINING_USER = _helper.testUsers[0];
            ChatroomController chatControllerTest = new ChatroomController();
            ChatRequestModel model = new ChatRequestModel()
            {
                RawChatroomIdValue = "123402",
                UserHandle = USER.DefaultHandle,
                ChatroomName = "TestJoinChatroom",
                User = new UserModel() { Id = USER.Id, Username = USER.Username }
            };
            ChatResponseTestModel chatRoom = _helper.createChatroomAndAddUser(model);//for now don't do anything with the result

            //Pre-sanity check amke sure only 1 user in the room before we try ad join
            //Create test data model
            GetChatroomInformationRequestModel modelInfo = new GetChatroomInformationRequestModel()
            {
                UserId = chatRoom.ChatroomModel.UserId,
                ChatroomId = chatRoom.ChatroomModel.ChatroomId,
                ParentChatroomId = chatRoom.ChatroomModel.ParentChatroomId
            };

            //Test getting the information from the chatroom success
            var result = chatControllerTest.GetChatroomInformation(modelInfo) as JsonResult;
            Assert.AreEqual(1, ((GetChatroomInformationResponseModel)result.Data).UsersInformation.Count);
            Assert.AreEqual(USER.Id, ((GetChatroomInformationResponseModel)result.Data).UsersInformation[0].Id);
            Assert.AreEqual(USER.Username, ((GetChatroomInformationResponseModel)result.Data).UsersInformation[0].Username);

            //Create test data model - with all options possible since without them is easier. Do hard mode xD
            JoinChatroomRequestModel model2 = new JoinChatroomRequestModel()
            {
                UserId = JOINING_USER.Id,
                ChatroomId = chatRoom.ChatroomModel.ChatroomId,
                ParentChatroomId = chatRoom.ChatroomModel.ParentChatroomId,
                CurrentChatroomId = -1,
                UserHandle = JOINING_USER.DefaultHandle,
                Password = "",
                User = new UserModel() { Id = JOINING_USER.Id }
            };

            //Test creating a private chatroom success
            result = chatControllerTest.JoinChatroom(model2) as JsonResult;

            //Post-sanity check NOW there will be two users in the chatroom xD
            result = chatControllerTest.GetChatroomInformation(modelInfo) as JsonResult;
            Assert.AreEqual(2, ((GetChatroomInformationResponseModel)result.Data).UsersInformation.Count);
            Assert.AreEqual(USER.Id, ((GetChatroomInformationResponseModel)result.Data).UsersInformation[0].Id);
            Assert.AreEqual(USER.Username, ((GetChatroomInformationResponseModel)result.Data).UsersInformation[0].Username);
            Assert.AreEqual(JOINING_USER.Id, ((GetChatroomInformationResponseModel)result.Data).UsersInformation[1].Id);
            Assert.AreEqual(JOINING_USER.Username, ((GetChatroomInformationResponseModel)result.Data).UsersInformation[1].Username);
        }


        /*Call the LeaveChatroom method: The user will be removed from the current chatroom */
        [TestMethod]
        public void TestLeaveChatroom()
        {
            //Necessary to create the child chatroom before requesting its information
            var USER = _helper.testUsers[1];
            ChatroomController chatControllerTest = new ChatroomController();
            ChatRequestModel model = new ChatRequestModel()
            {
                RawChatroomIdValue = "123403",
                UserHandle = USER.DefaultHandle,
                ChatroomName = "TestLeaveChatroom",
                User = new UserModel() { Id = USER.Id, Username = USER.Username }
            };
            ChatResponseTestModel chatRoom = _helper.createChatroomAndAddUser(model);//for now don't do anything with the result

            //Pre-sanity check amke sure only 1 user in the room before we try ad join
            //Create test data model
            GetChatroomInformationRequestModel modelInfo = new GetChatroomInformationRequestModel()
            {
                UserId = chatRoom.ChatroomModel.UserId,
                ChatroomId = chatRoom.ChatroomModel.ChatroomId,
                ParentChatroomId = chatRoom.ChatroomModel.ParentChatroomId
            };

            //Test getting the information from the chatroom success
            var result = chatControllerTest.GetChatroomInformation(modelInfo) as JsonResult;
            Assert.AreEqual(1, ((GetChatroomInformationResponseModel)result.Data).UsersInformation.Count);
            Assert.AreEqual(USER.Id, ((GetChatroomInformationResponseModel)result.Data).UsersInformation[0].Id);
            Assert.AreEqual(USER.Username, ((GetChatroomInformationResponseModel)result.Data).UsersInformation[0].Username);


            //Create test data model
            LeaveChatroomRequestModel model2 = new LeaveChatroomRequestModel()
            {
                ChatroomId = chatRoom.ChatroomModel.ChatroomId,
                ParentId = chatRoom.ChatroomModel.ParentChatroomId,
                UserId = USER.Id,
                User = new UserModel() { Id = USER.Id}
            };

            var result2 = chatControllerTest.LeaveChatroom(model2) as EmptyResult;
            //nothing to assert since an empty object is returned but check below that there are no users in the room

            //Post-sanity check NOW there will be two users in the chatroom xD
            result = chatControllerTest.GetChatroomInformation(modelInfo) as JsonResult;
            Assert.AreEqual(0, ((GetChatroomInformationResponseModel)result.Data).UsersInformation.Count);
        }


        /*Call the Compose AND GetNewMessages methods: Both need eachother to work, so two bird with one stone. Message will be created, store, and then messages for this chatroom will be returned */
        [TestMethod]
        public void TestComposeAndGetNewMessages()
        {
            //Necessary to create the child chatroom before requesting its information
            var USER = _helper.testUsers[1];
            ChatroomController chatControllerTest = new ChatroomController();
            ChatRequestModel model = new ChatRequestModel()
            {
                RawChatroomIdValue = "123404",
                UserHandle = USER.DefaultHandle,
                ChatroomName = "TestComposeAndGetNewMessages",
                User = new UserModel() { Id = USER.Id, Username = USER.Username }
            };
            ChatResponseTestModel chatRoom = _helper.createChatroomAndAddUser(model);//for now don't do anything with the result

            //Right now every time we run the test another message gets added.
            ComposeMessageRequestModel messageModel = new ComposeMessageRequestModel()
            {
                Message = "This is a test message",
                ChatroomId = chatRoom.ChatroomModel.ChatroomId,
                UserId = USER.Id,
                ParentChatroomId = chatRoom.ChatroomModel.ParentChatroomId,
                UserHandle = USER.DefaultHandle
            };

            var msgResult = chatControllerTest.ComposeMessage(messageModel);

            //Create test data model
            GetNewMessagesRequestModel model2 = new GetNewMessagesRequestModel()
            {
                ChatroomId = chatRoom.ChatroomModel.ChatroomId,
                UserId = USER.Id,
                ExistingMessageIds = new List<int>(),
                ParentChatroomId = chatRoom.ChatroomModel.ParentChatroomId,
                User = new UserModel() { Id = USER.Id }
            };

            //Test that there are > 0 message results returned. 
            var result = chatControllerTest.GetNewMessages(model2) as JsonResult;
            Assert.AreNotEqual(0, ((GetNewMessagesResponseModel)result.Data).MessagesInformation.Count);
        }
    }
}
