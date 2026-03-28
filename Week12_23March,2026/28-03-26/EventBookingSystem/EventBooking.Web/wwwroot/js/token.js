/**
 * token.js – Calls POST /api/token and stores JWT in sessionStorage.
 */

const API_BASE = 'https://localhost:7100';

document.getElementById('getTokenBtn').addEventListener('click', async () => {
    const userIdEl   = document.getElementById('userId');
    const userNameEl = document.getElementById('userName');
    let valid = true;

    // Client-side validation
    [userIdEl, userNameEl].forEach(el => el.classList.remove('is-invalid'));

    if (!userIdEl.value.trim()) {
        userIdEl.classList.add('is-invalid');
        valid = false;
    }
    if (!userNameEl.value.trim()) {
        userNameEl.classList.add('is-invalid');
        valid = false;
    }
    if (!valid) return;

    const btn = document.getElementById('getTokenBtn');
    btn.disabled = true;
    btn.innerHTML = '<span class="spinner-border spinner-border-sm me-1"></span>Generating…';

    try {
        const res = await fetch(`${API_BASE}/api/token`, {
            method:  'POST',
            headers: { 'Content-Type': 'application/json' },
            body:    JSON.stringify({
                userId:   userIdEl.value.trim(),
                userName: userNameEl.value.trim()
            })
        });

        if (!res.ok) throw new Error(`API error ${res.status}`);

        const data = await res.json();
        TokenStore.set(data.token);

        document.getElementById('tokenOutput').textContent = data.token;
        document.getElementById('tokenSection').classList.remove('d-none');
        showAlert('alertBox',
            `<i class="bi bi-check-circle me-1"></i>Token generated! Expires: ${new Date(data.expires).toLocaleString()}`,
            'success');

    } catch (err) {
        showAlert('alertBox', `<i class="bi bi-x-circle me-1"></i>Failed: ${err.message}`, 'danger');
    } finally {
        btn.disabled = false;
        btn.innerHTML = '<i class="bi bi-key me-1"></i>Generate &amp; Save Token';
    }
});

document.getElementById('copyBtn').addEventListener('click', () => {
    const token = TokenStore.get();
    if (!token) return;
    navigator.clipboard.writeText(token).then(() => {
        const btn = document.getElementById('copyBtn');
        btn.innerHTML = '<i class="bi bi-check me-1"></i>Copied!';
        setTimeout(() => btn.innerHTML = '<i class="bi bi-clipboard me-1"></i>Copy Token', 2000);
    });
});

// If a token is already saved, show it
const existing = TokenStore.get();
if (existing) {
    document.getElementById('tokenOutput').textContent = existing;
    document.getElementById('tokenSection').classList.remove('d-none');
}
