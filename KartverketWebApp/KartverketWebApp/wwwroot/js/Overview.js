// Ensure the data is available globally
if (window.coordinatesJson && window.markersJson) {
    console.log("Coordinates JSON:", window.coordinatesJson);
    console.log("Markers JSON:", window.markersJson);

    // Parse the JSON to ensure correct data structure
    const coordinates = JSON.parse(window.coordinatesJson);
    const markers = JSON.parse(window.markersJson);

    // Initialize the map
    initializeMap(coordinates, markers);
} else {
    console.warn("Coordinates or Markers JSON is missing.");
}

// Function to initialize the map
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

    // Process the coordinates JSON to draw polygons
    if (coordinatesJson && coordinatesJson.length > 0) {
        try {
            // Assuming coordinatesJson contains MultiPolygon coordinates
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
    } else {
        console.warn("Coordinates JSON is empty or invalid.");
    }

    // Define a custom invisible icon
    var invisibleIcon = L.divIcon({
        className: 'invisible-marker', // Use a custom CSS class to make the marker invisible
        html: '<div class="LokasjonpunktIkon"></div>',
        iconSize: [20, 20] // Set size to 0 to ensure it doesn't render
    });

    // Process the markers JSON to place invisible markers on the map
    if (markersJson && Array.isArray(markersJson)) {
        try {
            markersJson.forEach(marker => {
                if (marker.Nord && marker.Ost) {
                    L.marker([marker.Nord, marker.Ost], { icon: invisibleIcon })
                        .bindPopup(`Tittel: ${marker.Tittel}`)
                        .addTo(map);
                }
            });
        } catch (error) {
            console.error("Error processing markers JSON:", error);
        }
    } else {
        console.warn("Markers JSON is empty or invalid.");
    }
}
