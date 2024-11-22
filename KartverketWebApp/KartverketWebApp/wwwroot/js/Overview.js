document.addEventListener("DOMContentLoaded", function () {
    // Ensure the data is available globally
    if (window.coordinatesJson && window.markersJson) {
        console.log("Coordinates JSON (Raw):", window.coordinatesJson);
        console.log("Markers JSON (Raw):", window.markersJson);

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
     * @returns {Array} Parsed JSON or an empty array.
     */
    function parseJsonSafely(jsonString) {
        try {
            const parsed = JSON.parse(jsonString);
            if (Array.isArray(parsed)) {
                return parsed; // Ensure the parsed result is an array
            }
            console.warn("Parsed JSON is not an array:", parsed);
            return [];
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

        // Initialize the map with a default center and zoom
        const map = L.map('map-oversikt', {
            center: [58.1548282, 8.0081166],
            zoom: 10,
            layers: [norgeKart]
        });

        // Store marker references for interaction
        const markerMap = {};

        // Draw polygons if coordinatesJson is available
        if (Array.isArray(coordinatesJson) && coordinatesJson.length > 0) {
            try {
                coordinatesJson.forEach((polygonCoords) => {
                    if (Array.isArray(polygonCoords[0])) {
                        const latlngs = polygonCoords[0].map(coord => [coord[1], coord[0]]); // Convert [lng, lat] to [lat, lng]
                        L.polygon(latlngs, {
                            color: '#848BEA',
                            fillColor: '#92B4EF',
                            fillOpacity: 0.2
                        }).addTo(map);
                    } else {
                        console.warn("Invalid polygon coordinates:", polygonCoords);
                    }
                });
            } catch (error) {
                console.error("Error processing polygon coordinates:", error);
            }
        } else {
            console.warn("No valid polygon data available.");
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
        if (Array.isArray(markersJson) && markersJson.length > 0) {
            try {
                markersJson.forEach(marker => {
                    if (marker.Nord && marker.Ost) {
                        console.log("Adding marker:", marker); // Log each marker being processed
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
                    } else {
                        console.warn("Invalid marker coordinates:", marker);
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
});
