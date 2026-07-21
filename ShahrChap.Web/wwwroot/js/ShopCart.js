$(document).ready(function () {

    // ==============================
    // ۱. انتخاب آدرس ارسال
    // ==============================
    function updateAddressUI() {
        $('.address-card').each(function () {
            const radio = $(this).find('input[type="radio"]');
            $(this).toggleClass('active', radio.is(':checked'));
        });
    }

    $('.address-card').on('click', function () {
        $(this).find('input[type="radio"]').prop('checked', true);
        updateAddressUI();
    });

    // حل مشکل دکمه‌ی بک مرورگر (کش صفحه)
    window.addEventListener('pageshow', function () {
        updateAddressUI();
    });

    updateAddressUI(); // اجرای اولیه در لود صفحه


    // ==============================
    // ۲. اعمال کد تخفیف
    // ==============================
    $('#apply-discount').on('click', function () {
        const code = $('#discount-code').val();

        if (code === "WELCOME") {
            Swal.fire('موفق', 'کد تخفیف ۱۰ درصدی اعمال شد!', 'success');
        } else {
            Swal.fire('خطا', 'کد تخفیف معتبر نیست.', 'error');
        }
    });


    // ==============================
    // ۳. دکمه‌ی پرداخت نهایی (رفتن به Checkout)
    // ==============================
    // $('#pay-btn').on('click', function () {
    //     const hasAddress = $(this).data('has-address');

    //     if (!hasAddress) {
    //         Swal.fire({
    //             icon: 'warning',
    //             title: 'آدرس ارسال ثبت نشده',
    //             text: 'لطفا ابتدا آدرس ارسال خود را ثبت کنید.',
    //             confirmButtonText: 'متوجه شدم'
    //         });
    //         return;
    //     }

    //     window.location.href = '/Checkout';
    // });

});


// ==============================
// ۴. حذف آیتم از سبد خرید
// (delegated - هم برای صفحه‌ی سبد، هم پاپ‌اور کار می‌کند)
// ==============================

$(document).on('click', '.open-delete-modal', function () {
    const cartItemId = $(this).data('id');
    $('#delete-cart-item-id').val(cartItemId);

    const modal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
    modal.show();
});

$(document).on('click', '#final-delete-btn', function () {
    const cartItemId = $('#delete-cart-item-id').val();

    $.ajax({
        url: '/Cart/DeleteCartItem',
        type: 'POST',
        data: { cartItemId: cartItemId },
        success: function (res) {
            if (!res.success) {
                Swal.fire('خطا', res.message || 'حذف با مشکل مواجه شد', 'error');
                return;
            }

            bootstrap.Modal.getInstance(document.getElementById('deleteConfirmModal'))?.hide();

            // حذف کارت آیتم از هر جایی که نمایش داده شده (صفحه‌ی سبد یا پاپ‌اور)
            $('.cart-item[data-id="' + cartItemId + '"]').fadeOut(300, function () {
                $(this).remove();
            });

            // آپدیت شمارنده‌ی روی آیکون navbar
            $('#cart-badge').text(res.totalItems);
            if (res.totalItems === 0) {
                $('#cart-badge').hide();
            }

            // آپدیت شمارنده و مبلغ داخل پاپ‌اور
            $('.items-count-pill').text(res.totalItems + ' آیتم');
            $('#popover-total-price').text(res.totalPrice.toLocaleString());

            // آپدیت مبلغ کل داخل صفحه‌ی سبد خرید (در صورت وجود)
            $('#cart-total-price').text(res.totalPrice.toLocaleString());
            $('#total-price').text(res.totalPrice.toLocaleString());

            Swal.fire({ icon: 'success', title: 'حذف شد', timer: 1500, showConfirmButton: false });

            if (res.totalItems === 0 && window.location.pathname === '/Cart') {
                location.reload();
            }
        },
        error: function () {
            Swal.fire('خطا', 'ارتباط با سرور برقرار نشد', 'error');
        }
    });
});