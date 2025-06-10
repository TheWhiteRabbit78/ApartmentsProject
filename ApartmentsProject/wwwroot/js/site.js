// Site-wide JavaScript
(function () {
    'use strict';

    // Initialize on DOM ready
    document.addEventListener('DOMContentLoaded', function () {
        initializeNavbar();
        initializeAnimations();
        initializeForms();
        initializeGallery();
        initializeTooltips();
        initializeAdminModals();
        setCurrentYear();
    });

    // Navbar scroll effect
    function initializeNavbar() {
        const navbar = document.querySelector('.navbar');
        if (!navbar) return;

        window.addEventListener('scroll', function () {
            if (window.scrollY > 50) {
                navbar.classList.add('navbar-scrolled');
            } else {
                navbar.classList.remove('navbar-scrolled');
            }
        });

        // Handle hash link navigation
        document.querySelectorAll('.nav-link[href*="#"], .scroll-link[href*="#"]').forEach(link => {
            link.addEventListener('click', function (e) {
                const href = this.getAttribute('href');
                const hashIndex = href.indexOf('#');

                if (hashIndex !== -1) {
                    let targetId = href.substring(hashIndex);
                    targetId = targetId.replace(/['"\s]/g, '');
                    const targetSection = document.querySelector(targetId);

                    // Only prevent default if target exists on current page
                    if (targetSection) {
                        e.preventDefault();
                        const navbar = document.querySelector('.navbar');
                        const navbarHeight = navbar ? navbar.offsetHeight : 0;
                        const targetPosition = targetSection.offsetTop - navbarHeight + 20;

                        window.scrollTo({
                            top: targetPosition,
                            behavior: 'smooth'
                        });

                        // Close mobile menu
                        const navbarCollapse = document.querySelector('.navbar-collapse');
                        if (navbarCollapse && navbarCollapse.classList.contains('show')) {
                            const bsCollapse = new bootstrap.Collapse(navbarCollapse);
                            bsCollapse.hide();
                        }
                    }
                    // If target doesn't exist, let browser handle normal navigation
                }
            });
        });

        // Handle scrolling after page load when coming from a different page with hash
        if (window.location.hash) {
            setTimeout(() => {
                const targetId = window.location.hash.replace(/['"\s]/g, '');
                const targetSection = document.querySelector(targetId);

                if (targetSection) {
                    const navbar = document.querySelector('.navbar');
                    const navbarHeight = navbar ? navbar.offsetHeight : 0;
                    const targetPosition = targetSection.offsetTop - navbarHeight + 20;

                    window.scrollTo({
                        top: targetPosition,
                        behavior: 'smooth'
                    });
                }
            }, 100); // Small delay to ensure page is fully rendered
        }
    }

    // Scroll animations
    function initializeAnimations() {
        const observerOptions = {
            threshold: 0.1,
            rootMargin: '0px 0px -50px 0px'
        };

        const observer = new IntersectionObserver(function (entries) {
            entries.forEach(entry => {
                if (entry.isIntersecting) {
                    entry.target.classList.add('animate-in');
                    observer.unobserve(entry.target);
                }
            });
        }, observerOptions);

        // Add animation classes to elements
        document.querySelectorAll('.apartment-card').forEach((el, index) => {
            el.style.animationDelay = `${index * 0.1}s`;
            el.classList.add('animate-fade-up');
            observer.observe(el);
        });

        document.querySelectorAll('.feature-card').forEach((el, index) => {
            el.style.animationDelay = `${index * 0.1}s`;
            el.classList.add('animate-fade-up');
            observer.observe(el);
        });
    }

    // Form handling
    function initializeForms() {
        // Contact form
        const contactForm = document.getElementById('contactForm');
        if (contactForm) {
            contactForm.addEventListener('submit', handleContactForm);
        }

        // Apartment interest buttons
        document.querySelectorAll('.contact-button').forEach(button => {
            button.addEventListener('click', handleApartmentInterest);
        });
    }

    // Contact form submission
    function handleContactForm(e) {
        e.preventDefault();

        const form = e.target;
        const submitBtn = form.querySelector('#submitBtn');
        const submitText = submitBtn.querySelector('.submit-text');
        const submitLoading = submitBtn.querySelector('.submit-loading');
        const messageDiv = document.getElementById('contactMessage');

        // Show loading state
        submitBtn.disabled = true;
        submitText.classList.add('d-none');
        submitLoading.classList.remove('d-none');

        // Get form data
        const formData = new FormData(form);
        const data = {
            fullName: formData.get('fullName'),
            email: formData.get('email'),
            phone: formData.get('phone'),
            message: formData.get('message'),
            apartmentId: formData.get('apartmentId') || null
        };

        // Add CSRF token
        const token = form.querySelector('input[name="__RequestVerificationToken"]').value;

        fetch('/Home/SubmitContact', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json',
                'RequestVerificationToken': token
            },
            body: JSON.stringify(data)
        })
            .then(response => response.json())
            .then(result => {
                if (result.success) {
                    showMessage(messageDiv, 'success', result.message);
                    form.reset();
                    document.getElementById('apartmentId').value = '';
                } else {
                    showMessage(messageDiv, 'danger', result.message);
                }
            })
            .catch(error => {
                showMessage(messageDiv, 'danger', 'Došlo je do greške. Molimo pokušajte ponovo.');
            })
            .finally(() => {
                submitBtn.disabled = false;
                submitText.classList.remove('d-none');
                submitLoading.classList.add('d-none');
            });
    }

    // Handle apartment interest
    function handleApartmentInterest(e) {
        const button = e.currentTarget;
        const apartmentId = button.dataset.apartmentId;
        const apartmentTitle = button.dataset.apartmentTitle;

        // Close modal if open
        const modal = button.closest('.modal');
        if (modal) {
            const bsModal = bootstrap.Modal.getInstance(modal);
            if (bsModal) bsModal.hide();
        }

        // Scroll to contact form
        setTimeout(() => {
            const contactSection = document.getElementById('contact');
            if (contactSection) {
                contactSection.scrollIntoView({ behavior: 'smooth' });
            }

            // Fill apartment info
            const apartmentIdInput = document.getElementById('apartmentId');
            const messageTextarea = document.getElementById('message');

            if (apartmentIdInput) apartmentIdInput.value = apartmentId;

            if (messageTextarea) {
                const currentMsg = messageTextarea.value.trim();
                const newMsg = `Zanima me stan: ${apartmentTitle}`;

                if (!currentMsg) {
                    messageTextarea.value = newMsg;
                } else if (!currentMsg.includes(apartmentTitle)) {
                    messageTextarea.value = `${currentMsg}\n\n${newMsg}`;
                }

                messageTextarea.focus();
            }
        }, 300);
    }

    // Gallery functionality for apartment details
    function initializeGallery() {
        const galleryMain = document.querySelector('.gallery-main img');
        const galleryThumbs = document.querySelectorAll('.gallery-thumb');

        if (!galleryMain || !galleryThumbs.length) return;

        galleryThumbs.forEach(thumb => {
            thumb.addEventListener('click', function () {
                const imgSrc = this.querySelector('img').src;
                galleryMain.src = imgSrc;

                // Update active state
                galleryThumbs.forEach(t => t.classList.remove('active'));
                this.classList.add('active');
            });
        });
    }

    // Initialize tooltips
    function initializeTooltips() {
        const tooltips = document.querySelectorAll('[data-bs-toggle="tooltip"]');
        tooltips.forEach(tooltip => {
            new bootstrap.Tooltip(tooltip);
        });
    }

    // Set current year in footer
    function setCurrentYear() {
        const yearElement = document.getElementById('currentYear');
        if (yearElement) {
            yearElement.textContent = new Date().getFullYear();
        }
    }

    // Show message helper
    function showMessage(container, type, message) {
        container.innerHTML = `
            <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                ${message}
                <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
            </div>
        `;
    }

    // Admin modal functionality
    function initializeAdminModals() {
        // Only initialize if we're on an admin page or if jQuery is available
        if (typeof $ === 'undefined') return;

        // Modal form submission handler
        $(document).on('submit', '.modal-form', function (e) {
            e.preventDefault();
            const form = $(this);
            const formData = new FormData(this);

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
                }
            });
        });
    }

    // Admin modal functions (attach to window object so they're globally accessible)
    window.openApartmentModal = function (id = 0) {
        if (typeof $ === 'undefined') return;

        // Get URLs from data attributes or global variables
        const getApartmentModalUrl = document.body.dataset.getApartmentModalUrl || '/Admin/GetApartmentModal';

        $.get(getApartmentModalUrl, { id: id }, function (data) {
            $('#modal-content').html(data);
            $('#genericModal').modal('show');
        });
    };

    window.openDeleteApartmentModal = function (id) {
        if (typeof $ === 'undefined') return;

        // Get URLs from data attributes or global variables
        const getDeleteApartmentModalUrl = document.body.dataset.getDeleteApartmentModalUrl || '/Admin/GetDeleteApartmentModal';

        $.get(getDeleteApartmentModalUrl, { id: id }, function (data) {
            $('#modal-content').html(data);
            $('#genericModal').modal('show');
        });
    };

    // Show notification helper (updated to work with both jQuery and vanilla JS)
    function showNotification(message, type) {
        // Try jQuery first
        if (typeof $ !== 'undefined' && $('#notification-area').length) {
            const alert = `<div class="alert alert-${type} alert-dismissible fade show" role="alert">
                            ${message}
                            <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                          </div>`;
            $('#notification-area').html(alert);
            setTimeout(() => $('.alert').fadeOut(), 5000);
        } else {
            // Fallback to vanilla JS version
            const notificationArea = document.getElementById('notification-area');
            if (notificationArea) {
                notificationArea.innerHTML = `
                    <div class="alert alert-${type} alert-dismissible fade show" role="alert">
                        ${message}
                        <button type="button" class="btn-close" data-bs-dismiss="alert"></button>
                    </div>
                `;
                setTimeout(() => {
                    const alert = notificationArea.querySelector('.alert');
                    if (alert) alert.remove();
                }, 5000);
            }
        }
    }

    // Make showNotification globally accessible
    window.showNotification = showNotification;

    // Add CSS animation classes
    const style = document.createElement('style');
    style.textContent = `
        .animate-fade-up {
            opacity: 0;
            transform: translateY(30px);
            transition: opacity 0.6s ease, transform 0.6s ease;
        }
        
        .animate-fade-up.animate-in {
            opacity: 1;
            transform: translateY(0);
        }
    `;
    document.head.appendChild(style);

})();