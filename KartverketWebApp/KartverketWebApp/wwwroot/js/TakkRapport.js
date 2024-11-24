class TakkRapport {
    constructor(appData) {
        if (!appData) {
            console.error('appData er ikke definert');
            return;
        }

        this.mapData = appData.mapData;
        this.map = null;

        console.log('Initializing TakkRapport with:', { mapData: this.mapData });
        this.initializeMap();
    }

    initializeMap() {
        if (!this.mapData) {
            console.error('mapData er ikke definert');
            return;
        }

        if (this.mapData?.coordinates?.length > 0) {
            const firstCoord = this.mapData.coordinates[0];
            const mapElement = document.getElementById('map_corrections');

            if (!mapElement) {
                console.error('map_corrections element ikke funnet');
                return;
            }

            // Initialize the map
            this.map = L.map('map_corrections', {
                zoomControl: true
            });

            // Define map layers
            const norgeKart = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', {
                attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
            });

            const turKart = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/toporaster/default/webmercator/{z}/{y}/{x}.png', {
                attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
            });

            const sjoKart = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png', {
                attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
            });

            // Add appropriate layer based on map type
            switch (this.mapData.mapType) {
                case "Norge kart":
                    this.map.addLayer(norgeKart);
                    break;
                case "Turkart":
                    this.map.addLayer(turKart);
                    break;
                case "Sjøkart":
                    this.map.addLayer(sjoKart);
                    break;
                default:
                    this.map.addLayer(norgeKart); // Default to Norge kart
                    break;
            }

            // Draw markers and polyline
            const bounds = L.latLngBounds();
            this.mapData.coordinates.forEach((coord, index) => {
                const marker = L.marker([coord.nord, coord.ost])
                    .addTo(this.map)
                    .bindPopup(`Punkt ${index + 1}<br>Nord: ${coord.nord}<br>Øst: ${coord.ost}`);
                bounds.extend([coord.nord, coord.ost]);
            });

            // Draw polyline between points
            const latlngs = this.mapData.coordinates.map(coord => [coord.nord, coord.ost]);
            L.polyline(latlngs, { color: 'blue' }).addTo(this.map);

            this.map.fitBounds(bounds);
        } else {
            console.warn('Ingen koordinater tilgjengelig for visning på kartet');
        }
    }
}

// Global initialization
window.TakkRapport = TakkRapport;