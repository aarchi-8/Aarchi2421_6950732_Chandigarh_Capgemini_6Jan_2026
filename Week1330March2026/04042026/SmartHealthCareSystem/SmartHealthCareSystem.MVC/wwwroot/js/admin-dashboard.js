// Minimal JS for dashboard (placeholder for future interactivity)
document.addEventListener('DOMContentLoaded', function() {
    // Example: Animate summary cards
    document.querySelectorAll('.card span').forEach(function(el) {
        el.style.opacity = 0;
        setTimeout(function() {
            el.style.transition = 'opacity 1s';
            el.style.opacity = 1;
        }, 300);
    });
});
