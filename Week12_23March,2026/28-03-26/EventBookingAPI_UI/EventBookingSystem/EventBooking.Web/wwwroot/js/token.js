// ── CONFIG ───────────────────────────────────────────
const API_BASE = 'https://localhost:7676';

// ── GENERATE TOKEN ───────────────────────────────────
document.getElementById('getTokenBtn').addEventListener('click', async () => {
    const userIdEl = document.getElementById('userId');
    const userNameEl = document.getElementById('userName');

    let valid = true;

    // Reset validation
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
    btn.innerHTML = `
        <span class="spinner-border spinner-border-sm me-2"></span>
        Generating...
    `;

    try {
        const res = await fetch(`${API_BASE}/api/token`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                userId: userIdEl.value.trim(),
                userName: userNameEl.value.trim()
            })
        });

        if (!res.ok) throw new Error(`API error ${res.status}`);

        const data = await res.json();

        // Save token
        TokenStore.set(data.token);

        // Show token section with animation
        const section = document.getElementById('tokenSection');
        document.getElementById('tokenOutput').textContent = data.token;
        section.classList.remove('d-none');
        section.classList.add('fade-in');

        // Success alert
        showAlert('alertBox',
            `<div class="d-flex align-items-center">
                <i class="bi bi-check-circle-fill text-success fs-5 me-2"></i>
                <div>
                    <strong>Token Generated Successfully!</strong><br>
                    <small>Expires: ${new Date(data.expires).toLocaleString()}</small>
                </div>
            </div>`,
            'success');

    } catch (err) {
        showAlert('alertBox',
            `<i class="bi bi-x-circle me-1"></i> ${err.message}`,
            'danger');
    } finally {
        btn.disabled = false;
        btn.innerHTML = `
            <i class="bi bi-shield-lock me-1"></i>Generate & Save Token
        `;
    }
});

// ── COPY TOKEN ───────────────────────────────────────
document.getElementById('copyBtn').addEventListener('click', () => {
    const token = TokenStore.get();
    if (!token) return;

    navigator.clipboard.writeText(token).then(() => {
        const btn = document.getElementById('copyBtn');

        btn.classList.remove('btn-outline-secondary');
        btn.classList.add('btn-success');

        btn.innerHTML = `<i class="bi bi-check me-1"></i>Copied`;

        setTimeout(() => {
            btn.classList.remove('btn-success');
            btn.classList.add('btn-outline-secondary');
            btn.innerHTML = `<i class="bi bi-clipboard me-1"></i>Copy`;
        }, 2000);
    });
});

// ── LOAD EXISTING TOKEN ──────────────────────────────
const existing = TokenStore.get();

if (existing) {
    const section = document.getElementById('tokenSection');

    document.getElementById('tokenOutput').textContent = existing;
    section.classList.remove('d-none');
    section.classList.add('fade-in');

    showAlert('alertBox',
        `<i class="bi bi-info-circle me-1"></i> Using saved token from session`,
        'info');
}