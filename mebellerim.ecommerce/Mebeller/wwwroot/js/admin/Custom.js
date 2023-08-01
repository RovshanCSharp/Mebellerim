$(function () {

    const body = $('body');
    const btnDarkMode = $('.btn-dark-mode');
    const btnRTL = $('.btn-rtl');
    const darkClass = 'dark';
    const btnFullScreen = $('.btnFullScreen');
    let isFullScreen = false;

    btnFullScreen.on('click', function () {
        if (!isFullScreen) {
            const elem = document.documentElement;
            if (elem.requestFullscreen) {
                elem.requestFullscreen();
            }
            isFullScreen = true;
        } else {
            if (document.exitFullscreen) {
                document.exitFullscreen();
            }
            isFullScreen = false;
        }
    });

    btnDarkMode.on("click", function () {
        if (body.hasClass(darkClass)) {
            body.removeClass(darkClass);
            $('#theme-dark').remove();
        } else {
            body.addClass(darkClass);
        }
        btnDarkMode.toggleClass(darkClass);
        return false;
    });

    btnRTL.on('click', function () {
        if (body.hasClass('rtl')) {
            body.removeClass('rtl');
        } else {
            body.addClass('rtl');
        }
        $("html").attr("dir", body.hasClass('rtl') ? 'rtl' : '');
        btnRTL.toggleClass('rtl');
    });


    // Toggle mobile menu
    $('.mobile-toggle').on('click', function () {
        $('.nav-menus').toggleClass('open');
    });

    // Cache the jQuery selector
    const $mobileSearch = $('.mobile-search');
    const $formControl = $('.form-control-plaintext');

    // Toggle mobile search
    $mobileSearch.on('click', function () {
        $formControl.toggleClass('open');
    });

    $formControl.on('keyup', () => {
        const {value} = $(this);
        $('body').toggleClass('offcanvas', value !== '');
    });

    // Add ripple effect to elements
    $('.js-ripple').on('click', function (e) {
        const $ripple = $(this);
        const $offset = $ripple.parent().offset();
        const $circle = $ripple.find('.c-ripple__circle');
        const x = e.pageX - $offset.left;
        const y = e.pageY - $offset.top;
        $circle.css({top: `${y}px`, left: `${x}px`});
        $ripple.addClass('is-active').on('animationend webkitAnimationEnd oanimationend MSAnimationEnd', function () {
            $ripple.removeClass('is-active');
        });
    });

    $('.bg-img').each(function () {
        const $img = $(this);
        $img.parent().css({
            'background': 'url(' + $img.prop('src') + ') center / cover no-repeat',
            'display': 'block'
        });
        $img.hide();
    });
});
