using ChatLoco.Controllers;
using ChatLoco.Models.User_Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Web.Mvc;
using ChatLoco.Services.User_Service;
using ChatLoco.Models.Chatroom_Model;
using ChatLoco.Services.Chatroom_Service;
using ChatLoco.Services.Security_Service;

namespace ChatLocoTest
{
    class TestHelper
    {
        //Holds all the test user data that will be used throughout the tests
        public List<UserTestModel> testUsers = new List<UserTestModel>();
        //blacklist users who will be created via test methods I.E. CreateUser()
        List<int> blacklistUsers = new List<int>();//add indicies to the list of users from testUsers you DON'T want created

        //Always initialize the list of uses that will be used in the below tests
        //In order to add more users to the tests, just add them here
        private void initializeUsers(List<UserTestModel> users)
        {
            testUsers = users;
        }

        //Initialize the list of blackListed users
        private void initializeBlacklistUsers(List<int> blacklist)
        {
            blacklistUsers = blacklist;
        }

        /*Construct a list of all test users to be removed from the db*/
        private List<RemoveUserRequestModel> initializeRemovableUsers()
        {
            List<RemoveUserRequestModel> users = new List<RemoveUserRequestModel>();
            foreach (UserTestModel us in testUsers)
            {
                users.Add(new RemoveUserRequestModel() { Username = us.Username });
            }
            return users;
        }

        /*Create necessary test users for other test functions to utilize*/
        private void createTestUsers()
        {
            UserController userControllerTest = new UserController();
            for (var i = 0; i < testUsers.Count; ++i)
            {
                if (!blacklistUsers.Contains(i))
                {
                    var result = userControllerTest.CreateUser(
                        new CreateUserRequestModel()
                        {
                            Username = testUsers[i].Username,
                            Password = testUsers[i].Password,
                            Email = testUsers[i].Email
                        }) as JsonResult;
                    //everytime still just asser that the user was actually created. If not, thats a problem so throw error
                    Assert.AreEqual(0, ((CreateUserResponseModel)result.Data).Errors.Count);
                    //grab the users ID and store that
                    var createdUser = UserService.GetUser(testUsers[i].Username);
                    testUsers[i].Id = createdUser.Id;
                }
            }
        }

        /*remove all test users*/
        private void removeTestUsers()
        {
            //grab the list of users to make sure are deleted.
            List<RemoveUserRequestModel> removals = initializeRemovableUsers();

            UserController userControllerTest = new UserController();

            foreach (RemoveUserRequestModel res in removals)
            {
                var result = userControllerTest.RemoveUser(res) as JsonResult;
                if (((RemoveUserResponseModel)result.Data).Errors.Count == 1)
                {
                    if (((RemoveUserResponseModel)result.Data).Errors[0].ErrorMessage == "user.no.exist")
                    {
                        //not a problem move on
                    }
                    else
                    {
                        //was an acutal problem with removing the user!! Fail and leave!!!
                        Assert.AreEqual("1", "2");
                    }
                }
            }
        }

        //Helper method for creating a chatroom since MVC uses controller contexts to route the user to the chat partial view page/
        //We don't need that routing here, all we need is the logic or 99% above that line so just replicate it here and
        //This will create a chatroom and add the user to it
        //The chatrrom only exists in memory while the tests are run. Each new build refreshes
        public ChatResponseTestModel createChatroomAndAddUser(ChatRequestModel request)
        {
            var response = new ChatResponseTestModel();

            int chatroomId = 0;
            if (request.RawChatroomIdValue != null)
            {
                chatroomId = request.RawChatroomIdValue.GetHashCode();
            }
            int parentChatroomId = chatroomId; //temporary during initial testing

            string chatroomName = request.ChatroomName;

            if (!ChatroomService.DoesChatroomExist(chatroomId))
            {
                ChatroomService.CreateChatroom(chatroomId, chatroomName);
            }

            var joinErrors = SecurityService.CanUserJoinChatroom(request);
            response.Errors.AddRange(joinErrors);

            if (joinErrors.Count == 0)
            {
                if (!ChatroomService.AddUserToChatroom(chatroomId, parentChatroomId, request.User.Id, request.UserHandle))
                {
                    response.AddError("Error adding user into chatroom.");
                }
            }

            var chatroomModel = new ChatroomModel()
            {
                ChatroomId = chatroomId,
                ChatroomName = chatroomName,
                ParentChatroomId = parentChatroomId,
                UserHandle = request.UserHandle,
                UserId = request.User.Id
            };

            response.ChatroomModel = chatroomModel;

            return response;
        }

        /*constructor - always delete the users that were created if not already gone. Create those who need creating*/
        public TestHelper(List<UserTestModel> users, List<int> blacklist)
        {
            //Initialize the users
            initializeUsers(users);
            //Initialize the blacklist
            initializeBlacklistUsers(blacklist);
            //Remove ALL test users
            removeTestUsers();
            //Create necessary test users
            createTestUsers();
        }
    }
}
