var _loadingContainer = $("#loading-container");
var _dimContainer = $("#dim-container");

function ShowLoading() {
    _dimContainer.show();
    _loadingContainer.show();
}

function HideLoading() {
    _loadingContainer.hide();
    _dimContainer.hide();
}

$(function () {
    $(_dimContainer).css({
        'position': 'absolute',
        'width': '100%',
        'height': '90%',
        'background': 'blue',
        'z-index': '1000',
        'opacity': '0.5'
    });
});

$(function () {
    $(_loadingContainer).css({
        'position': 'absolute',
        'left': '25%',
        'top': '25%',
        'z-index' : '1001'
    });
});