var _currentUser = null;

function IsUserLoggedIn() {
    if (_currentUser != null) {
        return true;
    }
    else {
        return false;
    }
}

function ResetUser() {
    if (_currentUser != null) {

    }
    _currentUser = null;
}

function CheckUserLogin() {
    var $dialog = $("#account-dialog");

    $.ajax({
        type: "GET",
        url: '/User/GetLoginForm',
        success: function (data) {

            $dialog.html("").append(data);
            $dialog.dialog({
                title: "Please Login"
            });
        }
    });
}