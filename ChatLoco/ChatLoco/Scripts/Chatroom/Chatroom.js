

var _AllMessages = [];
var _MessagesContainer = $("#MessagesContainer");
var _ChatroomName = $("#ChatName")[0].value;
var _Username = $("#Username")[0].value;

setInterval(GetNewMessages, 1000);

$("#ComposeForm").on("submit", SendComposedMessage);

function SendComposedMessage(e) {
    e.preventDefault();

    var $form = this;

    var $message = $form[0].value;

    $.ajax({
        type: "POST",
        url: '/Chatroom/SendMessage',
        data: { Message: $message, Username: _Username, ChatroomName: _ChatroomName },
        success: function () {
            $form[0].value = "";
        },
        error: function () {
            var s = "";
        }
    });

}

function GetNewMessages() {
    $.ajax({
        type: "POST",
        url: '/Chatroom/GetNewMessages',
        data: JSON.stringify({ ChatroomName: _ChatroomName, Username: _Username, CurrentMessages: _AllMessages }),
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