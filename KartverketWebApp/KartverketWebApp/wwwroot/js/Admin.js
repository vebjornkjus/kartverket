document.addEventListener("DOMContentLoaded", function () {
    const brukerTypeField = document.getElementById("BrukerType");
    const kommuneNummerField = document.getElementById("Kommunenummer");
    const kommuneNummerContainer = document.getElementById("kommuneNummerContainer");

    function toggleKommunenummer() {
        // Vis felt kun når brukertypen er saksbehandler
        if (brukerTypeField.value === "saksbehandler") {
            kommuneNummerContainer.style.display = "block";
            kommuneNummerField.required = true;
        } else {
            kommuneNummerContainer.style.display = "none";
            kommuneNummerField.required = false;
            kommuneNummerField.value = ''; // Tøm verdien når ikke saksbehandler
        }
    }

    brukerTypeField.addEventListener("change", toggleKommunenummer);
    toggleKommunenummer(); // Initialiser visning ved sideinnlasting
});