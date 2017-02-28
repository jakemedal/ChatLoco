var _findChatroomForm = $("#find-chatroom-form");
var _chatroom = null;

_findChatroomForm.on("submit", FindChatroom)

function FindChatroom(e) {
    e.preventDefault();

    ShowLoading();

    var $form = this;

    var $chatroomName = $form.elements.chatroomName.value;
    var $userHandle = $form.elements.userHandle.value;

    var $model = {
        ChatroomName: $chatroomName,
        UserHandle: $userHandle,
        User: GetUser()
    }

    $.ajax({
        type: "POST",
        url: '/Chatroom/Chat',
        data: $model,
        success: function (response) {

            if (DisplayErrors(response.Errors)) {
                return;
            }
            else {
                GetUser().UserHandle = $userHandle;
                $("#content-container").html("").append(response.Data);
                _chatroom = new Chatroom();
            }

            HideLoading();
        },
        error: function (data) {
            document.write(data.responseText);
        }
    });

}

function initMap() {

    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(showPosition);
    } else {
        alert("Geolocation is not supported by this browser.");
    }

    function showPosition(position) {
        var lat = position.coords.latitude;
        var lon = position.coords.longitude
        $("#lat").val(lat);
        $("#lon").val(lon);
        
        getNearbyPlaces(lat, lon);
    }

    function getNearbyPlaces(lat, lon) {
        var loc = {lat: lat, lng: lon};

        map = new google.maps.Map(document.getElementById('map'), {
            center: loc,
            zoom: 19
        });

        var request = {
            location: loc,
            radius: '100',
        };

        service = new google.maps.places.PlacesService(map);
        service.nearbySearch(request, callback);

        function callback(results, status) {
            console.log(results);

            if (status == google.maps.places.PlacesServiceStatus.OK) {
                for (i = 0; i < results.length; i++) {
                    marker = new google.maps.Marker({
                        position: new google.maps.LatLng(results[i].geometry.location.lat(), results[i].geometry.location.lng()),
                        map: map,
                        animation: google.maps.Animation.DROP,
                        title: 'Chatroom: ' + results[i].name
                    });
                }
            }
        }
    }

}