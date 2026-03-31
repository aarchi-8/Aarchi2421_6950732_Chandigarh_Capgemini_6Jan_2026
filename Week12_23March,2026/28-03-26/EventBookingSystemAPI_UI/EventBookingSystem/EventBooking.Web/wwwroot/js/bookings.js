// ── API CONFIG ─────────────────────────────────────────
const API_BASE = 'https://localhost:7676';
let cancelTargetId = null;

// ── LOAD BOOKINGS ──────────────────────────────────────
async function loadBookings() {
    const token = TokenStore.get();

    if (!token) {
        hideLoader();
        showAlert('alertBox',
            `<div class="d-flex align-items-center">
                <i class="bi bi-key fs-5 me-2"></i>
                Please <a href="/Auth/Token" class="fw-semibold ms-1">get a JWT token</a> to view bookings.
            </div>`,
            'warning');
        return;
    }

    try {
        const res = await fetch(`${API_BASE}/api/bookings`, {
            headers: TokenStore.headers()
        });

        if (res.status === 401) {
            hideLoader();
            showAlert('alertBox',
                `<i class="bi bi-shield-lock me-1"></i> Session expired. 
                 <a href="/Auth/Token" class="fw-semibold">Get new token</a>`,
                'warning');
            return;
        }

        const bookings = await res.json();
        renderBookings(bookings);

    } catch (err) {
        hideLoader();
        showAlert('alertBox',
            `<i class="bi bi-exclamation-triangle me-1"></i> ${err.message}`,
            'danger');
    }
}

// ── RENDER BOOKINGS ────────────────────────────────────
function renderBookings(bookings) {
    hideLoader();

    if (!bookings.length) {
        document.getElementById('emptyState').classList.remove('d-none');
        return;
    }

    document.getElementById('bookingsContainer').classList.remove('d-none');

    const tbody = document.getElementById('bookingsTableBody');

    tbody.innerHTML = bookings.map(b => {
        const eventDate = new Date(b.eventDate).toLocaleDateString('en-IN', { dateStyle: 'medium' });
        const bookedDate = new Date(b.bookedAt).toLocaleDateString('en-IN', { dateStyle: 'short' });

        const statusBadge = b.status === 'Confirmed'
            ? `<span class="badge bg-success-subtle text-success px-3 py-2 rounded-pill">✔ Confirmed</span>`
            : `<span class="badge bg-danger-subtle text-danger px-3 py-2 rounded-pill">✖ Cancelled</span>`;

        return `
        <tr class="fade-in">
          <td class="text-muted small">#${b.id}</td>

          <td>
            <div class="fw-semibold">${escHtml(b.eventTitle)}</div>
            <div class="text-muted small">${escHtml(b.eventLocation)}</div>
          </td>

          <td>${eventDate}</td>

          <td class="text-center">
            <span class="badge bg-primary rounded-pill px-3">${b.seatsBooked}</span>
          </td>

          <td class="text-muted small">${bookedDate}</td>

          <td class="text-center">
            ${statusBadge}
          </td>

          <td class="text-center">
            ${b.status === 'Confirmed'
                ? `<button class="btn btn-sm btn-outline-danger rounded-pill px-3"
                    onclick="openCancelModal(${b.id})">
                    <i class="bi bi-x-circle me-1"></i>Cancel
                   </button>`
                : `<span class="text-muted small">—</span>`}
          </td>
        </tr>`;
    }).join('');
}

// ── CANCEL MODAL ───────────────────────────────────────
function openCancelModal(bookingId) {
    cancelTargetId = bookingId;
    document.getElementById('cancelBookingId').textContent = `#${bookingId}`;
    new bootstrap.Modal(document.getElementById('cancelModal')).show();
}

// ── CONFIRM CANCEL ─────────────────────────────────────
document.getElementById('confirmCancelBtn').addEventListener('click', async () => {
    if (!cancelTargetId) return;

    const btn = document.getElementById('confirmCancelBtn');
    btn.disabled = true;
    btn.innerHTML = `<span class="spinner-border spinner-border-sm me-1"></span>Cancelling...`;

    try {
        const res = await fetch(`${API_BASE}/api/bookings/${cancelTargetId}`, {
            method: 'DELETE',
            headers: TokenStore.headers()
        });

        bootstrap.Modal.getInstance(document.getElementById('cancelModal')).hide();

        if (res.ok || res.status === 204) {
            showAlert('alertBox',
                `<i class="bi bi-check-circle me-1"></i> Booking #${cancelTargetId} cancelled successfully`,
                'success');

            reloadTable();

        } else {
            const data = await res.json().catch(() => ({}));
            showAlert('alertBox',
                `<i class="bi bi-x-circle me-1"></i> ${data.message || 'Cancellation failed'}`,
                'danger');
        }

    } catch (err) {
        showAlert('alertBox',
            `<i class="bi bi-exclamation-triangle me-1"></i> ${err.message}`,
            'danger');
    } finally {
        btn.disabled = false;
        btn.innerHTML = `<i class="bi bi-x-circle me-1"></i>Cancel Booking`;
        cancelTargetId = null;
    }
});

// ── HELPERS ───────────────────────────────────────────
function hideLoader() {
    document.getElementById('loadingSpinner').classList.add('d-none');
}

function reloadTable() {
    document.getElementById('bookingsContainer').classList.add('d-none');
    document.getElementById('loadingSpinner').classList.remove('d-none');
    loadBookings();
}

function escHtml(str) {
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;');
}

// ── INIT ──────────────────────────────────────────────
loadBookings();