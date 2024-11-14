
    // Definer forskjellige kartlag
    var norgeKart = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', {
        attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
    });

    var turKart = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/toporaster/default/webmercator/{z}/{y}/{x}.png', {
        attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
    });

    var sjoKart = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png', {
        attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
    });

    var background = L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
        attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">Kartverket</a>',
    zIndex: 0
    });

    // Initial kartoppsett
    var map = L.map('map', {
        center: [62.15, 8.5],
    zoom: 5,
    layers: [norgeKart, background]
    });

    function locateUser() {
        map.locate({
            setView: true,
            maxZoom: 18
        }).on('locationerror', function (e) {
            // Viser en advarsel med en tilpasset feilmelding
            alert("Kunne ikke hente geolokasjon. Sjekk at enheten din har geolokasjon aktivert og gi tillatelse for plassering.");
        });
    }

    // Bytter ut map_selection_container med map_container for å vise kartet etter et kart blir valgt
    const map_selector1 = document.querySelector('.map_container_hidden');
    const map_selector2 = document.querySelector('.map_selection_container');

    function MapSelect1() {
        map.removeLayer(turKart);
    map.removeLayer(sjoKart);
    map.addLayer(norgeKart);

    map_selector1.classList.remove('map_container_hidden');
    map_selector1.classList.add('map_container');
    map_selector2.classList.add('map_selection_container_hidden');

    setTimeout(function () {
        map.invalidateSize();
        }, 50);

    // Setter verdi for karttype i skjemaet
    document.getElementById('mapType').value = "Norge kart";
    }

    function MapSelect2() {
        map.removeLayer(norgeKart);
    map.removeLayer(sjoKart);
    map.addLayer(turKart);

    map_selector1.classList.remove('map_container_hidden');
    map_selector1.classList.add('map_container');
    map_selector2.classList.add('map_selection_container_hidden');

    setTimeout(function () {
        map.invalidateSize();
        }, 50);

    // Setter verdi for karttype i skjemaet
    document.getElementById('mapType').value = "Turkart";
    }

    function MapSelect3() {
        map.removeLayer(norgeKart);
    map.removeLayer(turKart);
    map.addLayer(sjoKart);

    map_selector1.classList.remove('map_container_hidden');
    map_selector1.classList.add('map_container');
    map_selector2.classList.add('map_selection_container_hidden');

    setTimeout(function () {
        map.invalidateSize();
        }, 50);

    // Setter verdi for karttype i skjemaet
    document.getElementById('mapType').value = "Sjøkart";
    }

    // Fjerner knapp etter den blir trykket på og transformerer map_container til å inneholde form_area
    const button = document.querySelector('.btn_map_transform');
    const div1 = document.querySelector('.map_container_hidden');
    const div2 = document.querySelector('.form_area');
    const div3 = document.querySelector('.app_main');
    const div4 = document.querySelector('.btn_map_transform');

    button.addEventListener('click', function () {
        // Veksler 'transformed'-klassen på div
        div1.classList.toggle('map_container_transform');
    div2.classList.toggle('form_area_transform');
    div3.classList.toggle('app_main_transform');
    div4.classList.remove('btn_map');
    div4.classList.add('btn_map_transform');
    });


    function toggleButtonClassOnRapport() {
    div4.classList.remove('btn_map');
        div4.classList.add('btn_map_transform');

    // Skjul verktøylinje
    const drawToolbar = document.querySelector('.leaflet-draw-toolbar');
    if (drawToolbar) {
        drawToolbar.style.display = 'none';
    }
}
function toggleButtonClass() {
    div4.classList.remove('btn_map_transform');
    div4.classList.add('btn_map');
}

