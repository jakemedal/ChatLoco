
var StatusHandlingObject = function() {

    function DisplayStatus(message, title) {
        var $statusDialog = $("#status-dialog")
        $statusDialog.html("");
        $statusDialog.append(message);
        $statusDialog.dialog({
            title: title ? title : "Status Information"
        });
        return true;
    }

    return {
        DisplayStatus: DisplayStatus
    }
};

var StatusHandler = new StatusHandlingObject();