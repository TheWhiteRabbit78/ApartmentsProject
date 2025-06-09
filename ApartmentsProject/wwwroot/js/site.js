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

        // Smooth scroll for navigation links
        //document.querySelectorAll('.nav-link .scroll-link').forEach(link => {
        //    link.addEventListener('click', function (e) {
        //        e.preventDefault();
        //        const targetId = this.getAttribute('href');
        //        const targetSection = document.querySelector(targetId);

        //        if (targetSection) {
        //            const navbarHeight = navbar.offsetHeight;
        //            const targetPosition = targetSection.offsetTop - navbarHeight - 20;
        //            //const targetPosition = targetSection.offsetTop + 150;

        //            window.scrollTo({
        //                top: targetPosition,
        //                behavior: 'smooth'
        //            });
        //        }

        //        // Close mobile menu if open
        //        const navbarCollapse = document.querySelector('.navbar-collapse');
        //        if (navbarCollapse && navbarCollapse.classList.contains('show')) {
        //            const bsCollapse = new bootstrap.Collapse(navbarCollapse);
        //            bsCollapse.hide();
        //        }
        //    });
        //});

        document.querySelectorAll('.nav-link[href^="#"] .scroll-link').forEach(link => {
            link.addEventListener('click', function (e) {
                e.preventDefault();
                const targetId = this.getAttribute('href');
                const targetSection = document.querySelector(targetId);

                if (targetSection) {
                    const navbar = document.querySelector('.navbar');
                    const navbarHeight = navbar ? navbar.offsetHeight : 0;
                    const targetPosition = targetSection.offsetTop - navbarHeight - 20;

                    window.scrollTo({
                        top: targetPosition,
                        behavior: 'smooth'
                    });
                }

                const navbarCollapse = document.querySelector('.navbar-collapse');
                if (navbarCollapse && navbarCollapse.classList.contains('show')) {
                    const bsCollapse = new bootstrap.Collapse(navbarCollapse);
                    bsCollapse.hide();
                }
            });
        });
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