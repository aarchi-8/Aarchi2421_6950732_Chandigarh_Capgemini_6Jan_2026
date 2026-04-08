// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

window.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('form').forEach(form => {
        if (form.getAttribute('asp-action') === 'Delete' || form.action.toLowerCase().includes('/delete')) {
            form.addEventListener('submit', event => {
                if (!confirm('Are you sure you want to delete this record?')) {
                    event.preventDefault();
                }
            });
        }
    });

    const feeInput = document.querySelector('input[name="ConsultationFee"]');
    const medicineInput = document.querySelector('input[name="MedicineCharges"]');
    const totalField = document.querySelector('#TotalAmountDisplay');

    if (feeInput && medicineInput && totalField) {
        const updateTotal = () => {
            const fee = parseFloat(feeInput.value) || 0;
            const medicine = parseFloat(medicineInput.value) || 0;
            totalField.textContent = (fee + medicine).toFixed(2);
        };

        feeInput.addEventListener('input', updateTotal);
        medicineInput.addEventListener('input', updateTotal);
        updateTotal();
    }
});
