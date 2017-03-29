
function FindChatroom() {
    var _findChatroomForm = null;
    var _changeLocationForm = null;

    var _chatroom = null;

    var FindChatroom = function(e) {
        e.preventDefault();

        NotificationHandler.ShowLoading();

        var $form = this;

        var $userHandle = $form.elements.userHandle.value;

        $chatroomId = $("#chatroomPlaces").val();
        $chatroomName = $("#chatroomPlaces option:selected").text();

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

    var ChangeLocation = function (e) {
        //get long and lat from e
        e.preventDefault();
        $("#chatroomPlaces").html("");

        var $form = e.target;
        var $lat = parseFloat($form[0].value);
        var $lon = parseFloat($form[1].value);

        MapHandler.getInitMap().getNearbyPlaces($lat, $lon);
    }

    var init = function() {
        MapHandler.init();

        _findChatroomForm = $("#find-chatroom-form");
        _findChatroomForm.on("submit", FindChatroom);

        _changeLocationForm = $("#change-location-form");
        _changeLocationForm.on("submit", ChangeLocation);

    }

    return {
        init: init
    }
}

var findChatroom = new FindChatroom();

findChatroom.init();