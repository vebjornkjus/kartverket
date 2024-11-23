// Map Layer Configuration
class MapLayerManager {
    constructor() {
        this.layers = {
            norgeKart: L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', {
                attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
            }),
            turKart: L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/toporaster/default/webmercator/{z}/{y}/{x}.png', {
                attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
            }),
            sjoKart: L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png', {
                attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
            }),
            background: L.tileLayer('https://tile.openstreetmap.org/{z}/{x}/{y}.png', {
                attribution: '&copy; <a href="http://www.openstreetmap.org/copyright">Kartverket</a>',
                zIndex: 0
            })
        };
    }

    getLayer(layerName) {
        return this.layers[layerName];
    }
}

// Tracking System
class TrackingSystem {
    constructor(map) {
        this.map = map;
        this.userPath = L.polyline([], { color: 'blue' }).addTo(map);
        this.coordinatesList = [];
        this.markers = [];
        this.blueCircleIcon = this.createBlueCircleIcon();
        this.setupEventListeners();
    }

    createBlueCircleIcon() {
        return L.divIcon({
            className: 'custom-marker',
            iconSize: [20, 20],
            html: '<div class="LokasjonpunktIkon"></div>'
        });
    }

    setupEventListeners() {
        document.getElementById("addPoint").addEventListener("click", () => this.addSinglePoint());
        document.getElementById("avbryt").addEventListener("click", () => this.cancelTracking());
    }

    addSinglePoint() {
        if (!navigator.geolocation) {
            alert("Geolokasjon støttes ikke av denne nettleseren.");
            return;
        }

        navigator.geolocation.getCurrentPosition(
            position => this.handlePosition(position),
            error => this.handleGeolocationError(error),
            { enableHighAccuracy: true }
        );
    }

    handlePosition(position) {
        const { latitude: lat, longitude: lng } = position.coords;

        if (this.isDuplicateCoordinate(lat, lng)) {
            this.showDuplicateWarning();
            return;
        }

        this.addNewPoint(lat, lng);
        this.updateFormCoordinates();
    }

    isDuplicateCoordinate(lat, lng) {
        return this.coordinatesList.length > 0 &&
            this.coordinatesList[this.coordinatesList.length - 1].lat === lat &&
            this.coordinatesList[this.coordinatesList.length - 1].lng === lng;
    }

    addNewPoint(lat, lng) {
        this.userPath.addLatLng([lat, lng]);
        this.coordinatesList.push({ lat, lng });

        const marker = L.marker([lat, lng], { icon: this.blueCircleIcon })
            .addTo(this.map)
            .bindPopup("Din posisjon: Nytt punkt lagt til.");

        this.markers.push(marker);
        this.map.setView([lat, lng], 18);
    }

    updateFormCoordinates() {
        document.querySelectorAll(".coordinate-input").forEach(el => el.remove());

        this.coordinatesList.forEach((coord, index) => {
            const form = document.getElementById("form_id");
            const latInput = this.createCoordinateInput(`koordinater[${index}].Nord`, coord.lat);
            const lonInput = this.createCoordinateInput(`koordinater[${index}].Ost`, coord.lng);

            form.appendChild(latInput);
            form.appendChild(lonInput);
        });
    }

    createCoordinateInput(name, value) {
        const input = document.createElement("input");
        input.type = "hidden";
        input.name = name;
        input.value = value;
        input.classList.add("coordinate-input");
        return input;
    }

    cancelTracking() {
        this.coordinatesList = [];
        this.markers.forEach(marker => this.map.removeLayer(marker));
        this.markers = [];
        this.userPath.setLatLngs([]);
        this.updateLocationToolVisibility();
    }

    updateLocationToolVisibility() {
        const tool = document.getElementById("lokasjonVerktoyVisable");
        if (tool) {
            tool.id = "lokasjonVerktoyHidden";
            tool.style.backgroundColor = "";
        }
    }

    handleGeolocationError(error) {
        console.log("Feil ved henting av posisjon:", error);
        alert("Kunne ikke hente geolokasjon. Sjekk at enheten din har geolokasjon aktivert og gi tillatelse for plassering.");
    }
}

