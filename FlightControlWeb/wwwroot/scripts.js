// Empty JS for your own code to be here
const input = document.querySelector('input[type="file"]');
input.addEventListener('change', function (e) {
    console.log(input.files);
    const reader = new FileReader();
    reader.onload = function () {
        console.log(reader.result);
    }
    if (input.files[0].type === "application/json") {
        reader.readAsText(input.files[0]);
    }
}, false)

$(function () {
    var mymap = L.map('mapid').setView([51.505, -0.09], 3);
    L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
    }).addTo(mymap);
    //planes icons
    var blackPlane = L.icon({
        iconUrl: 'images/plane.png',
        iconSize: [22, 45], // size of the icon
    });
    var bluePlane = L.icon({
        iconUrl: 'images/airplane1.png',
        iconSize: [22, 45], // size of the icon
    });
    var markers = new Array();
    var counter = 0;
    markers[counter] = L.marker([51.0, -0.09], { icon: blackPlane }).addTo(mymap);
    counter++;
    markers[counter] = L.marker([43.5, 10.01], { icon: blackPlane }).addTo(mymap);
    //clicking a plane logic
    function onMarkerClick(e) {
        for (var i = 0; i <= counter; i++) {
            var local = markers[i].getLatLng().toString().localeCompare(e.latlng.toString());
            //if the click was in plane in index i, change his icon
            if (local == 0) {
                markers[i].setIcon(bluePlane);
            } else {
                markers[i].setIcon(blackPlane);
            }
        }
        var text = "plane was clicked";
        document.getElementById("flightDetails").innerHTML = text;
    }
    //clicking the map logic
    function onMapClick() {
        for (var i = 0; i <= counter; i++) {
            markers[i].setIcon(blackPlane);
        }
        document.getElementById("flightDetails").innerHTML = "";
    }
    //configuring on click function to each marker
    for (var i = 0; i <= counter; i++) {
        markers[i].on('click', onMarkerClick);
    }
    mymap.on('click', onMapClick);
});