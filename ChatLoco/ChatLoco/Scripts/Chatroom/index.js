
function FindChatroom() {
    var _findChatroomForm = null;
    var _updateLocationForm = null;

    var _chatroom = null;

    var FindChatroom = function(e) {
        e.preventDefault();

        NotificationHandler.ShowLoading();

        var $form = this;

        var $coord1 = $form.elements.coord1.value;
        var $coord2 = $form.elements.coord2.value;

        var $userHandle = $form.elements.userHandle.value;

        if ($form.elements.chatroomPlaces.value === '') {
            $chatroomId = parseInt($coord1) + parseInt($coord2);
            $chatroomName = "Coords: " + $coord1 + ", " + $coord2;
        } else {
            $chatroomId = $("#chatroomPlaces").val();
            $chatroomName = $("#chatroomPlaces option:selected").text();
        }

        var $model = {
            ChatroomId: $chatroomId,
            ChatroomName: $chatroomName,
            UserHandle: $userHandle,
            User: AccountHandler.GetUser()
        }

        $.ajax({
            type: "POST",
            url: '/Chatroom/Chat',
            data: $model,
            success: function (response) {

                if (ErrorHandler.DisplayErrors(response)) {
                    return;
                }
                else {
                    AccountHandler.GetUser().UserHandle = $userHandle;
                    $("#chatroom-container").html("").append(response.Data);
                    ChatroomHandler = new ChatroomObject();
                    ChatroomHandler.init();
                }

                NotificationHandler.HideLoading();
            },
            error: function (data) {
                document.write(data.responseText);
            }
        });

    }

    var init = function() {
        MapHandler.initMap();

        _findChatroomForm = $("#find-chatroom-form");
        _findChatroomForm.on("submit", FindChatroom);

    }

    return {
        init: init
    }
}

var findChatroom = new FindChatroom();

findChatroom.init();