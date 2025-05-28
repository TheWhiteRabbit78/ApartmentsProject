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
        }, 800);
        $('.navbar-collapse').collapse('hide');
    });

    // Contact button in modal leads to contact section and sets apartment ID
    $('.contact-button').on('click', function () {
        const apartmentId = $(this).data('apartment-id');
        $('#apartmentId').val(apartmentId);

        $('html, body').animate({
            scrollTop: $('#contact').offset().top - 70
        }, 800);
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
            headers: {
                'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val()
            },
            success: function (response) {
                if (response.success) {
                    messageDiv.html(`<div class="alert alert-success">${response.message}</div>`);
                    document.getElementById('contactForm').reset();
                } else {
                    messageDiv.html(`<div class="alert alert-danger">${response.message}</div>`);
                }
            },
            error: function () {
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