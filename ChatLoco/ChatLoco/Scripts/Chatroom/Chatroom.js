
var ChatroomObject = function () {

    var _AllMessages = [];
    var _AllMessagesIds = [];
    var _AllPrivateChatroomButtons = [];
    var _AllUserElements = [];

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

    //TODO add style objects (toggle, dropdown)
    var _boldChoice = false;
    var _italicChoice = false;
    var _Style = "style=\"word-wrap:break-word; \"";
    var _Color = "white";

    var _UserInfoDialog = $("#user-info-dialog");
    var _UserForm = null;
    var _UserFormData = null;
    var _UserInformationContainer = null;

    var mouseDownScrollbar = 0;
    $("#MessagesPaneContainer #style-1").on('mousedown', function() {
        ++mouseDownScrollbar;
    }).on('mouseup', function () {
        --mouseDownScrollbar;
    });

    var Destroy = function () {
        if (_GetChatroomInformationInterval != null) {
            clearInterval(_GetChatroomInformationInterval);
        }

        if (_GetNewMessagesInterval != null) {
            clearInterval(_GetNewMessagesInterval);
        }

        $("#create-private-chatroom-dialog-button").off("click", OpenCreatePrivateChatroomDialog);

        $("#ComposeForm").off("submit", SendComposedMessage);
        $("#ParentChatroomButton").off("click", ChatroomClicked);
        $("#private-chatroom-request-form").off("submit", ChatroomRequestFormSubmit);

        NotificationHandler.ShowLoading();

        var $model = {
            UserId: AccountHandler.GetUser().Id,
            ParentId: _ParentChatroomId,
            ChatroomId: _ChatroomId
        };

        $.ajax({
            type: "POST",
            url: '/Chatroom/LeaveChatroom',
            data: JSON.stringify($model),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (data) {

            },
            error: function (data) {

            }
        });
    }

    var init = function () {

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
        $("#private-chatroom-request-form").on("submit", ChatroomRequestFormSubmit);

        $("#bold-toggle").on("click", IsBold);
        $("#italic-toggle").on("click", IsItalic);
        $("#select-color").on("click", SetColor);

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

        var $newChatroomId = e.target.getAttribute("value");

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

                while (_AllPrivateChatroomButtons.length != 0) {
                    var privateChatroomButton = _AllPrivateChatroomButtons.pop();
                    $(privateChatroomButton).off("click", ChatroomClicked);
                }

                while (_AllUserElements.length != 0) {
                    var userElement = _AllUserElements.pop();
                    $(userElement).off("click", OpenUserInfoDialog);
                }

                _UsersContainer.html("");
                for (var i = 0; i < data.UsersInformation.length; i++) {
                    var $user = data.UsersInformation[i];
                    var $username = data.UsersInformation[i].Username;
                    var $id = data.UsersInformation[i].Id;

                    var p = document.createElement("p");

                    var userElement = document.createElement("a");
                    userElement.setAttribute('value', $id);
                    userElement.textContent = $username;
                    $(userElement).on("click", OpenUserInfoDialog);
                    _AllUserElements.push(userElement);

                    p.appendChild(userElement);

                    _UsersContainer[0].appendChild(p);
                }

                _SubChatroomsList.html("<br/>");
                for (var i = 0; i < data.PrivateChatroomsInformation.length; i++) {
                    var $subChatroomName = data.PrivateChatroomsInformation[i].Name;
                    var $subChatroomId = data.PrivateChatroomsInformation[i].Id;
                    var $buttonType = 'secondary';
                    if ($subChatroomId === _ChatroomId) {
                        $buttonType = 'primary';
                    }

                    var p = document.createElement("p");

                    var privateChatroomButton = document.createElement("button");
                    privateChatroomButton.setAttribute('value', $subChatroomId);
                    privateChatroomButton.setAttribute('type', 'button');
                    privateChatroomButton.setAttribute('class', 'btn btn-' + $buttonType);
                    privateChatroomButton.textContent = $subChatroomName;
                    $(privateChatroomButton).on("click", ChatroomClicked);
                    _AllPrivateChatroomButtons.push(privateChatroomButton);

                    p.appendChild(privateChatroomButton);

                    _SubChatroomsList[0].appendChild(p);

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
                    StatusHandler.DisplayStatus($responseMessage, "Private Chatroom Creation Information");
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
        if (_boldChoice == true && _italicChoice == false) {
            var $style = "style=\"word-wrap:break-word; font-weight:bold; color:"+ _Color +"; \"";
        }
        else if(_boldChoice == false && _italicChoice == true) {
            var $style = "style=\"word-wrap:break-word; font-style: italic; color:" + _Color + "; \"";
        }
        else if (_boldChoice == true && _italicChoice == true) {
            var $style = "style=\"word-wrap:break-word; font-weight:bold; font-style: italic; color:" + _Color + "; \"";
        }
        else {
            var $style = "style=\"word-wrap:break-word; color:" + _Color + "; \"";
        }
       
        _Style = $style;

        var $model = {
            Message: $message,
            ChatroomId: _ChatroomId,
            UserId: _UserId,
            ParentChatroomId: _ParentChatroomId,
            MessageStyle: _Style,

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
    function IsBold(e) {
        e.preventDefault()

        if (_boldChoice == false) {
            $("#bold-toggle").removeClass("btn-primary");
            $("#bold-toggle").addClass("btn-secondary");
            _boldChoice = true;
            
        }
        else {
            $("#bold-toggle").removeClass("btn-secondary");
            $("#bold-toggle").addClass("btn-primary");
            _boldChoice = false;
            return _boldChoice;
        }
    }
    function IsItalic(e) {
        e.preventDefault();

        if (_italicChoice == false) {
            $("#italic-toggle").removeClass("btn-primary");
            $("#italic-toggle").addClass("btn-secondary");
            _italicChoice = true;
            return _italicChoice;
        }
        else {
            $("#italic-toggle").removeClass("btn-secondary");
            $("#italic-toggle").addClass("btn-primary");
            _italicChoice = false;
            return _italicChoice;
        }
    }
    function SetColor(e) {
        e.preventDefault();
        $("#select-color").removeClass(toString(e.target.getAttribute("value")));
        _Color = e.target.getAttribute("value");
        $("#select-color").addClass(toString(e.target.getAttribute("value")));
    }
    function OpenUserInfoDialog(e) {
        e.preventDefault();
        
        var $id = e.target.getAttribute("value");

        var $model = {
            Id: $id
        };
       
        $.ajax({
            type: "POST",
            url: '/User/GetUserInfo',
            data: $model,
            success: function (data) {

                if (ErrorHandler.DisplayErrors(data)) {
                    return;
                }
                StatusHandler.DisplayStatus(data, "User Information");
                 
                 _UserInfoDialog.html("").append(data);

                _UserForm = $("#user-info-form");
                _UserInformationContainer = $("#user-information-container");
             
                
                NotificationHandler.HideLoading();
                OpenUserDialog();
            },
            error: function () {
            }
        });
        

    }
    function OpenUserDialog() {
        if (typeof _UserInfoDialog == 'undefined') {
            return;
        }
        _UserInfoDialog.dialog({
            title: "User Information"
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
            ParentChatroomId: _ParentChatroomId,
            MessageStyle: _Style,
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
                    var $messageStyle = data.MessagesInformation[i].MessageStyle
                    
                    _MessagesContainer.append("<p " + $messageStyle +" > " +$newMessage + "</p>");
                    
                    _AllMessages.push($newMessage);
                    _AllMessagesIds.push(data.MessagesInformation[i].Id);
                }
                if (data.MessagesInformation.length != 0) {

                    if (mouseDownScrollbar == 0) { // If scrollbar is not being used, change scroll positioning to see new message
                        $('#MessagesPaneContainer #style-1').stop().animate({
                            scrollTop: $('#MessagesPaneContainer #style-1')[0].scrollHeight
                        }, 800);
                    }

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
