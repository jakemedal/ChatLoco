
function FindChatroom() {
    var _findChatroomForm = null;
    var _chatroom = null;

    var FindChatroom = function(e) {
        e.preventDefault();

        notifications.ShowLoading();

        var $form = this;

        var $coord1 = $form.elements.coord1.value;
        var $coord2 = $form.elements.coord2.value;
        var $userHandle = $form.elements.userHandle.value;

        //TODO: This will be replaced when we use google api to get unique location id by coordinates
        var $chatroomId = parseInt($coord1) + parseInt($coord2);

        //TODO: This will be replaced when we use google api to get location names by coordinates
        var $chatroomName = "Name: " + $coord1 + ", " + $coord2;

        var $model = {
            ChatroomId: $chatroomId,
            ChatroomName: $chatroomName,
            UserHandle: $userHandle,
            User: account.GetUser()
        }

        $.ajax({
            type: "POST",
            url: '/Chatroom/Chat',
            data: $model,
            success: function (response) {

                if (error.DisplayErrors(response)) {
                    return;
                }
                else {
                    account.GetUser().UserHandle = $userHandle;
                    $("#chatroom-container").html("").append(response.Data);
                    _chatroom = new Chatroom();
                }

                notifications.HideLoading();
            },
            error: function (data) {
                document.write(data.responseText);
            }
        });

    }

    var init = function() {
        initMap();

        _findChatroomForm = $("#find-chatroom-form");
        _findChatroomForm.on("submit", FindChatroom);
    }

    return {
        init: init
    }
}

var findChatroom = new FindChatroom();

findChatroom.init();