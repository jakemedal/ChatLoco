

var _loginForm = $("#login-form");

_loginForm.on("submit", login);


//this can be handled in plaintext since securing an SSL certificate will automatically encrypt all traffic both ways
function login(e){
    e.preventDefault();

    var $form = this;

    var $username = $form[0].value;
    var $password = $form[1].value;

    var $model = {
        Username: $username,
        Password: $password
    };

    $.ajax({
        type: "POST",
        url: '/User/Login',
        data: $model,
        success: function (data) {

            if (DisplayErrors(data.Errors)) {
                return;
            }
            else {
                //TODO redirect to account home page
            }

        },
        error: function () {
        }
    });

};
