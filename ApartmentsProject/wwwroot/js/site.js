// ==============================================
// APARTMENT LIGHTBOX CLASS
// ==============================================
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

// ==============================================
// UTILITY FUNCTIONS
// ==============================================
function showNotification(message, type = 'info') {
    const alert = `<div class="alert alert-${type} alert-dismissible fade show" role="alert">
                    ${message}
                    <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                  </div>`;
    $('#notification-area').html(alert);
    setTimeout(() => $('.alert').fadeOut(), 5000);
}

// ==============================================
// MODAL FUNCTIONS
// ==============================================
function openApartmentModal(id = 0) {
    $.get('/Admin/GetApartmentModal', { id: id }, function (data) {
        $('#modal-content').html(data);
        $('#genericModal').modal('show');
    });
}

function openApartmentDetailsModal(id) {
    $.get('/Admin/GetApartmentDetailsModal', { id: id }, function (data) {
        $('#modal-content').html(data);
        $('#genericModal').modal('show');
    });
}

function openDeleteApartmentModal(id) {
    $.get('/Admin/GetDeleteApartmentModal', { id: id }, function (data) {
        $('#modal-content').html(data);
        $('#genericModal').modal('show');
    });
}

function openAddImageModal(id) {
    $.get('/Admin/GetAddImageModal', { id: id }, function (data) {
        $('#modal-content').html(data);
        $('#genericModal').modal('show');
    });
}

function openCreateUserModal() {
    $.get('/Admin/GetCreateUserModal', function (data) {
        $('#modal-content').html(data);
        $('#genericModal').modal('show');
    });
}

function openDeleteUserModal(id) {
    $.get('/Admin/GetDeleteUserModal', { id: id }, function (data) {
        $('#modal-content').html(data);
        $('#genericModal').modal('show');
    });
}

function deleteImage(imageId, apartmentId) {
    if (confirm('Obrisati ovu sliku?')) {
        $.post('/Admin/DeleteImage', { id: imageId }, function (response) {
            if (response.isValid) {
                showNotification(response.message, 'success');
                setTimeout(() => openApartmentDetailsModal(apartmentId), 1000);
            } else {
                showNotification(response.message, 'danger');
            }
        });
    }
}

// ==============================================
// DOCUMENT READY
// ==============================================
$(document).ready(function () {
    // Initialize lightbox
    new ApartmentLightbox();

    // Set current year in footer
    $('#currentYear').text(new Date().getFullYear());

    // ==============================================
    // SMOOTH SCROLLING
    // ==============================================
    $('.scroll-link').on('click', function (e) {
        e.preventDefault();
        const target = $(this).attr('href');
        if ($(target).length) {
            $('html, body').animate({
                scrollTop: $(target).offset().top - 70
            }, 800);
        }
        $('.navbar-collapse').collapse('hide');
    });

    // ==============================================
    // CONTACT FORM HANDLING
    // ==============================================
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
            }, 800);
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

    // ==============================================
    // ADMIN CARD INTERACTIONS
    // ==============================================
    $('.admin-apartment-card').on('click', function (e) {
        // Don't trigger if clicking on buttons
        if ($(e.target).closest('.btn, .btn-group').length === 0) {
            const apartmentId = $(this).data('apartment-id');
            if (apartmentId) {
                openApartmentDetailsModal(apartmentId);
            }
        }
    });

    $('.admin-apartment-card .btn').on('click', function (e) {
        e.stopPropagation();
    });

    // ==============================================
    // MODAL FORM HANDLING
    // ==============================================
    $(document).on('submit', '.modal-form', function (e) {
        e.preventDefault();

        const form = $(this);
        const formData = new FormData(this);
        const submitBtn = form.find('button[type="submit"]');
        const originalText = submitBtn.html();

        // Show loading state
        submitBtn.prop('disabled', true)
            .html('<span class="spinner-border spinner-border-sm me-2"></span>Šalje...');

        $.ajax({
            type: form.attr('method'),
            url: form.attr('action'),
            data: formData,
            processData: false,
            contentType: false,
            success: function (response) {
                if (response.isValid) {
                    $('#genericModal').modal('hide');
                    showNotification(response.message, 'success');
                    setTimeout(() => location.reload(), 1000);
                } else {
                    $('#modal-content').html(response.html);
                }
            },
            error: function () {
                showNotification('Došlo je do greške. Molimo pokušajte ponovo.', 'danger');
            },
            complete: function () {
                submitBtn.prop('disabled', false).html(originalText);
            }
        });
    });

    // ==============================================
    // MODAL EVENT HANDLERS
    // ==============================================
    $(document).on('shown.bs.modal', '#genericModal', function () {
        // Parse validation for any forms in the modal
        $.validator.unobtrusive.parse('#genericModal');
        // Focus on first input
        $('#genericModal').find('input:first').focus();
    });

    $(document).on('hidden.bs.modal', '#genericModal', function () {
        $('#modal-content').empty();
    });

    // ==============================================
    // FORM VALIDATION ENHANCEMENT
    // ==============================================
    $(document).on('focus', '.form-control', function () {
        $(this).closest('.form-group, .mb-3').removeClass('has-error');
    });
});