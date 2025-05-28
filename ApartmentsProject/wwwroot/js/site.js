// ApartmentsProject JavaScript - Simplified

$(document).ready(function () {
    // Add current year to footer
    $('#currentYear').text(new Date().getFullYear());

    // Smooth scroll for navigation
    $('.scroll-link').on('click', function (e) {
        e.preventDefault();
        var target = $(this).attr('href');
        $('html, body').animate({
            scrollTop: $(target).offset().top - 70
        }, 50);
        $('.navbar-collapse').collapse('hide');
    });

    // Contact button in modal leads to contact section and sets apartment ID
    $('.contact-button').on('click', function () {
        const apartmentId = $(this).data('apartment-id');
        const apartmentTitle = $(this).closest('.modal').find('.modal-title').text();

        // Close the modal first
        $(this).closest('.modal').modal('hide');

        // Wait for modal to close, then scroll and populate form
        setTimeout(function () {
            // Set apartment ID
            $('#apartmentId').val(apartmentId);

            // Pre-fill message with apartment interest
            const currentMessage = $('#message').val();
            const apartmentMessage = `Zanima me stan: ${apartmentTitle}`;

            if (currentMessage.trim() === '') {
                $('#message').val(apartmentMessage);
            } else if (!currentMessage.includes(apartmentTitle)) {
                $('#message').val(currentMessage + '\n\n' + apartmentMessage);
            }

            // Scroll to contact form
            $('html, body').animate({
                scrollTop: $('#contact').offset().top - 70
            }, 800);

            // Optional: Focus on the name field
            $('#fullName').focus();
        }, 300); // Wait for modal close animation
    });

    // Handle contact form submission
    $('#contactForm').on('submit', function (e) {
        e.preventDefault();

        const submitBtn = $('#submitBtn');
        const submitText = $('.submit-text');
        const submitLoading = $('.submit-loading');
        const messageDiv = $('#contactMessage');

        // Show loading state
        submitBtn.prop('disabled', true);
        submitText.addClass('d-none');
        submitLoading.removeClass('d-none');
        messageDiv.empty();

        // Get form data
        const formData = {
            fullName: $("#fullName").val(),
            email: $("#email").val(),
            phone: $("#phone").val(),
            message: $("#message").val(),
            apartmentId: $("#apartmentId").val() || null
        };

        // Submit via AJAX
        $.ajax({
            url: '/Home/SubmitContact',
            type: 'POST',
            data: formData,
            beforeSend: function (xhr) {
                xhr.setRequestHeader('RequestVerificationToken',
                    $('input[name="__RequestVerificationToken"]').val());
            },
            success: function (response) {
                if (response.success) {
                    messageDiv.html(`<div class="alert alert-success">${response.message}</div>`);
                    document.getElementById('contactForm').reset();
                    $('#apartmentId').val(''); // Clear apartment ID too
                } else {
                    messageDiv.html(`<div class="alert alert-danger">${response.message}</div>`);
                }
            },
            error: function (xhr, status, error) {
                console.log('Error details:', xhr.responseText); // Debug line
                messageDiv.html('<div class="alert alert-danger">Došlo je do greške. Molimo pokušajte ponovo.</div>');
            },
            complete: function () {
                // Reset button state
                submitBtn.prop('disabled', false);
                submitText.removeClass('d-none');
                submitLoading.addClass('d-none');
            }
        });
    });
});