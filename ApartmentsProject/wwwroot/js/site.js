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

    // LIGHTBOX FUNCTIONALITY
    let currentApartmentImages = [];
    let currentImageIndex = 0;

    // Open lightbox when clicking on carousel images
    $(document).on('click', '.lightbox-trigger', function (e) {
        e.preventDefault();
        e.stopPropagation();

        const apartmentId = $(this).data('apartment-id');
        const imageIndex = $(this).data('image-index');
        const title = $(this).data('title');

        // Get all images for this apartment
        currentApartmentImages = [];
        $(`.lightbox-trigger[data-apartment-id="${apartmentId}"]`).each(function () {
            currentApartmentImages.push({
                src: $(this).attr('src'),
                caption: $(this).data('caption') || '',
                type: $(this).data('type') || '',
                title: $(this).data('title')
            });
        });

        currentImageIndex = imageIndex;
        openLightbox();
    });

    function openLightbox() {
        if (currentApartmentImages.length === 0) return;

        const currentImage = currentApartmentImages[currentImageIndex];

        $('#lightbox-image').attr('src', currentImage.src);
        $('#lightbox-title').text(currentImage.title);
        $('#lightbox-caption').text(`${currentImage.type}${currentImage.caption ? ' - ' + currentImage.caption : ''}`);

        // Show/hide navigation arrows
        if (currentApartmentImages.length > 1) {
            $('#lightbox-prev, #lightbox-next').show();
        } else {
            $('#lightbox-prev, #lightbox-next').hide();
        }

        $('#lightbox').addClass('active');
        $('body').addClass('lightbox-open');

        // Prevent body scroll
        $('body').css('overflow', 'hidden');
    }

    function closeLightbox() {
        $('#lightbox').removeClass('active');
        $('body').removeClass('lightbox-open');
        $('body').css('overflow', 'auto');

        // Reset zoom
        $('#lightbox-image').removeClass('zoomed');
    }

    // Close lightbox
    $('#lightbox-close').click(closeLightbox);

    // Close lightbox when clicking outside image
    $('#lightbox').click(function (e) {
        if (e.target === this) {
            closeLightbox();
        }
    });

    // Navigation
    $('#lightbox-prev').click(function () {
        currentImageIndex = (currentImageIndex - 1 + currentApartmentImages.length) % currentApartmentImages.length;
        updateLightboxImage();
    });

    $('#lightbox-next').click(function () {
        currentImageIndex = (currentImageIndex + 1) % currentApartmentImages.length;
        updateLightboxImage();
    });

    function updateLightboxImage() {
        const currentImage = currentApartmentImages[currentImageIndex];
        $('#lightbox-image').attr('src', currentImage.src);
        $('#lightbox-title').text(currentImage.title);
        $('#lightbox-caption').text(`${currentImage.type}${currentImage.caption ? ' - ' + currentImage.caption : ''}`);

        // Reset zoom when changing images
        $('#lightbox-image').removeClass('zoomed');
    }

    // Zoom functionality
    $('#lightbox-image').click(function () {
        $(this).toggleClass('zoomed');
    });

    // Keyboard navigation
    $(document).keydown(function (e) {
        if (!$('#lightbox').hasClass('active')) return;

        switch (e.which) {
            case 27: // ESC
                closeLightbox();
                break;
            case 37: // Left arrow
                if (currentApartmentImages.length > 1) {
                    $('#lightbox-prev').click();
                }
                break;
            case 39: // Right arrow
                if (currentApartmentImages.length > 1) {
                    $('#lightbox-next').click();
                }
                break;
        }
    });

}); // End document ready