

var _AllMessages = [];

var _MessagesContainer = $("#MessagesContainer"); //empty div we fill with html from JS
var _UsersContainer = $("#UsersContainer"); //empty div we fill with html from JS
var _SubChatroomsList = $("#SubChatroomsList"); //empty div we fill with html from JS

var _ChatroomName = $("#ChatName")[0].value;
var _Username = $("#Username")[0].value;
var _UserId = $("#User-Id")[0].value;
var _ParentChatroomName = $("#ParentChatroomName")[0].value;

var _ParentChatroomButton = $("#ParentChatroomButton");
var _CreateSubChatroomsContainer = $("#CreateSubChatroomContainer");

GetNewMessages(); //Grab new messages immediately
setInterval(GetNewMessages, 1000); //every one second, call GetNewMessages

GetChatroomInformation(); //We update the page 
setInterval(GetChatroomInformation, 5000); //Every five seconds, call GetChatroomInformation

$("#ComposeForm").on("submit", SendComposedMessage); //Bind SendComposedMessage method to the form for sending messages
$("#CreateSubChatroomForm").on("submit", CreateSubChatroom); //bind CreatSubChatroom method to the form for creating private chatrooms
$("#ParentChatroomButton").on("click", OpenChat);

//TODO 
$("#SubChatroomsList").on("click", OpenChat);
function OpenChat(e) {
    e.preventDefault();
    var $element = document.elementFromPoint(e.clientX, e.clientY);
    var $newChatroomName = $element.value;

    _AllMessages = [];
    _ChatroomName = $newChatroomName;

    $("#ChatroomName").html($newChatroomName);
    _MessagesContainer.html("");

    if (_ChatroomName == _ParentChatroomName) {
        _ParentChatroomButton.removeClass("btn btn-primary").addClass("btn btn-primary");
        _CreateSubChatroomsContainer.show();
    }
    else {
        _ParentChatroomButton.removeClass("btn btn-primary").addClass("btn btn-secondary");
        _CreateSubChatroomsContainer.hide();
    }

    GetChatroomInformation(); //We update the page 
    GetNewMessages(); //Grab new messages immediately
}

//This is called every 5 seconds.
//This method gets the users list and private chatrooms list
//It then fills in the two empty divs with html we generate
function GetChatroomInformation(e) {
    $.ajax({
        type: "POST", 
        url: '/Chatroom/GetChatroomInformation', //controller is ChatroomController.cs, method is GetChatroomInformation, it takes in GetChatroomInformationModel as its argument

        //The variables being defined match exact same names in GetChatroomInformationModel
        data: JSON.stringify({ ChatroomName: _ChatroomName, Username: _Username, ParentChatroomName: _ParentChatroomName }), //The controller will automatically construct the model since the names are the same
        contentType: "application/json; charset=utf-8", //we are using JSON to pass our content to the controller
        dataType: "json", //the data type is json

        //our success function is defined here. this is called if our ajax call was successful
        success: function (data) { //data is the information being returned from our controller method

            if (DisplayErrors(data.Errors)) { //this is a generic method defined in errorhandling.js
                return;
            }

            //Update users list
            _UsersContainer.html(""); //replace any html in the users div with nothing, effectively deleting old html
            for (var i = 0; i < data.Users.length; i++) {
                var $user = data.Users[i];
                if ($user.IsActive) {
                    var $username = data.Users[i].Username;
                    _UsersContainer.append("<p>" + $username + "</p>"); //append username to the empty div that we cleared out
                }
            }

            //Update subchatrooms list, works same way as users div does
            _SubChatroomsList.html("<br/>");
            for (var i = 0; i < data.SubChatroomsNames.length; i++) {
                var $subChatroomName = data.SubChatroomsNames[i];
                var $buttonType = 'secondary';
                if ($subChatroomName === _ChatroomName) {
                    $buttonType = 'primary';
                }
                _SubChatroomsList.append('<li><button value="' + $subChatroomName + '" type="button" class="btn btn-' + $buttonType + '">' + $subChatroomName + '</button></li><br/>');
            }

        },
        error: function (data) { } //if the ajax call fails, it will go to this method
    });
}