function toggleButtonClassRemove() {
    div4.classList.remove('btn_map');
    div4.classList.add('btn_map_transform');
}

    var locateControl = L.control({position: 'topleft' });

    locateControl.onAdd = function (map) {
        var div = L.DomUtil.create('div', 'leaflet-bar');
    div.innerHTML = '<button id="location" title="Hvor er jeg?" onclick="locateUser()"><i class="fa-solid fa-crosshairs"></i></button>'; // Legg til ikon eller tekst for knappen
    L.DomEvent.disableClickPropagation(div);

    return div;
    };

    // Legg til kontrollen til kartet
    locateControl.addTo(map);

    // Funksjon for å veksle mellom id-ene 'lokasjonVerktoyHidden' og 'lokasjonVerktoyVisable'
    function VisLokasjonverktoy() {
        var lokasjonVerktoy = document.getElementById("lokasjonVerktoyHidden") || document.getElementById("lokasjonVerktoyVisable");

    if (lokasjonVerktoy) {
            // Veksler id-attributtet
            if (lokasjonVerktoy.id === "lokasjonVerktoyHidden") {
        lokasjonVerktoy.id = "lokasjonVerktoyVisable";
    lokasjonVerktoy.style.backgroundColor = "white"; // Endrer bakgrunn til hvit når synlig
            } else {
        lokasjonVerktoy.id = "lokasjonVerktoyHidden";
    lokasjonVerktoy.style.backgroundColor = ""; // Tilbakestiller bakgrunnsfargen når skjult
            }
        }
    }

    // Knytter toggle-funksjonen til tracker-knappen
    var trackerButton = document.getElementById("tracker");
    if (trackerButton) {
        trackerButton.addEventListener("click", VisLokasjonverktoy);
    }

    var PosisjonTrackerControl = L.control({position: 'bottomleft' });

    PosisjonTrackerControl.onAdd = function (map) {
        var div = L.DomUtil.create('div', 'leaflet - custom - bottom - center');
    div.innerHTML = `
    <div id="lokasjonVerktoyHidden">
        <div id="undoRedo">
            <button id="undo"><i class="fa-solid fa-rotate-left"></i></button>
            <button id="redo"><i class="fa-solid fa-arrow-rotate-right"></i></button>
        </div>
        <div id="lokasjonOppe">
            <button id="addPoint"><i class="fa-solid fa-plus"></i> Legg til punkt</button>
        </div>
        <div id="lokasjonNede">
            <button id="avbryt"><i class="fa-solid fa-trash"></i> Avbryt</button>
            <button id="ferdig"><i class="fa-solid fa-check"></i> Ferdig</button>
        </div>
    </div>
    `; // Legg til knapper med riktig syntaks
    L.DomEvent.disableClickPropagation(div);

    return div;
    };

    // Legg til kontrollen til kartet
    PosisjonTrackerControl.addTo(map);

    var blueCircleIcon = L.divIcon({
        className: 'custom-marker', // Du kan legge til en tilpasset klasse for styling
    iconSize: [20, 20], // Størrelse på ikonet
    html: '<div class="LokasjonpunktIkon"></div>', // Bruk en div med en spesifikk klasse for markøren
});

    var userPath = L.polyline([], {color: 'blue' }).addTo(map);
    var coordinatesList = []; // Liste som holder alle koordinater
    var markers = []; // Liste for å holde alle markører

    // Funksjon for å legge til et enkelt punkt på kartet
    function addSinglePoint() {
    if (navigator.geolocation) {
        navigator.geolocation.getCurrentPosition(function (position) {
            var lat = position.coords.latitude;
            var lng = position.coords.longitude;

            // Sjekk om koordinatene er de samme som siste punkt i listen
            if (coordinatesList.length > 0) {
                var lastCoord = coordinatesList[coordinatesList.length - 1];
                if (lastCoord.lat === lat && lastCoord.lng === lng) {
                    // Hvis koordinatene er de samme, vis en popup på siste markør
                    if (markers.length > 0) {
                        markers[markers.length - 1]
                            .bindPopup("Duplikat koordinat: Prøv å gå lengre unna siste punkt.")
                            .openPopup();
                    }
                    console.log("Hoppet over å legge til duplikatkoordinater:", { lat, lng });
                }
            }

            // Legg til nytt punkt i polyline og coordinatesList
            userPath.addLatLng([lat, lng]);
            coordinatesList.push({ lat: lat, lng: lng });

            // Opprett ny markør for det nye punktet
            var marker = L.marker([lat, lng], { icon: blueCircleIcon }).addTo(map)
                .bindPopup("Din posisjon: Nytt punkt lagt til.");
            markers.push(marker);

            // Sentrer kartet på brukerens posisjon
            map.setView([lat, lng], 18);

            // Fjern eksisterende koordinat-inputs og oppdater dem
            document.querySelectorAll(".coordinate-input").forEach((el) => el.remove());
            coordinatesList.forEach((coord, index) => {
                let latInput = document.createElement("input");
                latInput.type = "hidden";
                latInput.name = `koordinater[${index}].Nord`;
                latInput.value = coord.lat;
                latInput.classList.add("coordinate-input");

                let lonInput = document.createElement("input");
                lonInput.type = "hidden";
                lonInput.name = `koordinater[${index}].Ost`;
                lonInput.value = coord.lng;
                lonInput.classList.add("coordinate-input");

                document.getElementById("form_id").appendChild(latInput);
            });

            console.log("Alle koordinater lagt til så langt:", coordinatesList);
        }, function (error) {
            console.log("Feil ved henting av posisjon:", error);
            alert("Kunne ikke hente geolokasjon. Sjekk at enheten din har geolokasjon aktivert og gi tillatelse for plassering.");
        }, { enableHighAccuracy: true });
    } else {
        alert("Geolokasjon støttes ikke av denne nettleseren.");
    }
}

    // Legger til event listener for å kjøre funksjonen når knappen trykkes
    document.getElementById("addPoint").addEventListener("click", addSinglePoint);

    function AvbrytSporing() {
        // Tøm koordinatlisten
        coordinatesList = [];

        // Fjern alle markører fra kartet og tøm markørlisten
        markers.forEach(marker => map.removeLayer(marker));
    markers = [];

    // Tøm polyline-stien
    userPath.setLatLngs([]);

    // Skjul lokasjonsverktøy ved å endre id tilbake til 'lokasjonVerktoyHidden'
    var lokasjonVerktoy = document.getElementById("lokasjonVerktoyVisable");
    if (lokasjonVerktoy) {
        lokasjonVerktoy.id = "lokasjonVerktoyHidden";
    lokasjonVerktoy.style.backgroundColor = ""; // Tilbakestill bakgrunnsfargen
        }

    console.log("Sporing avbrutt: Koordinater og markører fjernet.");
    }

    // Knytter avbryt sporing-funksjonen til 'Avbryt'-knappen
    document.getElementById("avbryt").addEventListener("click", AvbrytSporing);

    var drawnItems = new L.FeatureGroup();
    map.addLayer(drawnItems);

    // Oppretter tegnekontrollen på venstre side
    // Tegnekontrollen gjør det mulig å tegne markører, polygoner og polylinjer
    var drawControl = new L.Control.Draw({
        position: 'topleft',
    draw: {
        polygon: false,
    polyline: true,
    marker: false,
    circle: false,
    circlemarker: false,
    rectangle: false
        },
    edit: {
        featureGroup: drawnItems
        }
    });
    map.addControl(drawControl);

    function addTrackerToDrawToolbar() {
        // Finn Leaflet tegneverktøylinjen
        const drawToolbar = document.querySelector('.leaflet-draw-toolbar');

    if (drawToolbar) {
            const div = document.createElement('div');
    div.className = 'leaflet-bar leaflet-draw-draw-tracker'; // Legg til Leaflet bar styling og tilpasset klasse
    div.innerHTML = '<button id="tracker" title="direkte sporing"><i class="fa-solid fa-person-walking"></i></button>';
    div.style.cursor = "pointer";
    div.querySelector('#tracker').onclick = function () {
        VisLokasjonverktoy();
    locateUser();
            };
    L.DomEvent.disableClickPropagation(div);

    // Legg til div i verktøylinjen
    drawToolbar.appendChild(div);
        }
    }

    // Kall funksjonen for å legge til tracker-kontrollknappen til verktøylinjen
    addTrackerToDrawToolbar();

    // Funksjon for å fjerne tracker-kontroll fra verktøylinjen
    function removeTrackerControl() {
        const trackerButton = document.getElementById("tracker");
    if (trackerButton) {
        trackerButton.remove();
        }
}

