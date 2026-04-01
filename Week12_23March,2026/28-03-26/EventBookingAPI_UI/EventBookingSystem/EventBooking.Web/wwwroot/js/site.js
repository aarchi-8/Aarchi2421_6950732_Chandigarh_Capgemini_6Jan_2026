/**
 * EventHub – shared client-side utilities
 */

const TokenStore = {
    KEY: 'eb_jwt_token',
    get()    { return sessionStorage.getItem(this.KEY) || ''; },
    set(t)   { sessionStorage.setItem(this.KEY, t); },
    clear()  { sessionStorage.removeItem(this.KEY); },
    headers() {
        const t = this.get();
        return t ? { 'Authorization': `Bearer ${t}`, 'Content-Type': 'application/json' }
                 : { 'Content-Type': 'application/json' };
    }
};

/** Convenience: show a Bootstrap alert inside a container element */
function showAlert(containerId, message, type = 'danger') {
    const el = document.getElementById(containerId);
    if (!el) return;
    el.innerHTML = `
      <div class="alert alert-${type} alert-dismissible fade show" role="alert">
        ${message}
        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
      </div>`;
}
