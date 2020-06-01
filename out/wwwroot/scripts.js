//array my flights
const flights = new Array();
//array other flights
const otherFlights = new Array();
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
var otherMarkers = new Array();
var otherCounter = 0;
var otherPaths = new Array();
var previousPath = -1;
var previousRow = -1;
var table;
var otherPreviousPath = -1;
var otherPreviousRow = -1;
//ajax post request for adding new flight plan
const input = document.querySelector('input[type="file"]');
input.addEventListener('change', function (e) {
    const reader = new FileReader();
    reader.onload = function () {
        $.ajax({
            url: "../api/FlightPlan",
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
            previousRow = i;
            return;
        }
    }
    for (var i = 0; i < otherCounter; i++) {
        var local = otherMarkers[i].getLatLng().toString().localeCompare(e.latlng.toString());
        //if the click was in plane in index i, execute click logic on him
        if (local === 0) {
            id = otherMarkers[i].options.customId;
            otherClickLogic(id);
            otherPreviousPath = i;
            otherPreviousRow = i;
            return;
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
//adding event handlers of clicking row for new row in table otherFlights
function addOtherRowHandlers(counter) {
    var tr = "#o" + counter;
    $(document).on("click", tr, function () {
        otherRowClick($(this));
    });
}
//cancel event handlers of row in table
function cancelRowHandlers(idRow) {
    var tr = "#" + idRow;
    var del = "#del" + idRow;
    $(document).off("click", tr);
    $(document).off("click", del);
}
//clicking row in table myFlights
function onRowClick(e) {
    table = document.getElementById('myFlights');
    var trid = e.attr('id');
    var id = table.rows[trid].cells[1].innerHTML
    clickLogic(id);
    previousPath = trid;
    previousRow = trid;
}
//clicking row in table otherFlights
function otherRowClick(e) {
    table = document.getElementById('otherFlights');
    var string = e.attr('id').toString();
    var trid = string.substr(1, 1);
    var id = table.rows[trid].cells[1].innerHTML
    otherClickLogic(id);
    otherPreviousPath = trid;
    otherPreviousRow = trid;
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
    for (var index = 0; index < otherCounter; index++) {
        if (index == otherPreviousPath) {
            otherMarkers[index].setIcon(blackPlane);
            document.getElementById("flightDetails").innerHTML = "";
            mymap.removeLayer(otherPaths[index]);
            var otherTable = document.getElementById('otherFlights');
            otherTable.rows[otherPreviousRow].style.backgroundColor = "white";
        }
    }
    for (var i = 0; i < counter; i++) {
        if (markers[i].options.customId === id) {
            $("#flightDetails").html("");
            markers[i].setIcon(bluePlane);
            var end = flights[i].endTime;
            var flightUrl = "../api/FlightPlan/" + id;
            $.getJSON(flightUrl).done(function (data) {
                var lastSegm = data.segments.length - 1;
                $("#flightDetails").html("<tr><td>" + id + "</td>" +
                    "<td>" + data.company_name + "</td>" + "<td>" + data.passengers + "</td>" +
                    "<td>" + data.initial_Location.date_time + "</td>" +
                    "<td>{" + data.initial_Location.latitude + "," + data.initial_Location.longitude + "}</td>" +
                    "<td>" + end + "</td>" +
                    "<td>{" + data.segments[lastSegm].latitude + "," + data.segments[lastSegm].longitude + "}</td>" +
                    "</tr>");
            });
            paths[i].addTo(mymap);
            for (var ind = 0; ind < numRows; ind++) {
                var row = table.rows[ind];
                if (row.cells[1].innerHTML === id) {
                    row.style.backgroundColor = "LightSkyBlue";
                }
            }
        }
    }
}
//click logic when clicking marker or row; showing path, change plane color, show flight details
function otherClickLogic(id) {
    table = document.getElementById('otherFlights');
    var numRows = table.rows.length;
    for (var index = 0; index < otherCounter; index++) {
        if (index == otherPreviousPath) {
            otherMarkers[index].setIcon(blackPlane);
            document.getElementById("flightDetails").innerHTML = "";
            mymap.removeLayer(otherPaths[index]);
            table.rows[otherPreviousRow].style.backgroundColor = "white";
        }
    }
    for (var index = 0; index < counter; index++) {
        if (index == previousPath) {
            markers[index].setIcon(blackPlane);
            document.getElementById("flightDetails").innerHTML = "";
            mymap.removeLayer(paths[index]);
            var otherTable = document.getElementById('myFlights');
            otherTable.rows[previousRow].style.backgroundColor = "white";
        }
    }
    for (var i = 0; i < otherCounter; i++) {
        if (otherMarkers[i].options.customId === id) {
            $("#flightDetails").html("");
            otherMarkers[i].setIcon(bluePlane);
            var end = otherFlights[i].endTime;
            var flightUrl = "../api/FlightPlan/" + id;
            $.getJSON(flightUrl).done(function (data) {
                var lastSegm = data.segments.length - 1;
                $("#flightDetails").html("<tr><td>" + id + "</td>" +
                    "<td>" + data.company_name + "</td>" + "<td>" + data.passengers + "</td>" +
                    "<td>" + data.initial_Location.date_time + "</td>" +
                    "<td>{" + data.initial_Location.latitude + "," + data.initial_Location.longitude + "}</td>" +
                    "<td>" + end + "</td>" +
                    "<td>{" + data.segments[lastSegm].latitude + "," + data.segments[lastSegm].longitude + "}</td>" +
                    "</tr>");
            });
            otherPaths[i].addTo(mymap);
            for (var ind = 0; ind < numRows; ind++) {
                var row = table.rows[ind];
                console.log("ind:" + ind + "row cell:" + row.cells[1].innerHTML);
                if (table.rows[ind].cells[1].innerHTML === id) {
                    row.style.backgroundColor = "LightSkyBlue";
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
    for (var index = 0; index < otherCounter; index++) {
        otherMarkers[index].setIcon(blackPlane);
        mymap.removeLayer(otherPaths[index])
    }
    table = document.getElementById('myFlights');
    var numRows = table.rows.length;
    for (var ind = 0; ind < numRows; ind++) {
        var row = table.rows[ind];
        row.style.backgroundColor = "white";
    }
    table = document.getElementById('otherFlights');
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
    var id = table.rows[trid].cells[1].innerHTML
    $(document).off("click", e);
    $.ajax({
        url: "../api/Flights/" + id,
        type: "DELETE",
        success: function (data) {
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
function updatingMyTable() {
    setTimeout(updatingMyTable, 2000);
    var date = new Date();
    var now_utc = Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
        date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
    var iso = new Date(now_utc).toISOString().replace(".000", "");
    var flightUrl1 = "../api/Flights?relative_to=" + iso;
    $.getJSON(flightUrl1).done(function (data) {
        data.forEach(function (flight) {
            const found = flights.some(el => el.id === flight.flight_id);
            if (!found) {
                flights.push({ id: flight.flight_id, company: flight.company_name, endTime: flight.endTime });
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
    })
        .fail(function () {
            Toastify({
                text: "Error",
                duration: 1500,
                position: 'center',
                backgroundColor: "linear-gradient(to right, #00b09b, #96c93d)"
            }).showToast();
        });
}
function updatingOtherTable() {
    setTimeout(updatingOtherTable, 2000);
    var date = new Date();
    var now_utc = Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
        date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
    var iso = new Date(now_utc).toISOString().replace(".000", "");
    var flightUrl2 = "../api/Flights?relative_to=" + iso + "&sync_all";
    $.getJSON(flightUrl2).done(function (data) {
        data.forEach(function (flight) {
            if (flight.is_external) {
                const found = otherFlights.some(el => el.id === flight.flight_id);
                if (!found) {
                    otherFlights.push({ id: flight.flight_id, company: flight.company_name, endTime: flight.endTime });
                    otherMarkers.push(L.marker([flight.latitude, flight.longitude],
                        { customId: flight.flight_id, icon: blackPlane }));
                    otherMarkers[otherCounter].addTo(mymap);
                    otherMarkers[otherCounter].on('click', onMarkerClick);
                    var flightUrl = "../api/FlightPlan/" + flight.flight_id;
                    var latlngs = new Array();
                    $.getJSON(flightUrl).done(function (data) {
                        latlngs.push([data.initial_Location.latitude, data.initial_Location.longitude]);
                        for (var j = 0; j < data.segments.length; j++) {
                            latlngs.push([data.segments[j].latitude, data.segments[j].longitude]);
                        }
                        otherPaths.push(L.polyline(latlngs, { color: 'blue' }));
                        otherCounter++;
                    });
                }
                else {
                    for (var i = 0; i < otherCounter; i++) {
                        if (otherFlights[i].id === flight.flight_id) {
                            otherMarkers[i].setLatLng([flight.latitude, flight.longitude]);
                        }
                    }
                }

                var content = $("#otherFlights").html();
                if (!content.includes(flight.flight_id)) {
                    $("#otherFlights").append("<tr id=o" + otherCounter + "><td>#</td>"
                        + "<td>" + flight.flight_id + "</td>" +
                        "<td>" + flight.company_name + "</td></tr>");
                    addOtherRowHandlers(otherCounter);
                }
            }
        });
    })
        .fail(function () {
            Toastify({
                text: "Error",
                duration: 1500,
                position: 'center',
                backgroundColor: "linear-gradient(to right, #00b09b, #96c93d)"
            }).showToast();
        });
}
updatingMyTable();
updatingOtherTable();