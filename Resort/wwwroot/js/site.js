// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.

(() => {
    const sheet = document.getElementById("filterSheet");
    const open = document.getElementById("openFilter");
    const close1 = document.getElementById("closeFilter");
    const close2 = document.getElementById("closeFilter2");

    if (!sheet || !open) return;

    const openSheet = () => { sheet.classList.add("is-open"); sheet.setAttribute("aria-hidden", "false"); };
    const closeSheet = () => { sheet.classList.remove("is-open"); sheet.setAttribute("aria-hidden", "true"); };

    open.addEventListener("click", openSheet);
    close1?.addEventListener("click", closeSheet);
    close2?.addEventListener("click", closeSheet);

    document.addEventListener("keydown", (e) => {
        if (e.key === "Escape") closeSheet();
    });
})();
