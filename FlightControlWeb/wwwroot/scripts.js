//array flights
const flights = new Array();
//configuring map and marker icon
var mymap = L.map('mapid').setView([51.505, -0.09], 2);
L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
}).addTo(mymap);
var MarkerIcon = L.Icon.extend({
    options: {
        customId: "",
        iconSize: [18, 35],
    }
});
var bluePlane = new MarkerIcon({ iconUrl: 'images/airplane1.png' }),
    blackPlane = new MarkerIcon({ iconUrl: 'images/plane.png' });
var markers = new Array();
var counter = 0;
var paths = new Array();
var previousPath = -1;
var previousRow = null;
var table;
//ajax post request for adding new flight plan
const input = document.querySelector('input[type="file"]');
input.addEventListener('change', function (e) {
    const reader = new FileReader();
    reader.onload = function () {
        $.ajax({
            url: "https://localhost:44389/api/FlightPlan",
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data: reader.result,
            success: function (data) {
            },
            error: function (response) {
            },
        });
    }
    if (input.files[0].type === "application/json") {
        reader.readAsText(input.files[0]);
    }
}, false)
//clicking on marker
function onMarkerClick(e) {
    var id;
    for (var i = 0; i < counter; i++) {
        var local = markers[i].getLatLng().toString().localeCompare(e.latlng.toString());
        //if the click was in plane in index i, execute click logic on him
        if (local === 0) {
            id = markers[i].options.customId;
            clickLogic(id);
            previousPath = i;
        }
    }
}
//adding event handlers of clicking and deleting row for new row in table
function addRowHandlers(counter) {
    var tr = "#" + counter;
    var del = "#del" + counter;
    $(document).on("click", tr, function () {
        onRowClick($(this));
    });
    $(document).on("click", del, function (e) {
        deleteRow($(this));
        e.stopPropagation();
    });
}
//cancel event handlers of row in table
function cancelRowHandlers(idRow) {
    var tr = "#" + idRow;
    var del = "#del" + idRow;
    $(document).off("click", tr);
    $(document).off("click", del);
}
//clicking row in table
function onRowClick(e) {
    table = document.getElementById('myFlights');
    var trid = e.attr('id');
    var id = table.rows[trid].cells[1].innerHTML
    console.log("onRow" + id);
    clickLogic(id);
    previousPath = trid;
}
//click logic when clicking marker or row; showing path, change plane color, show flight details
function clickLogic(id) {
    table = document.getElementById('myFlights');
    var numRows = table.rows.length;
    for (var index = 0; index < counter; index++) {
        if (index == previousPath) {
            markers[index].setIcon(blackPlane);
            document.getElementById("flightDetails").innerHTML = "";
            mymap.removeLayer(paths[index]);
            table.rows[previousRow].style.backgroundColor = "white";
        }
    }
    for (var i = 0; i < counter; i++) {
        if (markers[i].options.customId === id) {
            $("#flightDetails").html("");
            markers[i].setIcon(bluePlane);
            var flightUrl = "../api/FlightPlan/" + id;
            $.getJSON(flightUrl).done(function (data) {
                var lastSegm = data.segments.length - 1;
                $("#flightDetails").html("<tr><td>" + id + "</td>" +
                    "<td>" + data.passenger + "</td>" + "<td>" + data.company_name + "</td>" +
                    "<td>" + data.initial_Location.date_time + "</td>" +
                    "<td>{" + data.initial_Location.latitude + "," + data.initial_Location.longitude + "}</td>" +
                    "<td>" + data.initial_Location.date_time + "</td>" +
                    "<td>{" + data.segments[lastSegm].latitude + "," + data.segments[lastSegm].longitude + "}</td>" +
                    "</tr>");
            });
            paths[i].addTo(mymap);
            for (var ind = 0; ind < numRows; ind++) {
                var row = table.rows[ind];
                if (row.cells[1].innerHTML === id) {
                    row.style.backgroundColor = "LightSkyBlue";
                    previousRow = ind;
                }
            }
        }
    }
}
//clicking the map logic
function onMapClick() {
    $("#flightDetails").html("");
    for (var index = 0; index < counter; index++) {
        markers[index].setIcon(blackPlane);
        mymap.removeLayer(paths[index])
    }
    table = document.getElementById('myFlights');
    var numRows = table.rows.length;
    for (var ind = 0; ind < numRows; ind++) {
        var row = table.rows[ind];
        row.style.backgroundColor = "white";
    }
}
mymap.on('click', onMapClick);
//deleting flight from my server flights
function deleteRow(e) {
    table = document.getElementById('myFlights');
    var string = e.attr('id').toString();
    var trid = string.substr(3,1);
    console.log("tr id: " + trid);
    var id = table.rows[trid].cells[1].innerHTML
    $(document).off("click", e);
    $.ajax({
        url: "https://localhost:44389/api/Flights/" + id,
        type: "DELETE",
        success: function (data) {
            console.log("deleted" + data);
            cancelRowHandlers(table.rows.length - 1);
            table.rows[trid].remove();
            mymap.removeLayer(markers[trid]);
            mymap.removeLayer(paths[trid]);
            var content = $("#flightDetails").html();
            if (content.includes(id)) {
                $("#flightDetails").html("");
            }
            flights.splice(trid, 1);
            markers.splice(trid, 1);
            paths.splice(trid, 1);
            $('#myFlights > tr').each(function () {
                var thisId = trid;
                var prevId = $(this).attr('id');
                if (prevId > thisId) {
                    $(this).attr('id', thisId);
                    $('#del' + prevId).attr('id', "del" + thisId);
                    thisId++;
                }
            });
            counter--;
        },
        error: function (response) {
            console.log("error");
        },
    });
}
//updating flights according the moment
function yourFunction() {
    setTimeout(yourFunction, 2000);
    var date = new Date();
    var now_utc = Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
        date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
    var flightUrl1 = "../api/Flights?relative_to=" + new Date(now_utc).toISOString();
    $.getJSON(flightUrl1, function (data) {
        data.forEach(function (flight) {
            const found = flights.some(el => el.id === flight.flight_id);
            if (!found) {
                flights.push({ id: flight.flight_id, company: flight.company_name });
                markers.push(L.marker([flight.latitude, flight.longitude],
                    { customId: flight.flight_id, icon: blackPlane }));
                markers[counter].addTo(mymap);
                markers[counter].on('click', onMarkerClick);
                var flightUrl = "../api/FlightPlan/" + flight.flight_id;
                var latlngs = new Array();
                $.getJSON(flightUrl).done(function (data) {
                    latlngs.push([data.initial_Location.latitude, data.initial_Location.longitude]);
                    for (var j = 0; j < data.segments.length; j++) {
                        latlngs.push([data.segments[j].latitude, data.segments[j].longitude]);
                    }
                    paths.push(L.polyline(latlngs, { color: 'blue' }));
                    counter++;
                });
            }
            else {
                for (var i = 0; i < counter; i++) {
                    if (flights[i].id === flight.flight_id) {
                        markers[i].setLatLng([flight.latitude, flight.longitude]);
                    }
                }
            }
            var content = $("#myFlights").html();
            if (!content.includes(flight.flight_id)) {
                $("#myFlights").append("<tr id=" + counter + "><td><button id=" + "del" + counter
                    + "><img src=" + 'images/trash.png' + "></button></td>"
                    + "<td>" + flight.flight_id + "</td>" +
                    "<td>" + flight.company_name + "</td></tr>");
                addRowHandlers(counter);
            }
        });
    });
}
yourFunction();