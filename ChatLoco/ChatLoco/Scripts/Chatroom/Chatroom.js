

var _AllMessages = [];

var _MessagesContainer = $("#MessagesContainer");
var _UsersContainer = $("#UsersContainer");
var _SubChatroomsList = $("#SubChatroomsList");

var _ChatroomName = $("#ChatName")[0].value;
var _Username = $("#Username")[0].value;
var _UserId = $("#User-Id")[0].value;

GetNewMessages();
setInterval(GetNewMessages, 1000);

GetChatroomInformation();
setInterval(GetChatroomInformation, 5000);

$("#ComposeForm").on("submit", SendComposedMessage);
$("#CreateSubChatroomForm").on("submit", CreateSubChatroom);



function GetChatroomInformation(e) {
    $.ajax({
        type: "POST",
        url: '/Chatroom/GetChatroomInformation',
        data: JSON.stringify({ ChatroomName: _ChatroomName, Username: _Username }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {

            if (DisplayErrors(data.Errors)) {
                return;
            }

            //Update users list
            _UsersContainer.html("");
            for (var i = 0; i < data.Users.length; i++) {
                var $username = data.Users[i].Username;
                _UsersContainer.append("<p>" + $username + "</p>");
            }

            //Update subchatrooms list
            _SubChatroomsList.html("");
            for (var i = 0; i < data.SubChatrooms.length; i++) {
                var $subChatroomName = data.SubChatrooms[i].Name;
                _SubChatroomsList.append("<li>" + $subChatroomName + "</li>");
            }

        },
        error: function (data) { }
    });
}

function CreateSubChatroom(e) {
    e.preventDefault();
    var $form = this;

    var $subChatroomName = $form[0].value;
    var $subChatroomDialog = $('#SubChatroomDialog');

    $.ajax({
        type: "POST",
        url: '/Chatroom/CreateSubChatroom',
        data: { SubChatroomName: $subChatroomName, Username: _Username, ChatroomName: _ChatroomName },
        success: function (data) {
            var $responseMessage = "";

            if (data) {
                $responseMessage = "<p>Chatroom "+ $subChatroomName+" created successfully.</p>";
            }
            else {
                $responseMessage = "<p>Chatroom " + $subChatroomName + " could not be created.</p>";
            }

            $subChatroomDialog.html("").append($responseMessage);
            $subChatroomDialog.dialog({
                modal: true,
                buttons: {
                    Ok: function () {
                        $(this).dialog("close");
                    }
                }
            });

            $form[0].value = "";
        },
        error: function () {
        }
    });
}

function SendComposedMessage(e) {
    e.preventDefault();

    var $form = this;

    var $message = $form[0].value;

    $.ajax({
        type: "POST",
        url: '/Chatroom/SendMessage',
        data: { Message: $message, Username: _Username, ChatroomName: _ChatroomName, UserId: _UserId },
        success: function () {
            $form[0].value = "";
        },
        error: function () {
        }
    });

}

function GetNewMessages() {
    $.ajax({
        type: "POST",
        url: '/Chatroom/GetNewMessages',
        data: JSON.stringify({ ChatroomName: _ChatroomName, Username: _Username, UserId: _UserId, CurrentMessages: _AllMessages }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                var $newMessage = data[i];

                _MessagesContainer.append("<p>" + $newMessage + "</p>");
                _AllMessages.push($newMessage);
            }
            if (data.length != 0) {
                _MessagesContainer.scrollTop(_MessagesContainer[0].scrollHeight);
            }
        },
        error: function (data) {
            var s = "";
        }
    });
}
