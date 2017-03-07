

var PartialViewHandlerObject = function () {

    function GetAndRenderPartialView(requestUrl, requestType, model, container, successCallback) {
        NotificationHandler.ShowLoading();
        
        $.ajax({
            type: requestType,
            url: requestUrl,
            data: JSON.stringify(model),
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            success: function (response) {
                if (ErrorHandler.DisplayErrors(response)) {
                    return;
                }

                container.html("").append(response.Data);

                if(successCallback) {
                    successCallback();
                }

                NotificationHandler.HideLoading();
            },
            error: function (data) {
                ErrorHandler.DisplayErrors(data);
            }
        });
    }

    return {
        GetAndRenderPartialView: GetAndRenderPartialView
    }
}

var PartialViewHandler = new PartialViewHandlerObject();

