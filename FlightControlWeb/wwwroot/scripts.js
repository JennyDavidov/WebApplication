//array my flights
let flights = new Array();
//array other flights
let otherFlights = new Array();
//configuring map and marker icon
let mymap = window.L.map('mapid').setView([51.505, -0.09], 1);
window.L.tileLayer('http://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a>',
}).addTo(mymap);
let MarkerIcon = window.L.Icon.extend({
    options: {
        customId: "",
        iconSize: [18, 35],
    }
});
let bluePlane = new MarkerIcon({ iconUrl: 'images/airplane1.png' }),
    blackPlane = new MarkerIcon({ iconUrl: 'images/plane.png' });
let markers = new Array();
let counter = 0;
let paths = new Array();
let otherMarkers = new Array();
let otherCounter = 0;
let otherPaths = new Array();
let previousPath = -1;
let previousRow = -1;
let table;
let otherPreviousPath = -1;
let otherPreviousRow = -1;
//ajax post request for adding new flight plan
const input = document.querySelector('input[type="file"]');
input.addEventListener('change', function () {
    const reader = new FileReader();
    reader.onload = function () {
        $.ajax({
            url: "../api/FlightPlan",
            type: "POST",
            contentType: "application/json",
            dataType: "json",
            data: reader.result,
            success: function () {
            },
            error: function () {
                window.Toastify({
                    text: "Error uploading json",
                    duration: 1500,
                    position: 'left',
                    backgroundColor: "linear-gradient(to right, #FF6347, #B22222)"
                }).showToast();
            },
        });
    }
    if (input.files[0].type === "application/json") {
        reader.readAsText(input.files[0]);
    }
}, false)
//clicking on marker
function onMarkerClick(e) {
    let id;
    for (let marker of markers) {
        let local = marker.getLatLng().toString().localeCompare(e.latlng.toString());
        //if the click was in plane in index i, execute click logic on him
        if (local === 0) {
            id = marker.options.customId;
            clickLogic(id);
            previousPath = markers.indexOf(marker);
            previousRow = markers.indexOf(marker);
            return;
        }
    }
    for (let omarker of otherMarkers) {
        let local = omarker.getLatLng().toString().localeCompare(e.latlng.toString());
        //if the click was in plane in index i, execute click logic on him
        if (local === 0) {
            id = omarker.options.customId;
            otherClickLogic(id);
            otherPreviousPath = otherMarkers.indexOf(omarker);
            otherPreviousRow = otherMarkers.indexOf(omarker);
            return;
        }
    }
}
//adding event handlers of clicking and deleting row for new row in table
function addRowHandlers(counter) {
    let tr = "#" + counter;
    let del = "#del" + counter;
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
    let tr = "#o" + counter;
    $(document).on("click", tr, function () {
        otherRowClick($(this));
    });
}
//cancel event handlers of row in table
function cancelRowHandlers(idRow) {
    let tr = "#" + idRow;
    let del = "#del" + idRow;
    $(document).off("click", tr);
    $(document).off("click", del);
}
//clicking row in table myFlights
function onRowClick(e) {
    table = document.getElementById('myFlights');
    let trid = e.attr('id');
    let id = table.rows[trid].cells[1].innerHTML;
    clickLogic(id);
    previousPath = trid;
    previousRow = trid;
}
//clicking row in table otherFlights
function otherRowClick(e) {
    table = document.getElementById('otherFlights');
    let string = e.attr('id').toString();
    let trid = string.substr(1, 1);
    let id = table.rows[trid].cells[1].innerHTML;
    otherClickLogic(id);
    otherPreviousPath = trid;
    otherPreviousRow = trid;
}
//click logic when clicking marker or row; showing path, change plane color, show flight details
function clickLogic(id) {
    table = document.getElementById('myFlights');
    let numRows = table.rows.length;
    for (let index = 0; index < counter; index++) {
        if (index == previousPath) {
            markers[index].setIcon(blackPlane);
            document.getElementById("flightDetails").innerHTML = "";
            mymap.removeLayer(paths[index]);
            table.rows[previousRow].style.backgroundColor = "white";
        }
    }
    for (let index = 0; index < otherCounter; index++) {
        if (index == otherPreviousPath) {
            otherMarkers[index].setIcon(blackPlane);
            document.getElementById("flightDetails").innerHTML = "";
            mymap.removeLayer(otherPaths[index]);
            let otherTable = document.getElementById('otherFlights');
            otherTable.rows[otherPreviousRow].style.backgroundColor = "white";
        }
    }
    for (let i = 0; i < counter; i++) {
        if (markers[i].options.customId === id) {
            $("#flightDetails").html("");
            markers[i].setIcon(bluePlane);
            let end = flights[i].endTime;
            let flightUrl = "../api/FlightPlan/" + id;
            $.getJSON(flightUrl).done(function (data) {
                let lastSegm = data.segments.length - 1;
                $("#flightDetails").html("<tr><td>" + id + "</td>" +
                    "<td>" + data.company_name + "</td>" + "<td>" + data.passengers + "</td>" +
                    "<td>" + data.initial_location.date_time + "</td>" +
                    "<td>{" + data.initial_location.latitude + "," + data.initial_location.longitude + "}</td>" +
                    "<td>" + end + "</td>" +
                    "<td>{" + data.segments[lastSegm].latitude + "," + data.segments[lastSegm].longitude + "}</td>" +
                    "</tr>");
            })
                .fail(function () {
                    window.Toastify({
                        text: "Error",
                        duration: 1500,
                        position: 'left',
                        backgroundColor: "linear-gradient(to right, #FF6347, #B22222)"
                    }).showToast();
                });
            paths[i].addTo(mymap);
            for (let ind = 0; ind < numRows; ind++) {
                let row = table.rows[ind];
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
    let numRows = table.rows.length;
    for (let index = 0; index < otherCounter; index++) {
        if (index == otherPreviousPath) {
            otherMarkers[index].setIcon(blackPlane);
            document.getElementById("flightDetails").innerHTML = "";
            mymap.removeLayer(otherPaths[index]);
            table.rows[otherPreviousRow].style.backgroundColor = "white";
        }
    }
    for (let index = 0; index < counter; index++) {
        if (index == previousPath) {
            markers[index].setIcon(blackPlane);
            document.getElementById("flightDetails").innerHTML = "";
            mymap.removeLayer(paths[index]);
            let otherTable = document.getElementById('myFlights');
            otherTable.rows[previousRow].style.backgroundColor = "white";
        }
    }
    for (let i = 0; i < otherCounter; i++) {
        if (otherMarkers[i].options.customId === id) {
            $("#flightDetails").html("");
            otherMarkers[i].setIcon(bluePlane);
            let end = otherFlights[i].endTime;
            let flightUrl = "../api/FlightPlan/" + id;
            $.getJSON(flightUrl).done(function (data) {
                let lastSegm = data.segments.length - 1;
                $("#flightDetails").html("<tr><td>" + id + "</td>" +
                    "<td>" + data.company_name + "</td>" + "<td>" + data.passengers + "</td>" +
                    "<td>" + data.initial_location.date_time + "</td>" +
                    "<td>{" + data.initial_location.latitude + "," + data.initial_location.longitude + "}</td>" +
                    "<td>" + end + "</td>" +
                    "<td>{" + data.segments[lastSegm].latitude + "," + data.segments[lastSegm].longitude + "}</td>" +
                    "</tr>");
            })
                .fail(function () {
                    window.Toastify({
                        text: "Error",
                        duration: 1500,
                        position: 'left',
                        backgroundColor: "linear-gradient(to right, #FF6347, #B22222)"
                    }).showToast();
                });
            otherPaths[i].addTo(mymap);
            for (let ind = 0; ind < numRows; ind++) {
                let row = table.rows[ind];
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
    for (let marker of markers) {
        marker.setIcon(blackPlane);
        mymap.removeLayer(paths[markers.indexOf(marker)])
    }
    for (let omarker of otherMarkers) {
        omarker.setIcon(blackPlane);
        mymap.removeLayer(otherPaths[otherMarkers.indexOf(omarker)])
    }
    table = document.getElementById('myFlights');
    let numRows = table.rows.length;
    for (let ind = 0; ind < numRows; ind++) {
        let row = table.rows[ind];
        row.style.backgroundColor = "white";
    }
    table = document.getElementById('otherFlights');
    numRows = table.rows.length;
    for (let ind = 0; ind < numRows; ind++) {
        let row = table.rows[ind];
        row.style.backgroundColor = "white";
    }
}
mymap.on('click', onMapClick);
//deleting flight from my server flights
function deleteRow(e) {
    table = document.getElementById('myFlights');
    let string = e.attr('id').toString();
    let trid;
    if (string.length == 4) {
        trid = string.substr(3, 1);
    } else {
        trid = string;
    }
    let id = table.rows[trid].cells[1].innerHTML;
    $(document).off("click", e);
    $.ajax({
        url: "../api/Flights/" + id,
        type: "DELETE",
        success: function () {
            cancelRowHandlers(table.rows.length - 1);
            table.rows[trid].remove();
            mymap.removeLayer(markers[trid]);
            mymap.removeLayer(paths[trid]);
            let content = $("#flightDetails").html();
            if (content.includes(id)) {
                $("#flightDetails").html("");
            }
            flights.splice(trid, 1);
            markers.splice(trid, 1);
            paths.splice(trid, 1);
            $('#myFlights > tr').each(function () {
                let thisId = trid;
                let prevId = $(this).attr('id');
                if (prevId > thisId) {
                    $(this).attr('id', thisId);
                    $('#del' + prevId).attr('id', "del" + thisId);
                    thisId++;
                }
            });
            counter--;
        },
        error: function () {
            window.Toastify({
                text: "Error deleting flight",
                duration: 1500,
                position: 'left',
                backgroundColor: "linear-gradient(to right, #FF6347, #B22222)"
            }).showToast();
        },
    });
}
function deleteOtherRow(trid) {
    table = document.getElementById('otherFlights');
    $(document).off("click", $(table.rows[(table.rows.length - 1)]));
    let id = table.rows[trid].cells[1].innerHTML;
    table.rows[trid].remove();
    mymap.removeLayer(otherMarkers[trid]);
    mymap.removeLayer(otherPaths[trid]);
    let content = $("#flightDetails").html();
    if (content.includes(id)) {
        $("#flightDetails").html("");
    }
    otherFlights.splice(trid, 1);
    otherMarkers.splice(trid, 1);
    otherPaths.splice(trid, 1);
    $('#otherFlights > tr').each(function () {
        let thisId = trid;
        let string = $(this).attr('id');
        let prevId = string.substr(1,1);
        if (prevId > thisId) {
            $(this).attr('id', "o" + thisId);
            thisId++;
        }
    });
    otherCounter--;
}
//updating flights according the moment
function updatingMyTable() {
    setTimeout(updatingMyTable, 2000);
    let date = new Date();
    let nowUtc = Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
        date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
    let iso = new Date(nowUtc).toISOString().replace(".000", "");
    let flightUrl1 = "../api/Flights?relative_to=" + iso;
    $.getJSON(flightUrl1).done(function (data) {
        data.forEach(function (flight) {
            const found = flights.some(el => el.id === flight.flight_id);
            if (!found) {
                flights.push({ id: flight.flight_id, company: flight.company_name, endTime: flight.endTime });
                markers.push(window.L.marker([flight.latitude, flight.longitude],
                    { customId: flight.flight_id, icon: blackPlane }));
                markers[counter].addTo(mymap);
                markers[counter].on('click', onMarkerClick);
                let flightUrl = "../api/FlightPlan/" + flight.flight_id;
                let latlngs = new Array();
                $.getJSON(flightUrl).done(function (data) {
                    latlngs.push([data.initial_location.latitude, data.initial_location.longitude]);
                    for (let j = 0; j < data.segments.length; j++) {
                        latlngs.push([data.segments[j].latitude, data.segments[j].longitude]);
                    }
                    paths.push(window.L.polyline(latlngs, { color: 'blue' }));
                    counter++;
                });
            }
            else {
                //flight is over
                let timeLessSec = new Date(nowUtc);
                timeLessSec.setSeconds(new Date(nowUtc).getSeconds() + 1);
                if ((new Date(nowUtc).toISOString() === new Date(flight.endTime).toISOString()) ||
                    (new Date(timeLessSec).toISOString() === new Date(flight.endTime).toISOString())) {
                    for (let i = 0; i < counter; i++) {
                        if (flights[i].id === flight.flight_id) {
                            table = document.getElementById('myFlights');
                            deleteRow($(table.rows[i]));
                        }
                    }
                }
                for (let i = 0; i < counter; i++) {
                    if (flights[i].id === flight.flight_id) {
                        markers[i].setLatLng([flight.latitude, flight.longitude]);
                    }
                }
            }
            let content = $("#myFlights").html();
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
            window.Toastify({
                text: "Error",
                duration: 1500,
                position: 'left',
                backgroundColor: "linear-gradient(to right, #FF6347, #B22222)"
            }).showToast();
        });
}
function updatingOtherTable() {
    setTimeout(updatingOtherTable, 2000);
    let deleted = false;
    let date = new Date();
    let nowUtc = Date.UTC(date.getUTCFullYear(), date.getUTCMonth(), date.getUTCDate(),
        date.getUTCHours(), date.getUTCMinutes(), date.getUTCSeconds());
    let iso = new Date(nowUtc).toISOString().replace(".000", "");
    let flightUrl2 = "../api/Flights?relative_to=" + iso + "&sync_all";
    $.getJSON(flightUrl2).done(function (data) {
        data.forEach(function (flight) {
            if (flight.is_external) {
                const found = otherFlights.some(el => el.id === flight.flight_id);
                if (!found) {
                    otherFlights.push({ id: flight.flight_id, company: flight.company_name, endTime: flight.endTime });
                    otherMarkers.push(window.L.marker([flight.latitude, flight.longitude],
                        { customId: flight.flight_id, icon: blackPlane }));
                    otherMarkers[otherCounter].addTo(mymap);
                    otherMarkers[otherCounter].on('click', onMarkerClick);
                    let flightUrl = "../api/FlightPlan/" + flight.flight_id;
                    let latlngs = new Array();
                    $.getJSON(flightUrl).done(function (data) {
                        latlngs.push([data.initial_location.latitude, data.initial_location.longitude]);
                        for (let j = 0; j < data.segments.length; j++) {
                            latlngs.push([data.segments[j].latitude, data.segments[j].longitude]);
                        }
                        otherPaths.push(window.L.polyline(latlngs, { color: 'blue' }));
                    });
                    otherCounter++;
                }
                else {
                    for (let i = 0; i < otherCounter; i++) {
                        if (otherFlights[i].id === flight.flight_id) {
                            otherMarkers[i].setLatLng([flight.latitude, flight.longitude]);
                        }
                    }
                    //flight is over
                    let timeLessSec = new Date(nowUtc);
                    timeLessSec.setSeconds(new Date(nowUtc).getSeconds() + 1);
                    if ((new Date(nowUtc).toISOString() === new Date(flight.endTime).toISOString()) ||
                        (new Date(timeLessSec).toISOString() === new Date(flight.endTime).toISOString())) {
                        for (let i = 0; i < otherCounter; i++) {
                            if (otherFlights[i].id === flight.flight_id) {
                                deleteOtherRow(i);
                                deleted = true;
                                break;
                            }
                        }
                    }
                }
                if (!deleted) {
                    let content = $("#otherFlights").html();
                    if (!content.includes(flight.flight_id)) {
                        $("#otherFlights").append("<tr id=o" + (otherCounter - 1) + "><td>#</td>"
                            + "<td>" + flight.flight_id + "</td>" +
                            "<td>" + flight.company_name + "</td></tr>");
                        addOtherRowHandlers(otherCounter - 1);
                    }
                }
            }
        });
    });
}
updatingMyTable();
updatingOtherTable();
window.onerror = function myErrorHandler(errorMsg) {
    window.Toastify({
        text: "Error" + errorMsg,
        duration: 3000,
        position: 'left',
        backgroundColor: "linear-gradient(to right, #FF6347, #B22222)"
    }).showToast();
    return false;
}