// Function to disable tracker control
function disableTrackerControl() {
    const trackerButton = document.getElementById("tracker");
    if (trackerButton) {
        trackerButton.disabled = true;
        trackerButton.style.backgroundColor = "#e5e5e5";
        trackerButton.style.pointerEvents = "none";
    }
}

// Function to enable tracker control
function enableTrackerControl() {
    const trackerButton = document.getElementById("tracker");
    if (trackerButton) {
        trackerButton.disabled = false;
        trackerButton.style.backgroundColor = "white";
        trackerButton.style.pointerEvents = "auto";
    }
}

    // Hendelse for å utløse AvbrytSporing når polyline-verktøyet aktiveres
    map.on('draw:drawstart', function (e) {
        if (e.layerType === 'polyline') {
            AvbrytSporing();
            disableTrackerControl();
        }
    });

map.on('draw:drawstop', function () {
    enableTrackerControl(); // Enable tracker control
});

    // Hendelse som håndterer når et lag tegnes på kartet
    map.on(L.Draw.Event.CREATED, function (e) {
        var type = e.layerType,
    layer = e.layer;

    drawnItems.addLayer(layer);

    map.removeControl(drawControl);

    // Legg til verktøylinjen på nytt uten polyline-verktøyet, men med redigeringskontroller aktivert
    map.removeControl(drawControl);
    drawControl = new L.Control.Draw({
        position: 'topleft',
    draw: {
        polygon: false,
    polyline: false, // Deaktiver polyline-tegning her
    marker: false,
    circle: false,
    circlemarker: false,
    rectangle: false
            },
    edit: {
        featureGroup: drawnItems // Aktiver redigering for tegnet lag
            }
        });
    map.addControl(drawControl);

    // Hent GeoJSON-representasjonen av tegnet lag
    var geoJsonData = layer.toGeoJSON();

    // Ekstraher koordinater som separate lister for breddegrad og lengdegrad
    var latitudes = [];
    var longitudes = [];
    geoJsonData.geometry.coordinates.forEach(function (coord) {
        latitudes.push(coord[1]); // Breddegrad er det andre elementet i hvert koordinatpar
    longitudes.push(coord[0]); // Lengdegrad er det første elementet i hvert koordinatpar
        });

        // Fjern eventuelle eksisterende koordinat-inputs
        document.querySelectorAll(".coordinate-input").forEach((el) => el.remove());

        // Dynamisk opprett skjulte felt for hvert koordinatpar
        latitudes.forEach((latitude, index) => {
        let latInput = document.createElement("input");
    latInput.type = "hidden";
    latInput.name = `koordinater[${index}].Nord`; // Matcher parameterenavnet og strukturen
    latInput.value = latitude;
    latInput.classList.add("coordinate-input");

    let lonInput = document.createElement("input");
    lonInput.type = "hidden";
    lonInput.name = `koordinater[${index}].Ost`; // Matcher parameterenavnet og strukturen
    lonInput.value = longitudes[index];
    lonInput.classList.add("coordinate-input");

    document.getElementById("form_id").appendChild(latInput);
    document.getElementById("form_id").appendChild(lonInput);
        });

    });

    map.on('draw:created', function (e) {
        // Når tegningen er ferdig, blir hendelsen utløst

        // Tilgang til laget (f.eks. polygon, polyline) opprettet av brukeren
        var layer = e.layer;

    // Legg til laget på kartet (valgfritt, hvis du trenger det skal vises på kartet)
    map.addLayer(layer);

    toggleButtonClass();
    });

    function updateDrawControl(enablePolyline) {
        map.removeControl(drawControl); // Fjern den gjeldende tegnekontrollen

    // Legg til tegnekontroll på nytt med polyline-verktøy basert på enablePolyline-parameteren
    drawControl = new L.Control.Draw({
        position: 'topleft',
    draw: {
        polygon: false,
    polyline: enablePolyline, // Aktiver eller deaktiver polyline basert på parameteren
    marker: false,
    circle: false,
    circlemarker: false,
    rectangle: false
            },
    edit: {
        featureGroup: drawnItems // Hold redigeringsverktøy aktive
            }
        });
    map.addControl(drawControl);
    }

    map.on('draw:deleted', function () {
        // Aktiver polyline-verktøyet hvis alle linjer slettes
        if (drawnItems.getLayers().length === 0) {
        updateDrawControl(true);
            addTrackerToDrawToolbar(); // Legg til tracker-knappen på nytt
            toggleButtonClassRemove();
    console.log("Polyline-verktøyet aktivert på nytt etter at alle linjer er slettet.");
        }
    });

