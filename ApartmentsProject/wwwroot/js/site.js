$(document).ready(function () {
    $('#currentYear').text(new Date().getFullYear());

    $('.scroll-link').on('click', function (e) {
        e.preventDefault();
        const target = $(this).attr('href');
        if ($(target).length) {
            $('html, body').animate({
                scrollTop: $(target).offset().top - 70
            }, 50);
        }
        $('.navbar-collapse').collapse('hide');
    });

    //Adds text to form for some UX bs
    $('.contact-button').on('click', function () {
        const apartmentId = $(this).data('apartment-id');
        const apartmentTitle = $(this).data('apartment-title');

        $(this).closest('.modal').modal('hide');

        setTimeout(() => {
            $('#apartmentId').val(apartmentId);
            const currentMsg = $('#message').val().trim();
            const newMsg = `Zanima me stan: ${apartmentTitle}`;

            if (!currentMsg) {
                $('#message').val(newMsg);
            } else if (!currentMsg.includes(apartmentTitle)) {
                $('#message').val(`${currentMsg}\n\n${newMsg}`);
            }

            $('html, body').animate({
                scrollTop: $('#contact').offset().top - 70
            }, 50);
            $('#fullName').focus();
        }, 300);
    });

    $('#contactForm').on('submit', function (e) {
        e.preventDefault();

        const $btn = $('#submitBtn');
        const $text = $('.submit-text');
        const $loading = $('.submit-loading');
        const $msg = $('#contactMessage');

        // Show loading state
        $btn.prop('disabled', true);
        $text.addClass('d-none');
        $loading.removeClass('d-none');
        $msg.empty();

        $.ajax({
            url: '/Home/SubmitContact',
            type: 'POST',
            data: {
                fullName: $('#fullName').val(),
                email: $('#email').val(),
                phone: $('#phone').val(),
                message: $('#message').val(),
                apartmentId: $('#apartmentId').val() || null
            },
            beforeSend: function (xhr) {
                xhr.setRequestHeader('RequestVerificationToken',
                    $('input[name="__RequestVerificationToken"]').val());
            },
            success: function (response) {
                if (response.success) {
                    $msg.html(`<div class="alert alert-success">${response.message}</div>`);
                    document.getElementById('contactForm').reset();
                    $('#apartmentId').val('');
                } else {
                    $msg.html(`<div class="alert alert-danger">${response.message}</div>`);
                }
            },
            error: function () {
                $msg.html(`<div class="alert alert-danger">
                  Došlo je do greške. Molimo pokušajte ponovo.</div>`);
            },
            complete: function () {
                $btn.prop('disabled', false);
                $text.removeClass('d-none');
                $loading.addClass('d-none');
            }
        });
    });

});