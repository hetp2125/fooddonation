// Food category emoji map
const foodEmojis = {
    'Hot Meals': '🍲', 'Baked Goods': '🍞', 'Fresh Produce': '🥦',
    'Pantry Items': '🥫', 'Grains': '🌾', 'Dairy': '🧀',
    'Protein': '🥩', 'Beverages': '🥤', 'Snacks': '🍎', 'Other': '🍱'
};

function getCategoryEmoji(category) {
    return foodEmojis[category] || '🍱';
}

// Auto-dismiss alerts
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('.alert').forEach(alert => {
        setTimeout(() => {
            const bsAlert = new bootstrap.Alert(alert);
            bsAlert.close();
        }, 5000);
    });

    // Confirm dialogs for destructive actions
    document.querySelectorAll('[data-confirm]').forEach(el => {
        el.addEventListener('click', (e) => {
            if (!confirm(el.dataset.confirm)) e.preventDefault();
        });
    });

    // Set min date for expiration date inputs
    document.querySelectorAll('input[type="date"]').forEach(input => {
        if (!input.min) {
            const tomorrow = new Date();
            tomorrow.setDate(tomorrow.getDate() + 1);
            input.min = tomorrow.toISOString().split('T')[0];
        }
    });

    // Quantity validation against available
    const qtyInput = document.getElementById('quantity-input');
    const maxQty = document.getElementById('max-quantity');
    if (qtyInput && maxQty) {
        qtyInput.max = maxQty.value;
        qtyInput.addEventListener('input', () => {
            const val = parseInt(qtyInput.value);
            const max = parseInt(maxQty.value);
            if (val > max) qtyInput.value = max;
            if (val < 1) qtyInput.value = 1;
        });
    }
});
