var NotificationObject = function() {
    var _loadingContainer = $("#loading-container");
    var _dimContainer = $("#dim-container");

    function ShowLoading() {
        ShowDim();
        _loadingContainer.show();
    }

    function HideLoading() {
        HideDim();
        _loadingContainer.hide();
    }

    function ShowDim(zIndex, color, opacity) {

        var $color = 'blue';
        if (color) {
            $color = color;
        }

        var $zIndex = '1000'
        if (zIndex) {
            $zIndex = zIndex;
        }

        var $opacity = '0.5'
        if (opacity) {
            $opacity = opacity;
        }

        $(_dimContainer).css({
            'position': 'absolute',
            'width': '100%',
            'height': '90%',
            'background': $color,
            'z-index': $zIndex,
            'opacity': $opacity
        });

        _dimContainer.show();
    }

    function HideDim() {
        _dimContainer.hide();
    }

    $(function () {
        $(_loadingContainer).css({
            'position': 'absolute',
            'left': '25%',
            'top': '25%',
            'z-index': '1001'
        });
    });

    function ShowNewMessageAlert() {
        if (ChatroomHandler && AccountHandler.IsIdle()) {
            document.title = " (!) " + ChatroomHandler.GetChatroomName();
        }
    }

    function HideNewMessageAlert() {
        if (ChatroomHandler) {
            document.title = ChatroomHandler.GetChatroomName();
        }
    }

    return {
        ShowLoading: ShowLoading,
        HideLoading: HideLoading,
        ShowDim: ShowDim,
        HideDim: HideDim,
        ShowNewMessageAlert: ShowNewMessageAlert,
        HideNewMessageAlert: HideNewMessageAlert
    }
}

var NotificationHandler = new NotificationObject();