var ErrorObject = function() {
    function DisplayErrors(data) {
        if (data.Errors == null || data.Errors.length == 0) {
            return false;
        }

        $(".ui-dialog-content").dialog("close");

        var $errorDialog = $("#error-dialog");

        $errorDialog.html("");
        for (var i = 0; i < data.Errors.length; i++) {
            var $errorMessage = data.Errors[i].ErrorMessage;
            $errorDialog.append("<h2>The following errors were detected: </h2>");
            $errorDialog.append("<p>" + $errorMessage + "</p>");
        }
        $errorDialog.dialog();

        NotificationHandler.HideLoading();
        return true;
    }


    function DisplayCrash(data) {
        document.write(data.responseText);
    }

    return {
        DisplayErrors: DisplayErrors,
        DisplayCrash: DisplayCrash
    }
}

var ErrorHandler = new ErrorObject();
