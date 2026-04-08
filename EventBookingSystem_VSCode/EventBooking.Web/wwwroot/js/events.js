/**
 * events.js – Event listing & booking
 * Fetches from api/events (public) and posts to api/bookings (JWT required).
 */

const API_BASE = 'https://localhost:7100';
let allEvents  = [];
let selectedEvent = null;

const categoryColors = {
    'Music':       'bg-danger',
    'Tech':        'bg-primary',
    'Sports':      'bg-success',
    'Workshop':    'bg-warning text-dark',
    'Conference':  'bg-info text-dark',
    'General':     'bg-secondary',
};

// ── Load Events ─────────────────────────────────────────────────────────────
async function loadEvents() {
    try {
        const res = await fetch(`${API_BASE}/api/events`);
        if (!res.ok) throw new Error(`Server error: ${res.status}`);
        allEvents = await res.json();
        renderEvents(allEvents);
    } catch (err) {
        document.getElementById('loadingSpinner').classList.add('d-none');
        showAlert('alertBox', `<i class="bi bi-exclamation-triangle me-2"></i>Failed to load events. Is the API running?<br><small>${err.message}</small>`, 'warning');
    }
}

function renderEvents(events) {
    const spinner = document.getElementById('loadingSpinner');
    const grid    = document.getElementById('eventsGrid');
    const noRes   = document.getElementById('noResults');

    spinner.classList.add('d-none');

    if (events.length === 0) {
        grid.style.removeProperty('display');
        grid.classList.add('d-none');
        noRes.classList.remove('d-none');
        return;
    }

    noRes.classList.add('d-none');
    grid.style.removeProperty('display');
    grid.classList.remove('d-none');

    grid.innerHTML = events.map(e => {
        const colorClass = categoryColors[e.category] || 'bg-secondary';
        const dateStr    = new Date(e.date).toLocaleDateString('en-IN', { dateStyle: 'medium' });
        const timeStr    = new Date(e.date).toLocaleTimeString('en-IN', { timeStyle: 'short' });
        const isSoldOut  = e.availableSeats === 0;
        const price      = e.ticketPrice > 0 ? `₹${e.ticketPrice.toFixed(2)}` : 'Free';

        return `
        <div class="col-sm-6 col-xl-4">
          <div class="card event-card">
            <div class="card-header ${colorClass} text-white d-flex justify-content-between align-items-center">
              <span class="badge badge-category bg-white bg-opacity-25">${e.category}</span>
              <span class="seats-badge badge bg-white bg-opacity-25">
                <i class="bi bi-person-fill me-1"></i>${e.availableSeats} seats
              </span>
            </div>
            <div class="card-body d-flex flex-column">
              <h5 class="card-title fw-bold mb-1">${escHtml(e.title)}</h5>
              <p class="card-text text-muted small mb-2 flex-grow-1">${escHtml(e.description)}</p>
              <ul class="list-unstyled small text-muted mb-3">
                <li><i class="bi bi-calendar3 me-1 text-primary"></i>${dateStr} &nbsp;${timeStr}</li>
                <li><i class="bi bi-geo-alt me-1 text-danger"></i>${escHtml(e.location)}</li>
                <li><i class="bi bi-tag me-1 text-success"></i>${price}</li>
              </ul>
              <button class="btn ${isSoldOut ? 'btn-outline-secondary disabled' : 'btn-primary'} w-100"
                      onclick="openBookingModal(${e.id})"
                      ${isSoldOut ? 'disabled' : ''}>
                <i class="bi bi-ticket me-1"></i>${isSoldOut ? 'Sold Out' : 'Book Now'}
              </button>
            </div>
          </div>
        </div>`;
    }).join('');
}

// ── Search ───────────────────────────────────────────────────────────────────
document.getElementById('searchInput').addEventListener('input', function () {
    const q = this.value.toLowerCase();
    renderEvents(allEvents.filter(e =>
        e.title.toLowerCase().includes(q) ||
        e.location.toLowerCase().includes(q) ||
        e.category.toLowerCase().includes(q)
    ));
});

