var MAX_RADIUS = 500;
var DEFAULT_RADIUS = 100;

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
            RawChatroomIdValue: $chatroomId,
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
                    if (ChatroomHandler != null) {
                        ChatroomHandler.Destroy();
                    }
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

        if (AccountHandler.GetUser() != null) {
            _findChatroomForm[0].elements[1].value = AccountHandler.GetUser().Settings.DefaultHandle;
        }

    }

    var Destroy = function () {
        _findChatroomForm.off("submit", FindChatroom);

        _changeLocationForm.off("submit", ChangeLocation);
    }

    $(function () {
        var handle = $("#slider-handle");
        $("#slider").slider({
            max: MAX_RADIUS,
            min: 1,
            value: DEFAULT_RADIUS,
            create: function () {
                handle.text($(this).slider("value") + "m");
            },
            slide: function (event, ui) {
                handle.text(ui.value + "m");
            },
            stop: function (event, ui) {
                $("#chatroomPlaces").html("");
                MapHandler.init();
            }
        });
    });

    return {
        init: init,
        Destroy: Destroy
    }
}

var findChatroom = new FindChatroom();

findChatroom.init();