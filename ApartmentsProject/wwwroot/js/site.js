class ApartmentLightbox {
    constructor() {
        this.images = [];
        this.currentIndex = 0;
        this.init();
    }

    init() {
        this.createLightboxHTML();
        this.bindEvents();
    }

    createLightboxHTML() {
        if (document.getElementById('lightbox')) return;

        const lightbox = document.createElement('div');
        lightbox.id = 'lightbox';
        lightbox.className = 'lightbox';
        lightbox.innerHTML = `
            <div class="lightbox-content">
                <img id="lightbox-image" class="lightbox-image" src="" alt="">
                <button id="lightbox-close" class="lightbox-close">&times;</button>
                <button id="lightbox-prev" class="lightbox-nav lightbox-prev">
                    <i class="fas fa-chevron-left"></i>
                </button>
                <button id="lightbox-next" class="lightbox-nav lightbox-next">
                    <i class="fas fa-chevron-right"></i>
                </button>
                <div id="lightbox-info" class="lightbox-info">
                    <h5 id="lightbox-title"></h5>
                    <p id="lightbox-caption"></p>
                </div>
            </div>
        `;
        document.body.appendChild(lightbox);
    }

    bindEvents() {
        $(document).on('click', '.lightbox-trigger', (e) => this.open(e));
        $('#lightbox-close').on('click', () => this.close());
        $('#lightbox-prev').on('click', () => this.prev());
        $('#lightbox-next').on('click', () => this.next());
        $('#lightbox-image').on('click', () => this.toggleZoom());
        $('#lightbox').on('click', (e) => {
            if (e.target.id === 'lightbox') this.close();
        });
        $(document).on('keydown', (e) => this.handleKeyboard(e));
    }

    open(e) {
        e.preventDefault();
        e.stopPropagation();

        const $trigger = $(e.currentTarget);
        const apartmentId = $trigger.data('apartment-id');
        const imageIndex = $trigger.data('image-index');

        this.loadApartmentImages(apartmentId);
        this.currentIndex = imageIndex || 0;
        this.updateImage();
        this.show();
    }

    loadApartmentImages(apartmentId) {
        this.images = [];
        $(`.lightbox-trigger[data-apartment-id="${apartmentId}"]`).each((_, el) => {
            const $el = $(el);
            this.images.push({
                src: $el.attr('src'),
                caption: $el.data('caption') || '',
                type: $el.data('type') || '',
                title: $el.data('title')
            });
        });
    }

    updateImage() {
        if (this.images.length === 0) return;

        const img = this.images[this.currentIndex];
        $('#lightbox-image').attr('src', img.src).removeClass('zoomed');
        $('#lightbox-title').text(img.title);
        $('#lightbox-caption').text(
            `${img.type}${img.caption ? ' - ' + img.caption : ''}`
        );

        $('#lightbox-prev, #lightbox-next').toggle(this.images.length > 1);
    }

    show() {
        $('#lightbox').addClass('active');
        $('body').css('overflow', 'hidden');
    }

    close() {
        $('#lightbox').removeClass('active');
        $('body').css('overflow', 'auto');
        $('#lightbox-image').removeClass('zoomed');
    }

    prev() {
        this.currentIndex = (this.currentIndex - 1 + this.images.length) % this.images.length;
        this.updateImage();
    }

    next() {
        this.currentIndex = (this.currentIndex + 1) % this.images.length;
        this.updateImage();
    }

    toggleZoom() {
        $('#lightbox-image').toggleClass('zoomed');
    }

    handleKeyboard(e) {
        if (!$('#lightbox').hasClass('active')) return;

        switch (e.which) {
            case 27: this.close(); break;
            case 37: if (this.images.length > 1) this.prev(); break;
            case 39: if (this.images.length > 1) this.next(); break;
        }
    }
}

$(document).ready(function () {
    new ApartmentLightbox();

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