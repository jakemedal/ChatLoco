
function DisplayErrors(errors) {
    if (errors == null || errors.length == 0) {
        return false;
    }

    $(".ui-dialog-content").dialog("close");

    var $errorDialog = $("#error-dialog");

    $errorDialog.html("");
    for (var i = 0; i < errors.length; i++) {
        var $errorMessage = errors[i].ErrorMessage;
        $errorDialog.append("<h2>The following errors were detected: </h2>");
        $errorDialog.append("<p>" + $errorMessage + "</p>");
    }
    $errorDialog.dialog();
    return true;
}
