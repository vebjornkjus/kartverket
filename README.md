```
# **Kartverket Applikasjon** 🗺️

[![.NET](https://img.shields.io/badge/.NET-512BD4?style=for-the-badge&logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/)
[![Docker](https://img.shields.io/badge/Docker-2496ED?style=for-the-badge&logo=docker&logoColor=white)](https://www.docker.com/)
[![MariaDB](https://img.shields.io/badge/MariaDB-003545?style=for-the-badge&logo=mariadb&logoColor=white)](https://mariadb.org/)
![Build Status](https://github.com/vebjornkjus/kartverket/workflows/.NET%20Build%20&%20Test/badge.svg)

En applikasjon for administrasjon og visning av geografiske data, samt rapporthåndtering.

---

## **Innhold** 📑
- [Applikasjonens Oppsett](#applikasjonens-oppsett)
- [Komponenter](#komponenter)
- [Funksjonaliteter](#funksjonaliteter)
- [Brukerveiledning](#brukerveiledning)
- [Endringslogg](#endringslogg)

---

## **Applikasjonens Oppsett** ⚙️

### **Krav**
- Docker 🐳
- MariaDB 🗄️
- .NET SDK 🔧

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

## Contributors
<a href="https://github.com/vebjornkjus/kartverket/graphs/contributors">
  <img src="https://contrib.rocks/image?repo=vebjornkjus/kartverket" />
</a>

## **Komponenter** 🏗️

- **Arkitektur**: MVC (Model-View-Controller)
- **Database**: MariaDB
- **Visualisert database**: https://drive.google.com/file/d/138KNbuhLaRDdX2dy3fIppUxjwNX8oaid/view?usp=sharing 
- **Hovedklasser**:
  - `Rapport` 📝
  - `Steddata` 📍
  - `Kart` 🗺️
  - `Koordinater` 📐
  - `Person` 👤
  - `Bruker` 🔑
  - `Ansatt` 👔
  - `Meldinger` 💬

---

## **Funksjonaliteter** 💡

### **Standard Bruker** 👤
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

### **Saksbehandler** 👔
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

### **Administrator** 👑
- **Brukeradministrasjon:**  
  - Legge til, redigere, eller fjerne brukere (standardbrukere, Sakshendlere og Spesialbruker). Kommer
  - Administrere roller og rettigheter for brukere.(kommer)

---

### **Spesialbruker** ⚡
- **Proriterte rapporter:**  
  - Utrykkningspersonell eller lignende får rapporter prioritert(kommer)

---
## **Test scenario** 🧪
1. Opprett en bruker
2. Velg Kart type
3. Du kan teste ut de ulike verktøyene
4. Rapporter blir tildelt ansatt basert på kommune nummer
5. Tegne en rapport i Kristiansand for å følge scenarioet
6. Rapporter som blir rapportert i kommune det ikke finnes ansatte i, blir tildelt ansatt 
7. Fullfør og trykk rapporter
8. Fyll ut info. Legg til bilde om du vil. Rapporter!
9. Her kan du redigere rapporten.
10. Videre kan du gå til min side og se rapporten.

11. Logg ut og logg inn med:
   AgderKristiansand2@example.com
   passord123.
12. Dette er saksbehandler siden. Trykk på rapporten du netopp sendte inn
13. Bla helt ned til bunnen og send en melding. Denne vil komme i melding siden din.
14. Oppe til høyre, trykk send til og send til Nils. Du kan kun sende til andre ansatte i kristiansand
15. Bytt bruker, logg in med:
    AgderKristiansand@example.com
    passord123
16. Prøv å avklar en rapport og fjerne en annen. disse vil nå komme i tidligere rapporter siden.
17. Logg på brukeren du lagde.
18. I min side vil du kunne redigere og se status for rapporten
19. I meldinger vil du se meldingen du sendte til brukerern
20. Logg nå inn på en admin bruker:
    Admin@example.com
    passord123
21. Endre din bruker til saksbehandler. Prøv å slett en annen

---

## **Brukerveiledning** 📖
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

### Siste Endringer

[Endringer vil vises automatisk her]

###
