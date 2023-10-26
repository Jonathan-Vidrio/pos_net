document.addEventListener('DOMContentLoaded', async function () {
    const confirmCancel = $('#confirmationModal');
    const supervisorTokenInput = $('#supervisorToken');
    const form = $('form');

    const handleKeydown = function(event) {
        if (event.key === 'Enter' && confirmCancel.hasClass('show')) {
            cancelSale();
        }
    };

    const cancelSale = () => {
        const token = supervisorTokenInput.val();
        if (token) {
            $.ajax({
                url: 'Sale/VerifySupervisorToken',
                type: 'POST',
                data: {token: token},
                success: function (response) {
                    if (response.isValid) {
                        form.append(`<input type="hidden" name="SupervisorToken" value="${token}">`);
                        form.submit();
                    } else {
                        alert(response.message);
                    }
                },
                error: function () {
                    alert('Error al verificar el token del supervisor.');
                }
            });
        } else {
            alert('Por favor, ingresa el token del supervisor.');
        }
    }

    $('.btn-primary').on('click', function(e) {
        e.preventDefault();
        confirmCancel.modal('show');
    });

    confirmCancel.on('shown.bs.modal', function() {
        supervisorTokenInput.focus();
        document.addEventListener('keydown', handleKeydown);
    });

    confirmCancel.on('hidden.bs.modal', function() {
        document.removeEventListener('keydown', handleKeydown);
    });

    confirmCancel.on('click', '.confirm-btn', cancelSale); // Asegúrate de que tu botón de confirmación en el modal tiene una clase 'confirm-btn'

    $('.btn-primary').on('click', function(e) {
        e.preventDefault();
        confirmCancel.modal('show');
    });
});



