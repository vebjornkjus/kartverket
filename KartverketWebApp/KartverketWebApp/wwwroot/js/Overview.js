// Ensure the data is available globally
if (window.coordinatesJson && window.markersJson) {
    console.log("Coordinates JSON:", window.coordinatesJson);
    console.log("Markers JSON:", window.markersJson);

    // Parse the JSON to ensure correct data structure
    const coordinates = parseJsonSafely(window.coordinatesJson);
    const markers = parseJsonSafely(window.markersJson);

    // Initialize the map
    initializeMap(coordinates, markers);
} else {
    console.warn("Coordinates or Markers JSON is missing.");
}

/**
 * Safely parse JSON and handle errors.
 * @param {string} jsonString - JSON string to parse.
 * @returns {object} Parsed JSON or an empty array.
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
    var norgeKart = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', {
        attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
    });

    // Initialize the map with a default center and zoom
    var map = L.map('map', {
        center: [58.1548282, 8.0081166],
        zoom: 10,
        layers: [norgeKart]
    });

    // Store marker references for interaction
    const markerMap = {};

    // Draw polygons if coordinatesJson is available
    if (coordinatesJson && coordinatesJson.length > 0) {
        try {
            coordinatesJson.forEach((polygonCoords) => {
                const latlngs = polygonCoords[0].map(coord => [coord[1], coord[0]]); // Convert [lng, lat] to [lat, lng]
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

    // Define a default and large marker icon
    const defaultIcon = L.divIcon({
        className: 'default-marker',
        html: '<div class="LokasjonpunktIkon"></div>',
        iconSize: [20, 20]
    });

    const largeIcon = L.divIcon({
        className: 'large-marker',
        html: '<div class="LokasjonpunktIkon"></div>',
        iconSize: [25, 25]
    });

    // Add markers with hover and click functionality
    if (markersJson && Array.isArray(markersJson)) {
        try {
            markersJson.forEach(marker => {
                if (marker.Nord && marker.Ost) {
                    const mapMarker = L.marker([marker.Nord, marker.Ost], { icon: defaultIcon })
                        .bindPopup(`Tittel: ${marker.Tittel}`)
                        .on('click', function () {
                            submitFormWithId(marker.RapportId); // Trigger the form submission
                        })
                        .on('mouseover', function () {
                            // Highlight corresponding table row
                            const tableRow = document.getElementById(`rapport-row-${marker.RapportId}`);
                            if (tableRow) {
                                tableRow.classList.add('highlight');
                            }

                            // Enlarge the marker
                            mapMarker.setIcon(largeIcon);
                        })
                        .on('mouseout', function () {
                            // Remove highlight from table row
                            const tableRow = document.getElementById(`rapport-row-${marker.RapportId}`);
                            if (tableRow) {
                                tableRow.classList.remove('highlight');
                            }

                            // Revert marker to default size
                            mapMarker.setIcon(defaultIcon);
                        });

                    // Store the marker for interaction with the table
                    markerMap[marker.RapportId] = mapMarker;

                    mapMarker.addTo(map);
                }
            });
        } catch (error) {
            console.error("Error processing markers JSON:", error);
        }
    }

    // Add hover events to the table rows
    document.querySelectorAll('tr[id^="rapport-row-"]').forEach(row => {
        const rapportId = row.id.replace('rapport-row-', '');

        row.addEventListener('mouseover', () => {
            if (markerMap[rapportId]) {
                markerMap[rapportId].setIcon(largeIcon); // Enlarge the marker
            }
        });

        row.addEventListener('mouseout', () => {
            if (markerMap[rapportId]) {
                markerMap[rapportId].setIcon(defaultIcon); // Revert marker to default size
            }
        });
    });
}

