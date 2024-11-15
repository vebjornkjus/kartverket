document.addEventListener("DOMContentLoaded", function () {
    const navItems = document.querySelectorAll(".nav-item");
    const tabContents = document.querySelectorAll(".tab-content");

    navItems.forEach(item => {
        item.addEventListener("click", function (e) {
            e.preventDefault();

            // Remove active class from all nav items
            navItems.forEach(nav => nav.classList.remove("active"));
            // Add active class to the clicked item
            this.classList.add("active");

            // Hide all tab contents
            tabContents.forEach(tab => tab.classList.remove("active"));

            // Show the selected tab content
            const target = this.getAttribute("data-target");
            document.querySelector(target).classList.add("active");
        });
    });
});
