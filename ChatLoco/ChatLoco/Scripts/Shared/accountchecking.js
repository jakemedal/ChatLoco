var AccountObject = function() {

    var _currentUser = null;
    var _loginDialog = $("#login-dialog");
    //reference to the settings dialog partial view
    var _settingsDialog = $("#settings-dialog");
    var _disconnectedDialog = $("#disconnected-dialog");
    var _accountNavbar = $("#account-navbar");
    var _chatroomContainer = $("#chatroom-container");

    var _loginFormData = null;
    //Settings dialog data
    var _settingsFormData = null;
    var _settingsForm = null;
    var _settingsInformationContainer = null;

    var _loginForm = null;
    var _loginInformationContainer = null;
    var _idle = false;
    var _idleCheckInterval = null

    $(document).on("click", CheckUserLogin);

    //whenever the user clicks the settings tab open the settings dialog
    $("#settings-link").on("click", ShowSettings);

    $("#logout-link").on("click", LogoutClicked);

    $("#find-chatroom-link").on("click", FindChatroomLinkClicked);

    $("#check-server-link").on("click", CheckServerConnection);

    function FindChatroomLinkClicked (e) {
        e.preventDefault();

        NotificationHandler.ShowLoading();

        $.ajax({
            type: "GET",
            url: '/Chatroom/GetFindChatroom',
            success: function (data) {
                $("#chatroom-container").html("").append(data);
                if (findChatroom != null) {
                    findChatroom.Destroy();
                }
                if (ChatroomHandler != null) {
                    ChatroomHandler.Destroy();
                    ChatroomHandler = null;
                }
                findChatroom = new FindChatroom();
                findChatroom.init();
                NotificationHandler.HideLoading();
            },
            error: function (data) {
                ErrorHandler.DisplayCrash(data);
            }
        });

    }

    function ShowDimBehindDialog() {
        NotificationHandler.ShowDim('black', '0.7');
    }

    function LogoutClicked(e) {
        e.preventDefault();
        Logout(false);
    }

    function DirtyLogout() {
        Logout(true);
    }

    function Logout(isDirty) {

        var $parentChatroomId = -1;
        var $chatroomId = -1;

        if (ChatroomHandler) {
            $parentChatroomId = ChatroomHandler.GetParentChatroomId();
            $chatroomId = ChatroomHandler.GetChatroomId();
        }

        var $model = {
            User: GetUser(),
            ParentChatroomId: $parentChatroomId,
            ChatroomId: $chatroomId
        };

        var $url = '/User/Logout';

        if (isDirty == true) {
            $url = '/User/DirtyLogout';
        }

        $.ajax({
            type: "POST",
            url: $url,
            data: $model,
            success: function (data) {
                if (ErrorHandler.DisplayErrors(data)) {
                    return;
                }
                _currentUser = null;

                $.ajax({
                    type: "GET",
                    url: '/Chatroom/GetFindChatroom',
                    success: function (data) {
                        $("#chatroom-container").html("").append(data);
                        if (findChatroom != null) {
                            findChatroom.Destroy();
                        }
                        findChatroom = new FindChatroom();
                        findChatroom.init();
                    },
                    error: function(data){
                        ErrorHandler.DisplayCrash(data);
                    }
                });

                StatusHandler.DisplayStatus("<p>Logged out successfully.</p>");
                ShowDimBehindDialog();
                _accountNavbar.hide();
            },
            error: function (data) {
                ErrorHandler.DisplayCrash(data);
            }
        });

    }

    function GetUser() {
        return _currentUser;
    }

    function IsUserLoggedIn() {
        return (_currentUser != null);
    }

    function UpdateCurrentUser(user) {
        _currentUser = user;
    }

    function CheckUserLogin(e) {

        if (IsUserLoggedIn()) {
            return;
        }

        if (!$(".ui-dialog").is(":visible")) {
            if (typeof e != 'undefined') {
                e.preventDefault();
            }
        }

        if (_loginFormData == null) {
            NotificationHandler.ShowLoading();
            $.ajax({
                type: "GET",
                url: '/User/GetLoginForm',
                success: function (data) {
                    _loginFormData = data;
                    _loginDialog.html("").append(_loginFormData);

                    _loginForm = $("#login-form");
                    _loginInformationContainer = $("#login-information-container");

                    _loginForm.on("submit", login);

                    NotificationHandler.HideLoading();
                    OpenLoginDialog();
                }
            });
        }
        else {
            OpenLoginDialog();
        }

    }
    
    //Render the partial view
    function ShowSettings(e) {
        //stop the default action of the event so we can manually call out function
        e.preventDefault();

        if (!$(".ui-dialog").is(":visible")) {
            if (typeof e != 'undefined') {
                e.preventDefault();
            }
        }

        //the dialog hasn't been populated yet - do it
        if (_settingsFormData == null) {
            NotificationHandler.ShowLoading();
            $.ajax({
                type: "GET",
                url: '/User/GetSettingsForm',
                success: function (data) {
                    _settingsFormData = data;
                    _settingsDialog.html("").append(_settingsFormData);

                    _settingsForm = $("#settings-form");
                    _settingsInformationContainer = $("#settings-information-container");
                    $("#email-input").val(GetUser().Settings.Email);
                    $("#default-handle-input").val(GetUser().Settings.DefaultHandle);

                    _settingsDialog.on("dialogclose", ManualCloseSettingsDialog);//in case the user closes the dialog with the X, undim the background
                    _settingsForm.on("submit", updateSettings);

                    NotificationHandler.HideLoading();
                    OpenSettingsDialog();
                }
            });
        }
        else {
            OpenSettingsDialog();
        }

    }

    //form submission - update user settings
    function updateSettings(e) {
        e.preventDefault();//stop default action

        NotificationHandler.ShowLoading();
        //Store the form of the object that called the method
        var $form = this;

        var $updatedEmail = $form.elements.Email.value;
        var $updatedDefaultHandle = $form.elements.DefaultHandle.value;

        //UpdateSettingsRequestModel 
        var $model = {
            Settings: {
                DefaultHandle: $updatedDefaultHandle,
                Email: $updatedEmail
            },
            Username: GetUser().Username
        };

        $.ajax({
            type: "POST",
            url: '/User/UpdateSettings',
            data: $model,
            success: function (data) {
                if (ErrorHandler.DisplayErrors(data)) {
                    return;
                }
                var $settingsErrorsContainer = $("#settings-errors");

                $settingsErrorsContainer.html("");

                if (data.SettingsErrors.length > 0) {
                    for (var j = 0; j < data.SettingsErrors.length; j++) {
                        var $errorMessage = data.SettingsErrors[j].ErrorMessage;
                        $settingsErrorsContainer.append("<p>" + $errorMessage + "</p>");
                    }
                    NotificationHandler.HideLoading();
                    ShowDimBehindDialog();
                }
                else {//inform user we were successful and exit
                    CloseSettingsDialog();
                    StatusHandler.DisplayStatus("<p>User Settings saved successfully!</p>");
                    UpdateCurrentUser(data.User);
                    _accountNavbar.show();
                    NotificationHandler.HideLoading();
                }

            },
            error: function () {
            }
        });

    }

    function OpenSettingsDialog() {
        if (typeof _settingsDialog == 'undefined') {//failsafe incase not initialized
            return;
        }

        ShowDimBehindDialog();
        _settingsDialog.dialog({//might be redundant
            title: "User Settings"
        });
    }

    function ManualCloseSettingsDialog() {
        CloseSettingsDialog();
        _accountNavbar.show();
        NotificationHandler.HideLoading();
    }

    function CloseSettingsDialog() {
        if (typeof _settingsDialog == 'undefined') {//failsafe
            return;
        }

        _settingsDialog.dialog("close");
    }

    var _allowCloseDisconnectedDialog = false;
    var _reconnectedCallback = null;

    function CheckServerConnection(e) {
        e.preventDefault();

        NotificationHandler.ShowLoading();

        $.ajax({
            type: "GET",
            url: '/api/Ping',
            success: function (data) {
                _allowCloseDisconnectedDialog = true;
                _disconnectedDialog.dialog("close");
                _allowCloseDisconnectedDialog = false;
                if (_reconnectedCallback) {
                    _reconnectedCallback();
                }
                NotificationHandler.HideLoading();
            },
            error: function (data) {
                NotificationHandler.HideLoading();
                ShowDimBehindDialog();
            }
        });
    }

    function OpenDisconnectedDialog(reconnectCallback) {
        ShowDimBehindDialog();

        if (reconnectCallback) {
            _reconnectedCallback = reconnectCallback;
        }

        _disconnectedDialog.dialog({
            title: "Server Connection Lost",
            close: function () {
                if (!_allowCloseDisconnectedDialog) {
                    OpenDisconnectedDialog();
                }
            }
        });
    }

    function OpenLoginDialog() {
        if (typeof _loginDialog == 'undefined') {
            return;
        }

        ShowDimBehindDialog();
        _loginDialog.dialog({
            title: "Please Login"
        });
    }

    function CloseLoginDialog() {
        if (typeof _loginDialog == 'undefined') {
            return;
        }

        _loginDialog.dialog("close");
    }

    //this can be handled in plaintext since securing an SSL certificate will automatically encrypt all traffic both ways
    function login(e) {
        e.preventDefault();

        NotificationHandler.ShowLoading();

        var $form = this;

        var $username = $form.elements.Username.value;
        var $password = $form.elements.Password.value;

        var $model = {
            Username: $username,
            Password: $password
        };

        $.ajax({
            type: "POST",
            url: '/User/Login',
            data: $model,
            success: function (data) {
                if (ErrorHandler.DisplayErrors(data)) {
                    return;
                }

                var $loginErrorsContainer = $("#login-errors");

                $loginErrorsContainer.html("");

                if (data.LoginErrors.length > 0) {
                    for (var j = 0; j < data.LoginErrors.length; j++) {
                        var $errorMessage = data.LoginErrors[j].ErrorMessage;
                        $loginErrorsContainer.append("<p>" + $errorMessage + "</p>");
                    }
                    NotificationHandler.HideLoading();
                    ShowDimBehindDialog();
                }
                else {
                    CloseLoginDialog();
                    StatusHandler.DisplayStatus("<p>Logged in successfully.</p>");
                    UpdateCurrentUser(data.User);
                    _accountNavbar.show();
                    $("#username-header").html("").append(GetUser().Username);
                    if (GetUser().Role === 1) {
                        $("#user-management").show();
                    }
                    NotificationHandler.HideLoading();
                }

            },
            error: function () {
            }
        });

    };

    var _idleCheck = false;

    function MarkAsIdle() {
        if (_idleCheck) {
            _idle = true;
        }
        else {
            _idleCheck = true;
        }
    }

    function IsIdle() {
        return _idle;
    }

    function UpdateIdle() {
        _idle = false;
        _idleCheck = false;
        NotificationHandler.HideNewMessageAlert();
    }

    function init() {
        _idleCheckInterval = setInterval(MarkAsIdle, 1000);

        $(document).on("mousemove", UpdateIdle);
    }

    function destroy() {
        if (_idleCheckInterval) {
            clearInterval(_idleCheckInterval);
        }
    }

    return {
        OpenDisconnectedDialog: OpenDisconnectedDialog,
        GetUser: GetUser,
        CheckUserLogin: CheckUserLogin,
        UpdateIdle: UpdateIdle,
        IsIdle: IsIdle,
        Logout: Logout,
        DirtyLogout: DirtyLogout,
        init: init,
        destroy: destroy
    }
}

var AccountHandler = new AccountObject();

AccountHandler.init();
