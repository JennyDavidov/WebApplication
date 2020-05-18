// Empty JS for your own code to be here
$(function () {
    var mymap = L.map('mapid').setView([51.505, -0.09], 13);
    L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
    }).addTo(mymap);
    var planeIcon = L.icon({
        iconUrl: 'images/plane.png',
        iconSize: [38, 65], // size of the icon
    });
    var marker = L.marker([51.5, -0.09], { icon: planeIcon }).addTo(mymap);
    var popup = L.popup();
    function onMapClick(e) {
        popup
            .setLatLng(e.latlng)
            .setContent("You clicked the map at " + e.latlng.toString())
            .openOn(mymap);
        document.getElementById("flightDetails").innerHTML = "plane was clicked";
    }
    marker.on('click', onMapClick);
});