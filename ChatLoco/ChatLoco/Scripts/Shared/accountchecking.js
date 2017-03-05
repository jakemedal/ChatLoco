var AccountObject = function() {

    var _currentUser = null;
    var _loginDialog = $("#login-dialog");
    var _accountNavbar = $("#account-navbar");
    var _chatroomContainer = $("#chatroom-container");
    var _loginFormData = null;

    var _loginForm = null;
    var _loginInformationContainer = null;

    $(document).on("click", CheckUserLogin);

    $("#logout-link").on("click", Logout);

    function ShowDimBehindDialog() {
        NotificationHandler.ShowDim('100', 'black', '0.7');
    }

    function Logout(e) {
        e.preventDefault();

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

        $.ajax({
            type: "POST",
            url: '/User/Logout',
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
                    NotificationHandler.HideLoading();
                }

            },
            error: function () {
            }
        });

    };

    return {
        GetUser: GetUser,
        CheckUserLogin: CheckUserLogin
    }
}

var AccountHandler = new AccountObject();
