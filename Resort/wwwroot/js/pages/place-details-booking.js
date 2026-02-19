(() => {
    const form = document.querySelector("[data-booking]");
    if (!form) return;

    const placeId = Number(form.dataset.placeId);
    const pricePerNight = Number(form.dataset.price);
    const bookedDaysUrl = form.dataset.bookedDaysUrl;

    const titleEl = form.querySelector("[data-cal-title]");
    const gridEl = form.querySelector("[data-cal-grid]");
    const prevBtn = form.querySelector("[data-cal-prev]");
    const nextBtn = form.querySelector("[data-cal-next]");

    const checkInEl = form.querySelector("[data-checkin]");
    const checkOutEl = form.querySelector("[data-checkout]");
    const metaEl = form.querySelector("[data-meta]");

    const fromHidden = form.querySelector("[data-from]");
    const toHidden = form.querySelector("[data-to]");
    const submitBtn = form.querySelector("[data-submit]");

    const today = dateOnly(new Date());
    let view = new Date(today.getFullYear(), today.getMonth(), 1);
    let start = null;          
    let endDisplay = null;     
    let bookedSet = new Set(); 

    prevBtn.addEventListener("click", async () => {
        view = new Date(view.getFullYear(), view.getMonth() - 1, 1);
        await loadMonth();
    });

    nextBtn.addEventListener("click", async () => {
        view = new Date(view.getFullYear(), view.getMonth() + 1, 1);
        await loadMonth();
    });

    form.addEventListener("submit", (e) => {
        if (!fromHidden.value || !toHidden.value) {
            e.preventDefault();
            showMeta("Please select at least 1 day.");
        }
    });

    function pad2(n) { return String(n).padStart(2, "0"); }
    function key(d) { return `${d.getFullYear()}-${pad2(d.getMonth() + 1)}-${pad2(d.getDate())}`; }
    function dateOnly(d) { return new Date(d.getFullYear(), d.getMonth(), d.getDate()); }
    function addDays(d, n) { const x = new Date(d); x.setDate(x.getDate() + n); return dateOnly(x); }
    function fmt(d) { return `${pad2(d.getDate())}/${pad2(d.getMonth() + 1)}/${d.getFullYear()}`; }

    function nightsCount(fromInclusive, toExclusive) {
        const ms = (toExclusive - fromInclusive);
        return Math.max(1, Math.round(ms / (1000 * 60 * 60 * 24)));
    }

    function monthName(m) {
        return ["January", "February", "March", "April", "May", "June", "July", "August", "September", "October", "November", "December"][m];
    }

    function showMeta(msg, isError = false) {
        metaEl.innerHTML = `<span class="muted">${msg}</span>`;
        if (isError) metaEl.style.opacity = "1";
    }

    async function fetchBookedDays(year, month1to12) {
        const url = `${bookedDaysUrl}?placeId=${placeId}&year=${year}&month=${month1to12}`;
        const res = await fetch(url, { headers: { "Accept": "application/json" } });
        if (!res.ok) return new Set();
        const arr = await res.json();
        return new Set(arr);
    }

    async function loadMonth() {
        bookedSet = await fetchBookedDays(view.getFullYear(), view.getMonth() + 1);
        render();
        syncUI();
    }

    function render() {
        titleEl.textContent = `${monthName(view.getMonth())} ${view.getFullYear()}`;
        gridEl.innerHTML = "";

        const first = new Date(view.getFullYear(), view.getMonth(), 1);
        const last = new Date(view.getFullYear(), view.getMonth() + 1, 0);
        const startWeekday = first.getDay();

        // empty prefix
        for (let i = 0; i < startWeekday; i++) {
            const empty = document.createElement("div");
            empty.className = "cal-day is-empty";
            gridEl.appendChild(empty);
        }

        for (let day = 1; day <= last.getDate(); day++) {
            const d = dateOnly(new Date(view.getFullYear(), view.getMonth(), day));
            const k = key(d);

            const btn = document.createElement("button");
            btn.type = "button";
            btn.className = "cal-day";
            btn.textContent = String(day);
            btn.dataset.date = k;

            const isPast = d < today;
            const isBooked = bookedSet.has(k);

            if (isPast || isBooked) {
                btn.classList.add("is-disabled");
                btn.disabled = true;
            }

            // styles for selection
            if (start && key(start) === k) btn.classList.add("is-start");
            if (endDisplay && key(endDisplay) === k) btn.classList.add("is-end");
            if (start && endDisplay && d > start && d < endDisplay) btn.classList.add("is-in-range");

            btn.addEventListener("click", () => onPick(d));

            gridEl.appendChild(btn);
        }
    }

    function rangeHasBooked(fromInclusive, toInclusive) {

        let cur = dateOnly(fromInclusive);
        const end = dateOnly(toInclusive);
        while (cur <= end) {
            if (bookedSet.has(key(cur))) return true;
            cur = addDays(cur, 1);
        }
        return false;
    }

    function onPick(d) {

        if (!start || (start && endDisplay)) {
            start = d;
            endDisplay = null;
            render();
            syncUI();
            return;
        }


        if (d < start) {
            start = d;
            endDisplay = null;
            render();
            syncUI();
            return;
        }

        endDisplay = d;

        // validate booked days inside range
        if (rangeHasBooked(start, endDisplay)) {
            endDisplay = null;
            render();
            showMeta("This range contains booked days. Choose different dates.", true);
            syncUI(false);
            return;
        }

        render();
        syncUI();
    }

    function syncUI(resetMeta = true) {
        if (!start) {
            checkInEl.value = "";
            checkOutEl.value = "";
            fromHidden.value = "";
            toHidden.value = "";
            submitBtn.disabled = true;
            if (resetMeta) showMeta("Select your dates to enable booking.");
            return;
        }

        const displayOut = endDisplay ?? start;

        const toExclusive = addDays(displayOut, 1);

        checkInEl.value = `${fmt(start)} • 09:00 AM`;
        checkOutEl.value = `${fmt(toExclusive)} • 12:00 PM`;


        fromHidden.value = key(start);
        toHidden.value = key(toExclusive);

        const nights = nightsCount(start, toExclusive);
        const total = (isNaN(pricePerNight) ? 0 : pricePerNight) * nights;

        submitBtn.disabled = false;

        if (resetMeta) {
            showMeta(`Selected: ${nights} day(s) • Estimated: ${Math.round(total)} SAR`);
        }
    }

    loadMonth();
})();
