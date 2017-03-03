function Account() {

    var _currentUser = null;
    var _accountDialog = $("#account-dialog");
    var _loginFormData = null;

    var _loginForm = null;
    var _loginInformationContainer = null;

    $(document).on("click", CheckUserLogin);

    $("#logout-link").on("click", Logout);

    function Logout(e) {
        e.preventDefault();

        var $model = {
            User: GetUser()
        };

        $.ajax({
            type: "POST",
            url: '/User/Logout',
            data: $model,
            success: function (data) {
                if (error.DisplayErrors(data)) {
                    return;
                }
                _currentUser = null;
                status.DisplayStatus("<p>Logged out successfully.</p>");
            },
            error: function (data) {
                error.DisplayCrash(data);
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
            notifications.ShowLoading();
            $.ajax({
                type: "GET",
                url: '/User/GetLoginForm',
                success: function (data) {
                    _loginFormData = data;
                    _accountDialog.html("").append(_loginFormData);

                    _loginForm = $("#login-form");
                    _loginInformationContainer = $("#login-information-container");

                    _loginForm.on("submit", login);

                    OpenAccountDialog();
                    notifications.HideLoading();
                }
            });
        }
        else {
            OpenAccountDialog();
        }

    }

    function OpenAccountDialog() {
        if (typeof _accountDialog == 'undefined') {
            return;
        }

        _accountDialog.dialog({
            title: "Please Login"
        });
    }


    //this can be handled in plaintext since securing an SSL certificate will automatically encrypt all traffic both ways
    function login(e) {
        e.preventDefault();

        notifications.ShowLoading();

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
                if (error.DisplayErrors(data)) {
                    return;
                }

                var $loginErrorsContainer = $("#login-errors");

                $loginErrorsContainer.html("");

                if (data.LoginErrors.length > 0) {
                    for (var j = 0; j < data.LoginErrors.length; j++) {
                        var $errorMessage = data.LoginErrors[j].ErrorMessage;
                        $loginErrorsContainer.append("<p>" + $errorMessage + "</p>");
                    }
                }
                else {
                    _loginInformationContainer.html("").html("<p>Logged in successfully.</p>");
                    UpdateCurrentUser(data.User);
                    $("#account-navbar").show();
                    $("#username-header").append(GetUser().Username);
                }

                notifications.HideLoading();

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

var account = new Account();
