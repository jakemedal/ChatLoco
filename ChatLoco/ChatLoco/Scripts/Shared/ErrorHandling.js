
function DisplayErrors(errors) {
    if (errors == null || errors.length == 0) {
        return false;
    }
    var $errorDialog = $("#error-dialog")
    $errorDialog.html("");
    for (var i = 0; i < errors.length; i++) {
        var $errorMessage = errors[i].ErrorMessage;
        $("#error-dialog").append("<h2>The following errors were detected: </h2>");
        $("#error-dialog").append("<p>"+$errorMessage+"</p>");
    }
    $errorDialog.dialog();
    return true;
}