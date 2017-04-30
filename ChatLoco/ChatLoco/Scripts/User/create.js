

var _createForm = $("#create-user-form");

_createForm.on("submit", createUser);


//this can be handled in plaintext since securing an SSL certificate will automatically encrypt all traffic both ways
function createUser(e) {
    NotificationHandler.ShowLoading();

    e.preventDefault();

    var $form = this;

    var $username = $form[0].value;
    var $email = $form[1].value;
    var $password = $form[2].value;
    var $confirmPassword = $form[3].value;
    var $redirectUrl = $form[4].value;

    if ($password != $confirmPassword) {
        NotificationHandler.HideLoading();
        StatusHandler.DisplayStatus("<p>Passwords do not match.</p>");
        $form[2].value = "";
        $form[3].value = "";
        return;
    }

    var $model = {
        Username: $username,
        Password: $password,
        Email: $email
    };

    $.ajax({
        type: "POST",
        url: '/User/CreateUser',
        data: $model,
        success: function (data) {

            if (ErrorHandler.DisplayErrors(data)) {
                $form[2].value = "";
                $form[3].value = "";
                return;
            }
            else {
                var msg = "<p>User " + data.Username + " created successfully</p>" +
                    "<p>with email address " + data.Email + "</p>"+
                    "<p>You will be directed to the homepage in five seconds.</p>";
                setTimeout(function () {
                    window.location = $redirectUrl;
                }, 5000);
                StatusHandler.DisplayStatus(msg)
            }

            NotificationHandler.HideLoading();
        },
        error: function (data) {
            ErrorHandler.DisplayCrash(data);
        }
    });

};
