document.addEventListener('DOMContentLoaded', function () {
    const forms = document.querySelectorAll('form');

    forms.forEach(form => {
        form.classList.add('needs-validation');
        form.setAttribute('novalidate', 'novalidate');

        form.addEventListener('submit', function (event) {
            if (!form.checkValidity()) {
                event.preventDefault();
                event.stopPropagation();
                form.classList.add('was-validated');
            } else {
                form.classList.add('was-validated');
            }
        }, false);
    });
});
