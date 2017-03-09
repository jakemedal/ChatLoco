


$.ajax({
    type: "GET",
    url: '/api/GetRevisionString',
    success: function (data) {
        if (ErrorHandler.DisplayErrors(data)) {
            return;
        }
        $("#revision-string").html(data);
    },
    error: function (data) {
        ErrorHandler.DisplayCrash(data);
    }
});