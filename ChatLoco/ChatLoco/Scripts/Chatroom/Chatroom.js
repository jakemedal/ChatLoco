
var ChatroomObject = function () {

    var _AllMessages = [];
    var _AllMessagesIds = [];

    var _MessagesContainer = null;
    var _UsersContainer = null;
    var _SubChatroomsList = null;

    var _ChatroomName = null;
    var _ChatroomId = null;
    var _UserHandle = null;
    var _UserId = null;
    var _ParentChatroomId = null;
    var _UserHandleContainer = null;

    var _ParentChatroomButton = null;

    var _PrivateChatroomRequestDialog = null;

    var _GetNewMessagesInterval = null;
    var _GetChatroomInformationInterval = null;

    var _CreatePrivateChatroomDialogButton = null;
    var _CreatePrivateChatroomDialog = null;
    var _CreatePrivateChatroomForm = null;

    var Destroy = function () {
        if (_GetChatroomInformationInterval != null) {
            clearInterval(_GetChatroomInformationInterval);
        }

        if (_GetNewMessagesInterval != null) {
            clearInterval(_GetNewMessagesInterval);
        }
    }

    var init = function () {
        Destroy();

        _AllMessages = [];
        _AllMessagesIds = [];

        _CreatePrivateChatroomDialogButton = $("#create-private-chatroom-dialog-button");
        _CreatePrivateChatroomDialogButton.on("click", OpenCreatePrivateChatroomDialog);

        _CreatePrivateChatroomDialog = $("#create-private-chatroom-dialog");
        _CreatePrivateChatroomForm = $("#create-private-chatroom-form");

        _MessagesContainer = $("#MessagesContainer");
        _UsersContainer = $("#UsersContainer");
        _SubChatroomsList = $("#SubChatroomsList");

        _ChatroomName = $("#ChatroomName")[0].value;
        _ChatroomId = $("#ChatroomId")[0].value;
        _UserHandle = $("#UserHandle")[0].value;
        _UserId = $("#UserId")[0].value;
        _ParentChatroomId = $("#ParentChatroomId")[0].value;
        _UserHandleContainer = $("#user-handle-container");

        _ParentChatroomButton = $("#ParentChatroomButton");

        _PrivateChatroomRequestDialog = $("#private-chatroom-dialog");

        GetNewMessages();
        _GetNewMessagesInterval = setInterval(GetNewMessages, 1000);

        GetChatroomInformation();
        _GetChatroomInformationInterval = setInterval(GetChatroomInformation, 5000);

        $("#ComposeForm").on("submit", SendComposedMessage);
        $("#ParentChatroomButton").on("click", ChatroomClicked);
        $("#SubChatroomsList").on("click", ChatroomClicked);
        $("#private-chatroom-request-form").on("submit", ChatroomRequestFormSubmit);

        ChangeChatroomNameTo(_ChatroomName);
    }

    function ChangeChatroomNameTo(name) {
        _ChatroomName = name;
        document.title = name;
        $("#ChatroomNameDisplay").html(name);
    }

    function OpenCreatePrivateChatroomDialog(e) {
        e.preventDefault();

        var $buttons = [{
            text: "Create",
            click: CreatePrivateChatroom
        }];

        _CreatePrivateChatroomDialog.dialog({
            title: "Create Private Chatroom",
            modal: true,
            buttons: $buttons
        });
    }

    function SetupRequestChatroomDialog() {
        var $form = _PrivateChatroomRequestDialog.find("#private-chatroom-request-form")[0];

        if (typeof $form != 'undefined') {

            $form.elements.userHandle.value = AccountHandler.GetUser().UserHandle;

            var $chatroomName = $form.elements.chatroomName.value;

            var $buttons = [{
                text: "Chat!",
                click: ChatroomRequestFormSubmit
            }];

            _PrivateChatroomRequestDialog.dialog({
                title: "Requesting to join " + $chatroomName,
                modal: true,
                buttons: $buttons
            });
        }

    }

    function ChatroomClicked(e) {
        e.preventDefault();

        var $element = document.elementFromPoint(e.clientX, e.clientY);

        //means we clicked the text, instead of the button
        if ($element.id === 'parent-chatroom-button-text') {
            $element = $element.parentElement;
        }

        var $newChatroomId = $element.value;

        if (typeof $newChatroomId == 'undefined') {
            return;
        }

        var $model = {
            ParentChatroomId: _ParentChatroomId,
            ChatroomId: $newChatroomId
        }

        PartialViewHandler.GetAndRenderPartialView("/Chatroom/GetJoinChatroomForm", "POST", $model, _PrivateChatroomRequestDialog, SetupRequestChatroomDialog);
    }

    function ChatroomRequestFormSubmit(e) {
        if (e) {
            e.preventDefault();
        }

        _PrivateChatroomRequestDialog.dialog("close");

        var $form = _PrivateChatroomRequestDialog.find("#private-chatroom-request-form")[0];

        var $userHandle = $form.elements.userHandle.value;

        var $password = null;
        if ($form.elements.password) {
            $password = $form.elements.password.value;
        }

        var $newChatroomId = $form.elements.newChatroomId.value;

        OpenChat($newChatroomId, $userHandle, $password);
    }

    function OpenChat($newChatroomId, $userHandle, $password) {
        NotificationHandler.ShowLoading();

        var $model = {
            UserId: _UserId,
            ChatroomId: $newChatroomId,
            ParentChatroomId: _ParentChatroomId,
            CurrentChatroomId: _ChatroomId,
            UserHandle: $userHandle,
            Password: $password,
            User: AccountHandler.GetUser()
        };

        $.ajax({
            type: "POST",
            url: '/Chatroom/JoinChatroom',
            data: JSON.stringify($model),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                if (ErrorHandler.DisplayErrors(data)) {
                    return;
                }

                _AllMessages = [];
                _AllMessagesIds = [];
                _ChatroomId = data.Id;

                _MessagesContainer.html("");

                AccountHandler.GetUser().UserHandle = data.UserHandle;

                _UserHandleContainer.html(data.UserHandle);

                if (_ChatroomId == _ParentChatroomId) {
                    _ParentChatroomButton.removeClass("btn btn-primary").addClass("btn btn-primary");
                }
                else {
                    _ParentChatroomButton.removeClass("btn btn-primary").addClass("btn btn-secondary");
                }

                GetChatroomInformation();
                GetNewMessages();

                ChangeChatroomNameTo(data.Name);

                NotificationHandler.HideLoading();
            },
            error: function (e) { }
        });
    }

    function GetChatroomInformation(e) {
        var $model = {
            ChatroomId: _ChatroomId,
            UserId: _UserId,
            ParentChatroomId: _ParentChatroomId
        };

        $.ajax({
            type: "POST",
            url: '/Chatroom/GetChatroomInformation',
            data: JSON.stringify($model),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {

                if (ErrorHandler.DisplayErrors(data)) {
                    return;
                }

                _UsersContainer.html("");
                for (var i = 0; i < data.UsersInformation.length; i++) {
                    var $user = data.UsersInformation[i];
                    var $username = data.UsersInformation[i].Username;
                    _UsersContainer.append("<p>" + $username + "</p>");
                }

                _SubChatroomsList.html("<br/>");
                for (var i = 0; i < data.PrivateChatroomsInformation.length; i++) {
                    var $subChatroomName = data.PrivateChatroomsInformation[i].Name;
                    var $subChatroomId = data.PrivateChatroomsInformation[i].Id;
                    var $buttonType = 'secondary';
                    if ($subChatroomId === _ChatroomId) {
                        $buttonType = 'primary';
                    }
                    _SubChatroomsList.append('<p><button value="' + $subChatroomId + '" type="button" class="btn btn-' + $buttonType + '">' + $subChatroomName + '</button></p>');
                }

            },
            error: function (data) {
                lostConnection();
            }
        });
    }

    function ClearCreatePrivateChatroomForm() {

        var $form = _CreatePrivateChatroomForm[0];

        $form.elements.name.value = "";
        $form.elements.password.value = "";
        $form.elements.blacklist.value = "";
        $form.elements.capacity.value = "";
    }

    function CreatePrivateChatroom(e) {
        e.preventDefault();

        NotificationHandler.ShowLoading();

        var $form = _CreatePrivateChatroomForm[0];

        var $ChatroomName = $form.elements.name.value;
        var $Password = $form.elements.password.value;
        var $Blacklist = $form.elements.blacklist.value;
        var $Capacity = $form.elements.capacity.value;

        var $model = {
            ChatroomName: $ChatroomName,
            ParentChatroomId: _ParentChatroomId,
            Password: $Password,
            Blacklist: $Blacklist,
            Capacity: $Capacity,
            User: AccountHandler.GetUser()
        };

        _CreatePrivateChatroomDialog.dialog("close");

        $.ajax({
            type: "POST",
            url: '/Chatroom/CreateChatroom',
            data: $model,
            success: function (data) {

                if (ErrorHandler.DisplayErrors(data)) {
                    return;
                }
                else {
                    var $responseMessage = "<p>Chatroom " + data.ChatroomName + " created successfully.</p>";
                    StatusHandler.DisplayStatus($responseMessage);
                    GetChatroomInformation();
                    ClearCreatePrivateChatroomForm();
                }

                NotificationHandler.HideLoading();
            },
            error: function () { }
        });
    }

    function SendComposedMessage(e) {
        e.preventDefault();

        var $form = this;

        var $message = $form[0].value;

        var $model = {
            Message: $message,
            ChatroomId: _ChatroomId,
            UserId: _UserId,
            ParentChatroomId: _ParentChatroomId
        };

        $.ajax({
            type: "POST",
            url: '/Chatroom/ComposeMessage',
            data: $model,
            success: function (data) {

                if (ErrorHandler.DisplayErrors(data)) {
                    return;
                }

                $form[0].value = "";
            },
            error: function () {
            }
        });

    }

    function GetChatroomId() {
        if (_ChatroomId) {
            return _ChatroomId;
        }
        else {
            return -1;
        }
    }

    function GetParentChatroomId() {
        if (_ParentChatroomId) {
            return _ParentChatroomId;
        }
        else {
            return -1;
        }
    }

    function lostConnection() {
        clearInterval(_GetChatroomInformationInterval);
        clearInterval(_GetNewMessagesInterval);
        AccountHandler.OpenDisconnectedDialog(AccountHandler.DirtyLogout);
    }

    function GetNewMessages() {
        var $model = {
            ChatroomId: _ChatroomId,
            UserId: _UserId,
            ExistingMessageIds: _AllMessagesIds,
            ParentChatroomId: _ParentChatroomId
        };

        $.ajax({
            type: "POST",
            url: '/Chatroom/GetNewMessages',
            data: JSON.stringify($model),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {
                for (var i = 0; i < data.MessagesInformation.length; i++) {
                    var $newMessage = data.MessagesInformation[i].FormattedMessage;

                    _MessagesContainer.append("<p>" + $newMessage + "</p>");

                    _AllMessages.push($newMessage);
                    _AllMessagesIds.push(data.MessagesInformation[i].Id);
                }
                if (data.MessagesInformation.length != 0) {
                    _MessagesContainer.scrollTop(_MessagesContainer[0].scrollHeight);
                    NotificationHandler.ShowNewMessageAlert();
                }
            },
            error: function (data) {
                lostConnection();
            }
        });
    }

    function GetChatroomName() {
        return _ChatroomName;
    }

    return {
        init: init,
        Destroy: Destroy,
        GetChatroomId: GetChatroomId,
        GetParentChatroomId: GetParentChatroomId,
        GetChatroomName: GetChatroomName
    }
}

var ChatroomHandler = null;
