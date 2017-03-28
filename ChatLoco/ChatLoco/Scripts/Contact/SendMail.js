var contactForm = $("#contact-form");

contactForm.on("submit", sendMail);

function sendMail(e) {
    NotificationHandler.ShowLoading();
    e.preventDefault();
    var $form = this;

    var $name = $form[0].value;
    var $email = $form[1].value;
    var $message = $form[2].value;
    var $redirectUrl = $form[3].value;
    var $model = {
        subject: "ChatLoco user " + $name + " needs your help.",
        fromEmail: $email,
        toEmail: "chatloco.contact@gmail.com",
        Message: $message + "\nReach them at: "+ $email
    };
    $.ajax({
        type: "POST",
        url: '/Contact/send',
        data: $model,
        success: function (response) {
            if (ErrorHandler.DisplayErrors(response)) {
                return;
            }

            StatusHandler.DisplayStatus('<p>' + response.Message + '</p>');

            setTimeout(function () {
                window.location.replace($redirectUrl);
            }, 3000);

            NotificationHandler.HideLoading();
        },
        error: function (data) {
            ErrorHandler.DisplayCrash(data);
        }
    });

};