document.addEventListener("DOMContentLoaded", function () {
    // Ensure the data is available globally
    const coordinatesJson = parseJsonSafely(window.coordinatesJson);
    const markersJson = parseJsonSafely(window.markersJson);

    if (!coordinatesJson.length && !markersJson.length) {
        console.warn("Coordinates or Markers JSON is missing.");
        return;
    }

    console.log("Parsed Coordinates JSON:", coordinatesJson);
    console.log("Parsed Markers JSON:", markersJson);

    // Initialize the map
    initializeMap(coordinatesJson, markersJson);

    /**
     * Safely parse JSON and handle errors.
     * @param {string} jsonString - JSON string to parse.
     * @returns {Array} Parsed JSON or an empty array.
     */
    function parseJsonSafely(jsonString) {
        try {
            return JSON.parse(jsonString) || [];
        } catch (error) {
            console.error("Error parsing JSON:", error.message);
            return [];
        }
    }

    /**
     * Initialize the map with polygons and markers.
     * @param {Array} coordinatesJson - Polygons data.
     * @param {Array} markersJson - Markers data.
     */
    function initializeMap(coordinatesJson, markersJson) {
        // Define the map tile layer
        const norgeKart = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', {
            attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
        });

        // Initialize the map
        const map = L.map('map', {
            center: [58.1548282, 8.0081166],
            zoom: 10,
            layers: [norgeKart]
        });

        // Draw polygons
        if (coordinatesJson && coordinatesJson.length) {
            try {
                coordinatesJson.forEach((polygonCoords) => {
                    const latlngs = polygonCoords[0].map(coord => [coord[1], coord[0]]);
                    L.polygon(latlngs, {
                        color: '#848BEA',
                        fillColor: '#92B4EF',
                        fillOpacity: 0.2
                    }).addTo(map);
                });
            } catch (error) {
                console.error("Error processing polygon coordinates:", error);
            }
        }

        // Define marker icons
        const defaultIcon = L.divIcon({ className: 'default-marker', html: '<div class="LokasjonpunktIkon"></div>', iconSize: [20, 20] });
        const largeIcon = L.divIcon({ className: 'large-marker', html: '<div class="LokasjonpunktIkon"></div>', iconSize: [25, 25] });

        // Draw markers
        if (markersJson && Array.isArray(markersJson)) {
            try {
                markersJson.forEach(marker => {
                    if (marker.Nord && marker.Ost) {
                        const mapMarker = L.marker([marker.Nord, marker.Ost], { icon: defaultIcon })
                            .bindPopup(`Tittel: ${marker.Tittel}`)
                            .on('click', () => submitFormWithId(marker.RapportId))
                            .on('mouseover', () => mapMarker.setIcon(largeIcon))
                            .on('mouseout', () => mapMarker.setIcon(defaultIcon));

                        mapMarker.addTo(map);
                    } else {
                        console.warn("Invalid marker coordinates:", marker);
                    }
                });
            } catch (error) {
                console.error("Error processing markers JSON:", error);
            }
        }
    }

    /**
     * Submit the form with a specific report ID.
     * @param {number} rapportId - The report ID to submit.
     */
    function submitFormWithId(rapportId) {
        const form = document.getElementById("updateStatusForm");
        const input = document.getElementById("rapportIdInput");
        input.value = rapportId;
        form.submit();
    }
});
