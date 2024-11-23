
class RapportDetaljert {
    constructor(mapData) {
        this.mapData = mapData;
        this.initializeMap();
        this.setupEventListeners();
    }

    initializeMap() {
        if (this.mapData && this.mapData.mapType && this.mapData.coordinates && this.mapData.coordinates.length > 0) {
            const firstCoord = this.mapData.coordinates[0];
            this.map = L.map('map_detaljert').setView([firstCoord.nord, firstCoord.ost], 13);

            const norgeKart = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', {
                attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
            });
            const turKart = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/toporaster/default/webmercator/{z}/{y}/{x}.png', {
                attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
            });
            const sjoKart = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png', {
                attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
            });

            console.log("Selecting map layer for mapType:", this.mapData.mapType);

            if (this.mapData.mapType === "Norge kart") {
                this.map.addLayer(norgeKart);
                console.log("Added Norge kart layer");
            } else if (this.mapData.mapType === "Turkart") {
                this.map.addLayer(turKart);
                console.log("Added Turkart layer");
            } else if (this.mapData.mapType === "Sjøkart") {
                this.map.addLayer(sjoKart);
                console.log("Added Sjøkart layer");
            } else {
                console.warn('Map type not recognized:', this.mapData.mapType);
            }

            const latlngs = this.mapData.coordinates.map(coord => [coord.nord, coord.ost]);
            const polyline = L.polyline(latlngs, { color: 'blue' }).addTo(this.map);
            this.map.fitBounds(polyline.getBounds());
        } else {
            console.warn('No coordinates available to display on the map or mapType is missing.');
        }
    }

    setupEventListeners() {
        document.addEventListener('click', this.handleOutsideClick.bind(this));
    }

    handleOutsideClick(event) {
        const dropdown = document.getElementById('ansatteListe');
        const button = event.target.closest('.detbtn');
        if (!button && dropdown && dropdown.style.display === 'block') {
            dropdown.style.display = 'none';
        }
    }

    static async visAnsatteListe() {
        const liste = document.getElementById('ansatteListe');

        if (liste.style.display === 'block') {
            liste.style.display = 'none';
            return;
        }

        try {
            const response = await fetch('/RapportStatus/HentTilgjengeligeAnsatte');
            const ansatte = await response.json();

            liste.innerHTML = '';
            ansatte.forEach(ansatt => {
                const button = document.createElement('button');
                button.className = 'dropdown-item';
                button.innerHTML = `
                    <span class="ansatt-navn">${ansatt.fulltNavn}</span>
                    <span class="ansatt-kommune">${ansatt.kommunenavn}</span>
                `;
                button.onclick = (e) => {
                    e.preventDefault();
                    if (confirm(`Er du sikker på at du vil overføre denne rapporten til ${ansatt.fulltNavn}?`)) {
                        RapportDetaljert.tildelTilAnsatt(ansatt.ansattId);
                    }
                };
                liste.appendChild(button);
            });
            liste.style.display = 'block';
        } catch (error) {
            console.error('Error:', error);
            alert('Kunne ikke hente liste over tilgjengelige saksbehandlere');
        }
    }

    static tildelTilAnsatt(ansattId) {
        const form = document.createElement('form');
        form.method = 'post';
        form.action = `/RapportStatus/TildelRapport?rapportId=${window.rapportId}&nyAnsattId=${ansattId}`;
        document.body.appendChild(form);
        form.submit();
    }
}

// Export for use in other files if needed
window.RapportDetaljert = RapportDetaljert;