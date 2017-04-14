var contactForm = $("#manage-users-form");

contactForm.on("submit", MakeAdmin);

function MakeAdmin(e) {
    NotificationHandler.ShowLoading();
    e.preventDefault();
    var $form = this;
    var $action = $("input[name='action']:checked").val();
    var $name = $form[0].value;
    var $model = {
        Username: $name,
        Action: $action
    };
    $.ajax({
        type: "POST",
        url: '/ManageUsers/HandleAdminAction',
        data: $model,
        success: function (response) {
            if (ErrorHandler.DisplayErrors(response)) {
                return;
            }

            StatusHandler.DisplayStatus('<p>' + response.Message + '</p>');

            NotificationHandler.HideLoading();
        },
        error: function (data) {
            ErrorHandler.DisplayCrash(data);
        }
    });

};