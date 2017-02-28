
var _findChatroomForm = $("#find-chatroom-form");
var _chatroom = null;

_findChatroomForm.on("submit", FindChatroom)

function FindChatroom(e) {
    e.preventDefault();

    var $form = this;

    var $chatroomName = $form.elements.chatroomName.value;
    var $userHandle = $form.elements.userHandle.value;

    var $model = {
        ChatroomName: $chatroomName,
        UserHandle: $userHandle,
        User: GetUser()
    }

    $.ajax({
        type: "POST",
        url: '/Chatroom/Chat',
        data: $model,
        success: function (response) {

            if (DisplayErrors(response.Errors)) {
                return;
            }
            else {
                GetUser().UserHandle = $userHandle;
                $("#content-container").html("").append(response.Data);
                _chatroom = new Chatroom();
            }

        },
        error: function (data) {
            document.write(data.responseText);
        }
    });

}