/**
 * bookings.js – My Bookings page
 * Loads bookings from api/bookings (JWT required) and handles cancellation.
 */

const API_BASE = 'https://localhost:7100';
let cancelTargetId = null;

async function loadBookings() {
    const token = TokenStore.get();
    if (!token) {
        document.getElementById('loadingSpinner').classList.add('d-none');
        showAlert('alertBox',
            '<i class="bi bi-key me-2"></i>Please <a href="/Auth/Token">get a JWT token</a> to view your bookings.',
            'warning');
        return;
    }

    try {
        const res = await fetch(`${API_BASE}/api/bookings`, {
            headers: TokenStore.headers()
        });

        if (res.status === 401) {
            document.getElementById('loadingSpinner').classList.add('d-none');
            showAlert('alertBox', '<i class="bi bi-shield-lock me-1"></i>Token expired or invalid. <a href="/Auth/Token">Get a new token.</a>', 'warning');
            return;
        }

        const bookings = await res.json();
        renderBookings(bookings);
    } catch (err) {
        document.getElementById('loadingSpinner').classList.add('d-none');
        showAlert('alertBox', `<i class="bi bi-exclamation-triangle me-1"></i>Failed to load bookings: ${err.message}`, 'danger');
    }
}

function renderBookings(bookings) {
    document.getElementById('loadingSpinner').classList.add('d-none');

    if (bookings.length === 0) {
        document.getElementById('emptyState').classList.remove('d-none');
        return;
    }

    document.getElementById('bookingsContainer').classList.remove('d-none');
    const tbody = document.getElementById('bookingsTableBody');

    tbody.innerHTML = bookings.map(b => {
        const eventDate  = new Date(b.eventDate).toLocaleDateString('en-IN', { dateStyle: 'medium' });
        const bookedDate = new Date(b.bookedAt).toLocaleDateString('en-IN', { dateStyle: 'short' });
        const statusClass = b.status === 'Confirmed' ? 'status-confirmed' : 'status-cancelled';

        return `
        <tr>
          <td class="text-muted small">#${b.id}</td>
          <td class="fw-semibold">${escHtml(b.eventTitle)}</td>
          <td>${eventDate}</td>
          <td>${escHtml(b.eventLocation)}</td>
          <td class="text-center">
            <span class="badge bg-primary rounded-pill">${b.seatsBooked}</span>
          </td>
          <td class="text-muted small">${bookedDate}</td>
          <td class="text-center">
            <span class="badge ${statusClass} rounded-pill px-3">${b.status}</span>
          </td>
          <td class="text-center">
            ${b.status === 'Confirmed'
                ? `<button class="btn btn-sm btn-outline-danger" onclick="openCancelModal(${b.id})">
                    <i class="bi bi-x-lg"></i> Cancel
                   </button>`
                : '<span class="text-muted">—</span>'}
          </td>
        </tr>`;
    }).join('');
}

// ── Cancel Booking ────────────────────────────────────────────────────────────
function openCancelModal(bookingId) {
    cancelTargetId = bookingId;
    document.getElementById('cancelBookingId').textContent = `#${bookingId}`;
    new bootstrap.Modal(document.getElementById('cancelModal')).show();
}

document.getElementById('confirmCancelBtn').addEventListener('click', async () => {
    if (!cancelTargetId) return;

    const btn = document.getElementById('confirmCancelBtn');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Cancelling…';

    try {
        const res = await fetch(`${API_BASE}/api/bookings/${cancelTargetId}`, {
            method:  'DELETE',
            headers: TokenStore.headers()
        });

        bootstrap.Modal.getInstance(document.getElementById('cancelModal')).hide();

        if (res.ok || res.status === 204) {
            showAlert('alertBox', `<i class="bi bi-check-circle me-1"></i>Booking #${cancelTargetId} cancelled successfully.`, 'success');
            // Reload table
            document.getElementById('bookingsContainer').classList.add('d-none');
            document.getElementById('loadingSpinner').classList.remove('d-none');
            await loadBookings();
        } else {
            const data = await res.json().catch(() => ({}));
            showAlert('alertBox', `<i class="bi bi-x-circle me-1"></i>${data.message || 'Cancellation failed.'}`, 'danger');
        }
    } catch (err) {
        showAlert('alertBox', `<i class="bi bi-exclamation-triangle me-1"></i>${err.message}`, 'danger');
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="bi bi-x-circle me-1"></i>Yes, Cancel';
        cancelTargetId = null;
    }
});

function escHtml(str) {
    return String(str)
        .replace(/&/g, '&amp;')
        .replace(/</g, '&lt;')
        .replace(/>/g, '&gt;');
}

loadBookings();
