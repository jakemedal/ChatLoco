﻿
function DisplayStatus(message) {
    var $statusDialog = $("#status-dialog")
    $statusDialog.html("");
    $statusDialog.append("<h2>Status Information </h2>");
    $statusDialog.append(message);
    $statusDialog.dialog();
    return true;
}