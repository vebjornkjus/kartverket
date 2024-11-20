Her er en **enkel mal** du kan kopiere **direkte** inn i en `README.md`-fil på GitHub:

```markdown
# **Kartverket Applikasjon**

En applikasjon for administrasjon og visning av geografiske data, samt rapporthåndtering.

---

## **Innhold**
- [Applikasjonens Oppsett](#applikasjonens-oppsett)
- [Komponenter](#komponenter)
- [Funksjonaliteter](#funksjonaliteter)
- [Brukerveiledning](#brukerveiledning)
- [Endringslogg](#endringslogg)

---

## **Applikasjonens Oppsett**

### **Krav**
- Docker
- MariaDB
- .NET SDK

### **Kjøring**
1. Klon repoet:
   ```bash
   git clone https://github.com/brukernavn/kartverket.git
   ```
2. Naviger til prosjektmappen:
   ```bash
   cd kartverket
   ```
3. Start med Docker Compose:
   ```bash
   docker-compose up --build
   ```
4. Åpne i nettleseren:
   ```
   http://localhost:5000
   ```

---

## **Komponenter**

- **Arkitektur**: MVC (Model-View-Controller)
- **Database**: MariaDB
- **Visualisert database**: https://drive.google.com/file/d/138KNbuhLaRDdX2dy3fIppUxjwNX8oaid/view?usp=sharing 
- **Hovedklasser**:
  - `Rapport`
  - `Steddata`
  - `Kart`
  - `Koordinater`
  - `Person`
  - `Bruker`
  - `Ansatt`
  - `Meldinger`

---

## **Funksjonaliteter**

### **Standard Bruker**
- **Tegne på kart og sende inn rapporter**  
  **Verktøy:**  
  - Polylinje-verktøy: For å tegne linjer på kartet.  
  - Dirkete sporing: Legger til punkter basert på hvor brukeren befinner seg.
- **Sende inn rapporter:**  
  - Rapporter inneholder tittel, beskrivelse, rapport-type, kart-type, koordinater, steddata og koordinatsystem.
  - Steddata (kommune, fylke) hnetes automatisk fra Kartverkets API.'
- **Meldingsfunksjon:**
  - Innmelder kan opprettholde kontakten med saksbehandler via meldings funksjon.   

---

### **Saksbehandler**
- **Oversikt over rapporter:**  
  - Se alle innsendte rapporter.
  - Rapporter blir automatisk tildelt bassert på kommunenummmer.  
  - Filtrere og sortere rapporter basert på status, dato eller sted.(Kommer)  
- **Håndtering av rapporter:**  
  - Tildele rapporter til andre ansatte. (Kommer) 
  - Endre status for rapporter (f.eks., "Under behandling", "Avsluttet").  
  - Behandle rapportdata direkte fra dashboardet.
- **Meldingsfunksjon:**
  - Saksbehandler kan opprettholde kontakten med Innmelder via meldings funksjon. 

---

### **Administrator**
- **Brukeradministrasjon:**  
  - Legge til, redigere, eller fjerne brukere (standardbrukere, Sakshendlere og Spesialbruker). Kommer
  - Administrere roller og rettigheter for brukere.(kommer)

---

### **Spesialbruker**
- **Proriterte rapporter:**  
  - Utrykkningspersonell eller lignende får rapporter prioritert(kommer)

---

## **Brukerveiledning**
- **Innmelder**
  1. Velg kart for rapporten
  2. Velg verktøy
  3. Fyll ut skjema
  4. Send inn
---
 
- **Saksbehandler**
    1. Trykk på til oversikt
    2. Velge side (Oversikt, varslinger, rapporter, meldinger, tidligere rapporter)
    3. Hvis du velger rapporter: Grønne rader er uåpnet.
    4. Trykk på en rad for å se detaljert rapport.
    5. Velg handling for rapporten.(Kommer)

---

## **Endringslogg**

| **Dato**       | **Commit melding**                                      | **Forfatter**            | **Verifisert** |
|-----------------|--------------------------------------------------------|-----------------------|--------------|
| **Nov 15, 2024** | Lagd kjempe bra saksbehandler side                     | FredrikHusebruh       | No           |
| **Nov 14, 2024** | Lagt til test data i applicationdbcontext              | FredrikHusebruh       | No           |
| **Nov 14, 2024** | Merge nye endringer                                    | FredrikHusebruh       | No           |
| **Nov 14, 2024** | Bygd database på ny og lagt til direktesporingsverktøy på kartet og fikset bugs | FredrikHusebruh | No           |
| **Nov 11, 2024** | Merge pull request #10 from vebjornkjus/revert-7-VebjørnSinNyeBranch1 | vebjornkjus | Yes          |
| **Nov 11, 2024** | HashedPassword og RetryOnFailure                       | vebjornkjus           | Yes          |
| **Nov 10, 2024** | Lagd design for rapportdetaljert view og lagt til maplayer under hovedkartet | FredrikHusebruh | No           |
| **Nov 10, 2024** | Brukere kan logge inn                                  | vebjornkjus           | Yes          |
| **Nov 10, 2024** | Lagd ny dynamisk RapportDetaljert view og gjort rader i saksbehandler view klikkbare | FredrikHusebruh | No           |
| **Nov 8, 2024**  | Lagt til geolocation knapp                             | FredrikHusebruh       | No           |
| **Nov 8, 2024**  | Gjort det mulig å sende inn data til db og hente ut stednavn fra API | FredrikHusebruh | No           |
| **Nov 8, 2024**  | Lagt til geolokasjon                                   | Kristiank02           | Yes          |
| **Nov 7, 2024**  | Registrere bruker til database                         | vebjornkjus           | No           |
| **Nov 7, 2024**  | Fikset opp i merge og database tilkobling              | FredrikHusebruh       | No           |
| **Nov 6, 2024**  | Lagt til både login-side og register-side              | vebjornkjus           | No           |
| **Nov 5, 2024**  | Lagt til innloggingsside                               | vebjornkjus           | No           |
| **Oct 31, 2024** | Database test data                                     | FredrikHusebruh       | No           |
| **Oct 29, 2024** | Tvangspush                                             | FredrikHusebruh       | No           |
| **Oct 26, 2024** | Lagt til tegneverktøy og flere punkter i rapport       | FredrikHusebruh       | No           |
| **Oct 17, 2024** | Lagt til Søkefunksjon for å finne kommuner             | FredrikHusebruh       | No           |
| **Oct 14, 2024** | API og kart i CorrectionsOverview funker som det skal  | FredrikHusebruh       | No           |
| **Oct 13, 2024** | Fortsetter med API strevet :(                          | FredrikHusebruh       | No           |
| **Oct 9, 2024**  | Prøvd å legge til API                                  | FredrikHusebruh       | No           |
| **Sep 30, 2024** | Lagt til ny forside og annet                           | FredrikHusebruh       | No           |
| **Sep 27, 2024** | Ny prototype                                           | FredrikHusebruh       | No           |
| **Sep 25, 2024** | Kart CSS                                               | FredrikHusebruh       | No           |
| **Sep 13, 2024** | Lagt inn prototype                                     | FredrikHusebruh       | No           |
| **Sep 10, 2024** | Lagt til web-applikasjon struktur                      | vebjornkjus           | Yes          |
| **Sep 10, 2024** | Initial commit                                         | vebjornkjus           | Yes          |

---
