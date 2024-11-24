class RapportDetaljert {
    constructor(appData) {
        // Sjekker om appData er tilgjengelig
        if (!appData) {
            console.error('appData er ikke definert');
            return;
        }

        // Fjerner eventuell eksisterende instans for å unngå duplikater
        if (window.rapportDetaljert) {
            window.rapportDetaljert.cleanup();
        }

        // Initialiserer hovedvariabler
        this.rapportId = appData.rapportId;
        this.mapData = appData.mapData;
        this.map = null;
        this.outsideClickHandler = this.handleOutsideClick.bind(this);

        console.log('Initialiserer med:', { rapportId: this.rapportId, mapData: this.mapData });

        // Setter opp kart og event listeners
        this.initializeMap();
        this.setupEventListeners();

        // Lagrer instansen globalt
        window.rapportDetaljert = this;
    }

    // Rydder opp i event listeners og kart når instansen avsluttes
    cleanup() {
        document.removeEventListener('click', this.outsideClickHandler);
        if (this.map) {
            this.map.remove();
            this.map = null;
        }
    }

    // Initialiserer kartet med valgt karttype og koordinater
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

            // Oppretter kartet med første koordinat som senterpunkt
            this.map = L.map('map_detaljert').setView([firstCoord.nord, firstCoord.ost], 13);

            // Definerer ulike kartlag
            const norgeKart = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/topo/default/webmercator/{z}/{y}/{x}.png', {
                attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
            });

            const turKart = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/toporaster/default/webmercator/{z}/{y}/{x}.png', {
                attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
            });

            const sjoKart = L.tileLayer('https://cache.kartverket.no/v1/wmts/1.0.0/sjokartraster/default/webmercator/{z}/{y}/{x}.png', {
                attribution: '&copy; <a href="https://www.kartverket.no/">Kartverket</a>'
            });

            // Velger riktig kartlag basert på mapType
            console.log('Karttype:', this.mapData.mapType);
            switch (this.mapData.mapType) {
                case "Norge kart":
                    this.map.addLayer(norgeKart);
                    console.log('La til Norge kart lag');
                    break;
                case "Turkart":
                    this.map.addLayer(turKart);
                    console.log('La til Turkart lag');
                    break;
                case "Sjøkart":
                    this.map.addLayer(sjoKart);
                    console.log('La til Sjøkart lag');
                    break;
                default:
                    console.warn('Ukjent karttype:', this.mapData.mapType);
                    this.map.addLayer(norgeKart);
                    break;
            }

            // Tegner linje mellom koordinatene på kartet
            const latlngs = this.mapData.coordinates.map(coord => [coord.nord, coord.ost]);
            const polyline = L.polyline(latlngs, { color: 'blue' }).addTo(this.map);
            this.map.fitBounds(polyline.getBounds());
        } else {
            console.warn('Ingen koordinater tilgjengelig for visning på kartet');
        }
    }

    // Setter opp event listeners for skjema og dropdown
    setupEventListeners() {
        document.addEventListener('click', this.outsideClickHandler);

        const commentForm = document.querySelector('.submission-content form');
        if (commentForm) {
            commentForm.addEventListener('submit', this.handleCommentSubmit.bind(this));
        } else {
            console.error('Kommentarskjema ikke funnet');
        }
    }

    // Håndterer klikk utenfor dropdown-menyen
    handleOutsideClick(event) {
        const dropdown = document.getElementById('ansatteListe');
        const button = event.target.closest('.detbtn');
        const dropdownItem = event.target.closest('.dropdown-item');

        if (!button && !dropdownItem && dropdown && dropdown.style.display === 'block') {
            dropdown.style.display = 'none';
        }
    }

    // Håndterer innsending av kommentarer
    async handleCommentSubmit(event) {
        event.preventDefault();

        console.log('Kommentar innsending startet');
        const commentInput = document.getElementById('newComment');
        const comment = commentInput.value.trim();

        if (!comment) {
            alert('Vennligst skriv en kommentar');
            return;
        }

        try {
            const token = document.querySelector('input[name="__RequestVerificationToken"]')?.value;
            if (!token) {
                throw new Error('CSRF token ikke funnet');
            }

            const formData = new FormData();
            formData.append('innhold', comment);
            formData.append('rapportId', this.rapportId);

            console.log('Sender kommentar:', {
                rapportId: this.rapportId,
                comment: comment
            });

            const response = await fetch('/Detaljert/AddComment', {
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
                console.log('Kommentar sendt vellykket');
                commentInput.value = '';
                alert('Kommentar ble sendt');
            } else {
                throw new Error(result.message || 'Kunne ikke sende kommentar');
            }
        } catch (error) {
            console.error('Feil ved sending av kommentar:', error);
            this.handleError(error, 'Kunne ikke sende kommentar');
        }
    }

    // Viser liste over tilgjengelige ansatte som rapporten kan overføres til
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

        if (liste.style.display === 'block') {
            console.log('Liste er allerede synlig - skjuler den');
            liste.style.display = 'none';
            return;
        }

        try {
            console.log('Henter ansatte data...');
            // Endret endpoint til å peke mot Detaljert controller
            const response = await fetch('/Detaljert/HentTilgjengeligeAnsatte');
            if (!response.ok) {
                throw new Error(`HTTP error! status: ${response.status}`);
            }
            const ansatte = await response.json();

            console.log('Mottatt ansatte data:', ansatte);
            console.log('Antall ansatte mottatt:', ansatte.length);

            const uniqueIds = new Set(ansatte.map(a => a.AnsattId));
            console.log('Unike AnsattIds:', uniqueIds.size);

            liste.innerHTML = '';

            console.log('Legger til ansatte i listen...');
            ansatte.forEach((ansatt, index) => {
                console.log(`Oppretter knapp ${index + 1} for ansatt:`, ansatt);
                const button = document.createElement('button');
                button.className = 'dropdown-item';
                button.innerHTML = `
                    <span class="ansatt-navn">${ansatt.Fornavn} ${ansatt.Etternavn}</span>
                    ${ansatt.Kommunenavn ? `<span class="ansatt-kommune">${ansatt.Kommunenavn}</span>` : ''}
                `;
                button.onclick = this.createAnsattClickHandler(ansatt);
                liste.appendChild(button);
            });

            liste.style.display = 'block';
            console.log('Antall knapper i listen:', liste.querySelectorAll('.dropdown-item').length);

        } catch (error) {
            console.error('Feil i visAnsatteListe:', error);
            this.handleError(error, 'Kunne ikke hente liste over tilgjengelige saksbehandlere');
        }

        console.log('=== visAnsatteListe END ===');
    }

    // Oppretter click handler for hver ansatt i listen
    createAnsattClickHandler(ansatt) {
        return async (e) => {
            e.preventDefault();
            e.stopPropagation();
            if (confirm(`Er du sikker på at du vil overføre denne rapporten til ${ansatt.Fornavn} ${ansatt.Etternavn}?`)) {
                await this.tildelTilAnsatt(ansatt.AnsattId);
            }
        };
    }

    // Håndterer tildeling av rapport til valgt ansatt
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

            // Endret endpoint til å peke mot Detaljert controller
            const response = await fetch('/Detaljert/TildelRapport', {
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

    // Generell feilhåndtering
    handleError(error, message) {
        console.error(message, error);
        alert(`${message}: ${error.message}`);
    }
}

// Globale initialiseringer og hjelpefunksjoner
window.RapportDetaljert = RapportDetaljert;

window.visAnsatteListe = function (event) {
    if (window.rapportDetaljert) {
        window.rapportDetaljert.visAnsatteListe(event);
    } else {
        console.error('rapportDetaljert instans ikke funnet');
    }
};

// Debugging hjelpefunksjon
window.debugRapportDetaljert = function () {
    console.log('Nåværende RapportDetaljert instans:', window.rapportDetaljert);
    console.log('AppData:', window.appData);
};