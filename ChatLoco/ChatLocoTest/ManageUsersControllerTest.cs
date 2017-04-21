
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ChatLoco.DAL;
using ChatLoco.Models.User_Model;
using ChatLoco.Controllers;
using System.Collections.Generic;
using ChatLoco.Services.User_Service;
using System.Web.Mvc;
using ChatLoco.Models.AdminAction_Model;
using ChatLoco.Entities.UserDTO;

namespace ChatLocoTest
{
    [TestClass]
    public class ManageUsersControllerTest
    {
        /* --This section is run everytime before any of the tests-- */
        /* --ensures the database doesn't have left over data from previous tests so that each time we run the tests the data is consistent--*/
/*###########################################################################################################################################*/

        //Need a reference to the db context
        private ChatLocoContext _db = new ChatLocoContext();
        //TestHelper that will hold all of the created users
        private TestHelper _helper;

        /*constructor - create an instance of TestHelper passing in the users we want to use for testing*/
        public ManageUsersControllerTest()
        {

            List<UserTestModel> newUsers = new List<UserTestModel>();
            newUsers.Add(new UserTestModel() { Username = "testmanageusercontroller00", Password = "manageusercontroller00", Email = "testmanageusercontroller00@test.com", Id = -1, DefaultHandle = "testmanageusercontroller00" });
            newUsers.Add(new UserTestModel() { Username = "testmanageusercontroller01", Password = "manageusercontroller01", Email = "testmanageusercontroller01@test.com", Id = -1, DefaultHandle = "testmanageusercontroller01" });

            _helper = new TestHelper(newUsers, new List<int>());
        }


        /* --This section includes default View functions-- */
/*###########################################################################################################################################*/

        /*Call the Index method: The Index view*/
        [TestMethod]
        public void TestIndexView()
        {
            ManageUsersController manageControllerTest = new ManageUsersController();
            var result = manageControllerTest.Index() as ViewResult;
            Assert.AreEqual("", result.ViewName);
        }

        /* --This section is the fun stuff. Begin testing database functions-- */
/*###########################################################################################################################################*/

        /*Call all the admin methods: After each assert that the correct role level has been applied until we delete it*/
        [TestMethod]
        public void TestHandleAdminAction()
        {
            var USER = _helper.testUsers[0];
            ManageUsersController manageControllerTest = new ManageUsersController();

            AdminActionRequestModel model = new AdminActionRequestModel()
            {
                Username = USER.Username,
                Action = "MakeAdmin"
            };

            //Test make admin
            var result = manageControllerTest.HandleAdminAction(model) as JsonResult;
            Assert.AreEqual(0, ((AdminActionResponseModel)result.Data).Errors.Count);
            Assert.AreEqual("Action Succesfully Completed.", ((AdminActionResponseModel)result.Data).Message);
            UserDTO user = UserService.GetUser(USER.Username);
            Assert.AreEqual(RoleLevel.Admin, user.Role);

            //Test blocking user
            model.Action = "Block";
            result = manageControllerTest.HandleAdminAction(model) as JsonResult;
            Assert.AreEqual(0, ((AdminActionResponseModel)result.Data).Errors.Count);
            Assert.AreEqual("Action Succesfully Completed.", ((AdminActionResponseModel)result.Data).Message);
            user = UserService.GetUser(USER.Username);
            Assert.AreEqual(RoleLevel.Blocked, user.Role);

            //Test unblocking user
            model.Action = "Unblock";
            result = manageControllerTest.HandleAdminAction(model) as JsonResult;
            Assert.AreEqual(0, ((AdminActionResponseModel)result.Data).Errors.Count);
            Assert.AreEqual("Action Succesfully Completed.", ((AdminActionResponseModel)result.Data).Message);
            user = UserService.GetUser(USER.Username);
            Assert.AreEqual(RoleLevel.User, user.Role);

            //Test deleting user
            model.Action = "Delete";
            result = manageControllerTest.HandleAdminAction(model) as JsonResult;
            Assert.AreEqual(0, ((AdminActionResponseModel)result.Data).Errors.Count);
            Assert.AreEqual("Action Succesfully Completed.", ((AdminActionResponseModel)result.Data).Message);
            user = UserService.GetUser(USER.Username);
            Assert.AreEqual(null, user);
        }

    }
}
