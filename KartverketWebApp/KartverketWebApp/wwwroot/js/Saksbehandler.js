document.addEventListener("DOMContentLoaded", function () {
    // Henter alle nav-elementene
    const navItems = document.querySelectorAll(".nav-item");

    // Legger til klikkhåndterer for hvert nav-element
    navItems.forEach(item => {
        item.addEventListener("click", function (e) {
            // Forhindrer standardoppførselen til klikket
            e.preventDefault();

            // Fjerner active-klassen fra alle nav-elementer
            navItems.forEach(nav => nav.classList.remove("active"));

            // Legger til active-klassen på det klikket nav-element
            this.classList.add("active");
        });
    });
});