//This is the method for creating a private chatroom
//We bound this method to the create private chatroom form earlier
function CreateSubChatroom(e) { // the argument here, e, is the object that is calling the method, in this case it would be the form
    e.preventDefault(); //we prevent the form from doing its default action, which would be submitting and refreshing the page
    var $form = this; //in this function, the this variable is the form

    var $subChatroomName = $form[0].value; //our first defined element in the form is the name, we get that element's value
    var $subChatroomDialog = $('#SubChatroomDialog'); //we find the hidden dialog div, so we can have it popup later

    $.ajax({
        type: "POST",
        url: '/Chatroom/CreateSubChatroom', //controller is ChatroomController.cs, method is CreateSubChatRoom
        //The method header in the controller looks like CreateSubChatroom(string SubChatroomName, string ChatroomName, string Username)
        //we match the arguments in the data
        //since we are not constructing a model from this, we dont need to use json
        data: { SubChatroomName: $subChatroomName, Username: _Username, ChatroomName: _ChatroomName },
        success: function (data) { //in this case the data being received from the controller method is a boolean
            var $responseMessage = "";

            if (data) {
                $responseMessage = "<p>Chatroom " + $subChatroomName + " created successfully.</p>";
                GetChatroomInformation(); //if successfully created a new chatroom, update the chatroom list immediately instead of waiting for the five second refresh
            }
            else {
                $responseMessage = "<p>Chatroom " + $subChatroomName + " could not be created.</p>";
            }

            $subChatroomDialog.html("").append($responseMessage); //in the dialog div, we clear the html, then we add our message
            $subChatroomDialog.dialog({ // .dialog is a jquery ui function which can be passed arguments to automatically construct a dialog popup modal
                modal: true, //this means we can assign it buttons and other things
                buttons: { 
                    Ok: function () { //our okay button has a function
                        $(this).dialog("close"); //the argument .dialog("close") is a jqueryui function that can be called on dialog objects to close them
                    }
                }
            });

            $form[0].value = ""; //this clears out the form from whatever text we entered in there originally, index 0 is our textbox
        },
        error: function () { }
    });
}

//i think this function can be figured out by referring to previous comments
function SendComposedMessage(e) {
    e.preventDefault();

    var $form = this;

    var $message = $form[0].value;

    $.ajax({
        type: "POST",
        url: '/Chatroom/SendMessage',
        data: { Message: $message, Username: _Username, ChatroomName: _ChatroomName, UserId: _UserId, ParentChatroomName: _ParentChatroomName },
        success: function () {
            $form[0].value = "";
        },
        error: function () {
        }
    });

}

//This function is called every second.
//It simply retrieves any messages that are new, and appends them to the messages container
function GetNewMessages() {
    $.ajax({
        type: "POST",
        url: '/Chatroom/GetNewMessages', //controller is ChatroomController.cs, method is GetNewMessages
        //the method header for GetNewMessages is GetNewMessages(GetNewMessagesModel RequestUpdate)
        //we use JSON here to allow for the automatic construction of our GetNewMessagesModel
        //this only works since the parameters we give it match the exact same variable names in our GetNewMessagesModel
        data: JSON.stringify({ ChatroomName: _ChatroomName, Username: _Username, UserId: _UserId, CurrentMessages: _AllMessages, ParentChatroomName: _ParentChatroomName }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        //the method returns a list of new messages
        //it does this by taking in a list of current messages that we have on the page, and returning the difference between current messages and all the messages in the chatroom
        success: function (data) { 
            for (var i = 0; i < data.length; i++) {
                var $newMessage = data[i];

                _MessagesContainer.append("<p>" + $newMessage + "</p>"); //append the new message to the messages container

                //add the new message to our array in JS which contains all the messages we see
                //we do this so next time the method is called, we are checking with our new messages updated
                _AllMessages.push($newMessage); 
            }
            if (data.length != 0) { //if we had new messages to add, then scroll to the bottom of the messages container
                _MessagesContainer.scrollTop(_MessagesContainer[0].scrollHeight);
            }
        },
        error: function (data) {
        }
    });
}
