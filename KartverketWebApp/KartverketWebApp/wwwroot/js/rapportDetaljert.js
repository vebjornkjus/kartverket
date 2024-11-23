class RapportDetaljert {
    constructor(appData) {
        if (!appData) {
            console.error('appData er ikke definert');
            return;
        }

        // Remove any existing instance
        if (window.rapportDetaljert) {
            window.rapportDetaljert.cleanup();
        }

        this.rapportId = appData.rapportId;
        this.mapData = appData.mapData;
        this.map = null;
        this.outsideClickHandler = this.handleOutsideClick.bind(this);

        console.log('Initializing with:', { rapportId: this.rapportId, mapData: this.mapData });

        // Initialize components
        this.initializeMap();
        this.setupEventListeners();

        // Store the instance
        window.rapportDetaljert = this;
    }

    cleanup() {
        document.removeEventListener('click', this.outsideClickHandler);
        if (this.map) {
            this.map.remove();
            this.map = null;
        }
    }

    initializeMap() {
        if (!this.mapData) {
            console.error('mapData er ikke definert');
            return;
        }

        if (this.mapData?.coordinates?.length > 0) {
            const firstCoord = this.mapData.coordinates[0];
            const mapElement = document.getElementById('map_detaljert');

            if (!mapElement) {
                console.error('map_detaljert element ikke funnet');
                return;
            }

            // Initialize the map
            this.map = L.map('map_detaljert').setView([firstCoord.nord, firstCoord.ost], 13);

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
            console.log('Map type:', this.mapData.mapType);
            switch (this.mapData.mapType) {
                case "Norge kart":
                    this.map.addLayer(norgeKart);
                    console.log('Added Norge kart layer');
                    break;
                case "Turkart":
                    this.map.addLayer(turKart);
                    console.log('Added Turkart layer');
                    break;
                case "Sjøkart":
                    this.map.addLayer(sjoKart);
                    console.log('Added Sjøkart layer');
                    break;
                default:
                    console.warn('Ukjent karttype:', this.mapData.mapType);
                    this.map.addLayer(norgeKart); // Default to Norge kart
                    break;
            }

            // Draw polyline for coordinates
            const latlngs = this.mapData.coordinates.map(coord => [coord.nord, coord.ost]);
            const polyline = L.polyline(latlngs, { color: 'blue' }).addTo(this.map);
            this.map.fitBounds(polyline.getBounds());
        } else {
            console.warn('Ingen koordinater tilgjengelig for visning på kartet');
        }
    }

    setupEventListeners() {
        // Close dropdown when clicking outside
        document.addEventListener('click', this.outsideClickHandler);

        // Set up comment form handler
        const commentForm = document.querySelector('.submission-content form');
        if (commentForm) {
            commentForm.addEventListener('submit', this.handleCommentSubmit.bind(this));
        } else {
            console.error('Comment form not found');
        }
    }

    handleOutsideClick(event) {
        const dropdown = document.getElementById('ansatteListe');
        const button = event.target.closest('.detbtn');
        const dropdownItem = event.target.closest('.dropdown-item');

        if (!button && !dropdownItem && dropdown && dropdown.style.display === 'block') {
            dropdown.style.display = 'none';
        }
    }

    async handleCommentSubmit(event) {
        event.preventDefault();

        console.log('Comment submission started');
        const commentInput = document.getElementById('newComment');
        const comment = commentInput.value.trim();

        if (!comment) {
            alert('Vennligst skriv en kommentar');
            return;
        }

        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            if (!token) {
                throw new Error('CSRF token not found');
            }

            const formData = new FormData();
            formData.append('innhold', comment);
            formData.append('rapportId', this.rapportId);

            console.log('Sending comment:', {
                rapportId: this.rapportId,
                comment: comment
            });

            const response = await fetch('/RapportStatus/AddComment', {
                method: 'POST',
                headers: {
                    'RequestVerificationToken': token
                },
                body: formData
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const result = await response.json();

            if (result.success) {
                console.log('Comment sent successfully');
                commentInput.value = '';
                alert('Kommentar ble sendt');
            } else {
                throw new Error(result.message || 'Kunne ikke sende kommentar');
            }
        } catch (error) {
            console.error('Error sending comment:', error);
            this.handleError(error, 'Kunne ikke sende kommentar');
        }
    }

    async visAnsatteListe(event) {
        if (event) {
            event.preventDefault();
            event.stopPropagation();
        }

        console.log('=== visAnsatteListe START ===');

        const liste = document.getElementById('ansatteListe');
        if (!liste) {
            console.error('ansatteListe element ikke funnet');
            return;
        }

        // Toggle display
        if (liste.style.display === 'block') {
            console.log('Liste er allerede synlig - skjuler den');
            liste.style.display = 'none';
            return;
        }

        try {
            console.log('Fetching ansatte data...');
            const response = await fetch('/RapportStatus/HentTilgjengeligeAnsatte');
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const ansatte = await response.json();

            // Detailed logging of the received data
            console.log('Raw ansatte data:', ansatte);
            console.log('Number of ansatte received:', ansatte.length);

            // Check for duplicates
            const uniqueIds = new Set(ansatte.map(a => a.AnsattId));
            console.log('Unique AnsattIds:', uniqueIds.size);

            // Clear existing content
            console.log('Clearing existing list content');
            liste.innerHTML = '';

            // Log before adding each item
            console.log('Adding ansatte to list...');
            ansatte.forEach((ansatt, index) => {
                console.log(`Creating button ${index + 1} for ansatt:`, ansatt);
                const button = document.createElement('button');
                button.className = 'dropdown-item';
                button.innerHTML = `
                    <span class="ansatt-navn">${ansatt.Fornavn} ${ansatt.Etternavn}</span>
                    ${ansatt.Kommunenavn ? `<span class="ansatt-kommune">${ansatt.Kommunenavn}</span>` : ''}
                `;
                button.onclick = this.createAnsattClickHandler(ansatt);
                liste.appendChild(button);
            });

            console.log('Setting list display to block');
            liste.style.display = 'block';

            // Log final DOM state
            console.log('Final number of buttons in list:', liste.querySelectorAll('.dropdown-item').length);

        } catch (error) {
            console.error('Error in visAnsatteListe:', error);
            this.handleError(error, 'Kunne ikke hente liste over tilgjengelige saksbehandlere');
        }

        console.log('=== visAnsatteListe END ===');
    }

    createAnsattClickHandler(ansatt) {
        return async (e) => {
            e.preventDefault();
            e.stopPropagation();
            if (confirm(`Er du sikker på at du vil overføre denne rapporten til ${ansatt.Fornavn} ${ansatt.Etternavn}?`)) {
                await this.tildelTilAnsatt(ansatt.AnsattId);
            }
        };
    }

    async tildelTilAnsatt(ansattId) {
        if (!this.rapportId || !ansattId) {
            console.error('Mangler rapportId eller ansattId');
            return;
        }

        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            if (!token) {
                throw new Error('Mangler CSRF token');
            }

            const response = await fetch('/RapportStatus/TildelRapport', {
                method: 'POST',
                headers: {
                    'Content-Type': 'application/json',
                    'RequestVerificationToken': token
                },
                body: JSON.stringify({
                    rapportId: this.rapportId,
                    nyAnsattId: ansattId
                })
            });

            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }

            const result = await response.json();
            if (result.success) {
                alert('Rapport ble overført vellykket');
                window.location.href = result.redirectUrl;
            } else {
                throw new Error(result.error || 'Kunne ikke tildele rapport');
            }
        } catch (error) {
            this.handleError(error, 'Kunne ikke tildele rapport til valgt saksbehandler');
        }
    }

    handleError(error, message) {
        console.error(message, error);
        alert(`${message}: ${error.message}`);
    }
}

// Global initialization
window.RapportDetaljert = RapportDetaljert;

// Global event handler
window.visAnsatteListe = function (event) {
    if (window.rapportDetaljert) {
        window.rapportDetaljert.visAnsatteListe(event);
    } else {
        console.error('rapportDetaljert instance not found');
    }
};

// Debugging helper
window.debugRapportDetaljert = function () {
    console.log('Current RapportDetaljert instance:', window.rapportDetaljert);
    console.log('AppData:', window.appData);
};