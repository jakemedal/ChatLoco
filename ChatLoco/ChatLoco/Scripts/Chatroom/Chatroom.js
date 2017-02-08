

var _AllMessages = [];
var _MessagesWindow = $("#MessagesContainer");

setInterval(GetNewMessages, 1000);

$("#ComposeForm").on("submit", SendComposedMessage);

function SendComposedMessage(e) {
    e.preventDefault();

    var $message = this[0].value;
    var $username = this[1].value;

    $.ajax({
        type: "POST",
        url: '/Chatroom/SendMessage',
        data: { Message: $message, Username: $username },
        success: function () {
            var s = "";
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
        data: JSON.stringify({ CurrentMessages : _AllMessages }),
        contentType: "application/json; charset=utf-8",
        dataType: "json",
        success: function (data) {
            for (var i = 0; i < data.length; i++) {
                var $newMessage = data[i];

                _MessagesWindow.append("<p>" + $newMessage + "</p>");
                _AllMessages.push($newMessage);
            }
        },
        error: function (data) {
            var s = "";
        }
    });
}