// Drawing Control System
class DrawingControlSystem {
    constructor(map) {
        this.map = map;
        this.drawnItems = new L.FeatureGroup().addTo(map);
        this.drawControl = null;
        this.setupDrawControl();
        this.setupEventListeners();
    }

    setupDrawControl() {
        this.drawControl = new L.Control.Draw({
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
                featureGroup: this.drawnItems
            }
        });
        this.map.addControl(this.drawControl);
    }

    setupEventListeners() {
        this.map.on('draw:created', e => this.handleDrawCreated(e));
        this.map.on('draw:deleted', () => this.handleDrawDeleted());
        this.map.on('draw:drawstart', e => this.handleDrawStart(e));
        this.map.on('draw:drawstop', () => this.handleDrawStop());
    }

    handleDrawCreated(e) {
        const layer = e.layer;
        this.drawnItems.addLayer(layer);
        this.updateDrawControl(false);
        this.processDrawnCoordinates(layer);
    }

    handleDrawDeleted() {
        if (this.drawnItems.getLayers().length === 0) {
            this.updateDrawControl(true);
            this.addTrackerToDrawToolbar();
            toggleButtonClassRemove();
        }
    }

    processDrawnCoordinates(layer) {
        const geoJsonData = layer.toGeoJSON();
        const coordinates = geoJsonData.geometry.coordinates.map(coord => ({
            lat: coord[1],
            lng: coord[0]
        }));

        this.updateFormWithCoordinates(coordinates);
    }

    updateFormWithCoordinates(coordinates) {
        document.querySelectorAll(".coordinate-input").forEach(el => el.remove());

        coordinates.forEach((coord, index) => {
            const form = document.getElementById("form_id");
            const latInput = this.createCoordinateInput(`koordinater[${index}].Nord`, coord.lat);
            const lonInput = this.createCoordinateInput(`koordinater[${index}].Ost`, coord.lng);

            form.appendChild(latInput);
            form.appendChild(lonInput);
        });
    }

    createCoordinateInput(name, value) {
        const input = document.createElement("input");
        input.type = "hidden";
        input.name = name;
        input.value = value;
        input.classList.add("coordinate-input");
        return input;
    }

    updateDrawControl(enablePolyline) {
        this.map.removeControl(this.drawControl);
        this.setupDrawControl();
    }

    addTrackerToDrawToolbar() {
        const drawToolbar = document.querySelector('.leaflet-draw-toolbar');
        if (!drawToolbar) return;

        const div = document.createElement('div');
        div.className = 'leaflet-bar leaflet-draw-draw-tracker';
        div.innerHTML = '<button id="tracker" title="direkte sporing"><i class="fa-solid fa-person-walking"></i></button>';
        div.style.cursor = "pointer";

        div.querySelector('#tracker').onclick = () => {
            VisLokasjonverktoy();
            locateUser();
        };

        L.DomEvent.disableClickPropagation(div);
        drawToolbar.appendChild(div);
    }
}

// Initialize Map and Systems
document.addEventListener('DOMContentLoaded', () => {
    // Initialize map
    const layerManager = new MapLayerManager();
    const map = L.map('map', {
        center: [62.15, 8.5],
        zoom: 5,
        layers: [layerManager.getLayer('norgeKart'), layerManager.getLayer('background')]
    });

    // Initialize tracking and drawing systems
    const trackingSystem = new TrackingSystem(map);
    const drawingSystem = new DrawingControlSystem(map);

    // Add location control
    const locateControl = L.control({ position: 'topleft' });
    locateControl.onAdd = function (map) {
        const div = L.DomUtil.create('div', 'leaflet-bar');
        div.innerHTML = '<button id="location" title="Hvor er jeg?" onclick="locateUser()"><i class="fa-solid fa-crosshairs"></i></button>';
        L.DomEvent.disableClickPropagation(div);
        return div;
    };
    locateControl.addTo(map);
});