// ── Booking Modal ────────────────────────────────────────────────────────────
function openBookingModal(eventId) {
    selectedEvent = allEvents.find(e => e.id === eventId);
    if (!selectedEvent) return;

    document.getElementById('modalEventTitle').textContent = selectedEvent.title;
    document.getElementById('modalEventMeta').textContent  =
        `${new Date(selectedEvent.date).toLocaleString('en-IN')} — ${selectedEvent.location}`;
    document.getElementById('modalAvailable').textContent  = selectedEvent.availableSeats;
    document.getElementById('modalPrice').textContent =
        selectedEvent.ticketPrice > 0 ? `₹${selectedEvent.ticketPrice.toFixed(2)} / seat` : 'Free';

    const seatsInput = document.getElementById('seatsInput');
    seatsInput.value = 1;
    seatsInput.max   = selectedEvent.availableSeats;
    seatsInput.classList.remove('is-invalid');
    document.getElementById('modalAlert').innerHTML = '';

    new bootstrap.Modal(document.getElementById('bookingModal')).show();
}

// ── Client-side Validation ───────────────────────────────────────────────────
function validateSeats() {
    const input = document.getElementById('seatsInput');
    const seats = parseInt(input.value, 10);
    const errEl = document.getElementById('seatsError');

    input.classList.remove('is-invalid');

    if (isNaN(seats) || seats < 1) {
        errEl.textContent = 'Please enter at least 1 seat.';
        input.classList.add('is-invalid');
        return null;
    }
    if (seats > 100) {
        errEl.textContent = 'Maximum 100 seats per booking.';
        input.classList.add('is-invalid');
        return null;
    }
    if (seats > selectedEvent.availableSeats) {
        errEl.textContent = `Only ${selectedEvent.availableSeats} seat(s) available.`;
        input.classList.add('is-invalid');
        return null;
    }
    return seats;
}

// ── Submit Booking ────────────────────────────────────────────────────────────
document.getElementById('confirmBookingBtn').addEventListener('click', async () => {
    const seats = validateSeats();
    if (seats === null) return;

    const token = TokenStore.get();
    if (!token) {
        showAlert('modalAlert', '<i class="bi bi-key me-1"></i>No JWT token found. Please <a href="/Auth/Token">get a token</a> first.', 'warning');
        return;
    }

    const btn = document.getElementById('confirmBookingBtn');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Booking…';

    try {
        const res = await fetch(`${API_BASE}/api/bookings`, {
            method:  'POST',
            headers: TokenStore.headers(),
            body:    JSON.stringify({ eventId: selectedEvent.id, seatsBooked: seats })
        });

        const data = await res.json();

        if (!res.ok) {
            showAlert('modalAlert', `<i class="bi bi-x-circle me-1"></i>${data.message || 'Booking failed.'}`, 'danger');
        } else {
            // Update local seat count
            selectedEvent.availableSeats -= seats;
            renderEvents(allEvents);

            bootstrap.Modal.getInstance(document.getElementById('bookingModal')).hide();
            showAlert('alertBox',
                `<i class="bi bi-check-circle me-1"></i>Booking confirmed! <strong>${seats}</strong> seat(s) for <strong>${selectedEvent.title}</strong>. Booking ID: #${data.id}`,
                'success');
        }
    } catch (err) {
        showAlert('modalAlert', `<i class="bi bi-exclamation-triangle me-1"></i>Network error: ${err.message}`, 'danger');
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="bi bi-check-circle me-1"></i>Confirm Booking';
    }
});

// ── Helpers ──────────────────────────────────────────────────────────────────
function escHtml(str) {
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;')
        .replace(/"/g, '&quot;');
}

// ── Init ─────────────────────────────────────────────────────────────────────
loadEvents();
