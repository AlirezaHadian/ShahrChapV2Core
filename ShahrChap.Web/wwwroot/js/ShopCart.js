$(document).ready(function () {
    const dModal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));

    // ۱. انتخاب آدرس (کد اصلی خودت)
    $('.address-card').on('click', function () {
        $('.address-card').removeClass('active');
        $(this).addClass('active');
        $(this).find('input[type="radio"]').prop('checked', true);
    });


    // ۳. اعمال کد تخفیف
    $('#apply-discount').on('click', function () {
        const code = $('#discount-code').val();
        if (code === "WELCOME") {
            alert("کد تخفیف ۱۰ درصدی اعمال شد!");
        } else {
            alert("کد تخفیف معتبر نیست.");
        }
    });

    // ۴. دکمه پرداخت
    $('#pay-btn').on('click', function () {

        const hasAddress = $(this).data('has-address');

        if (!hasAddress) {
            Swal.fire({
                icon: 'warning',
                title: 'آدرس ارسال ثبت نشده',
                text: 'لطفا ابتدا آدرس ارسال خود را ثبت کنید.',
                confirmButtonText: 'متوجه شدم'
            });

            return;
        }

        alert("در حال هدایت به درگاه ایمن بانک...");
    });
});

//$(document).ready(function () {
//    // باز کردن مودال حذف
//    $(document).on('click', '.open-delete-modal', function () {
//        const orderId = $(this).data('id');
//        $('#final-delete-link').attr('href', '/Cart/Delete/' + orderId);

//        // نمایش مودال بوت استرپ
//        var myModal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
//        myModal.show();
//    });
//});

$(document).on('click', '.open-delete-modal', function () {
    const cartItemId = $(this).data('id');

    $('#delete-cart-item-id').val(cartItemId);

    const modal = new bootstrap.Modal(
        document.getElementById('deleteConfirmModal')
    );

    modal.show();
});
$('#final-delete-btn').on('click', function () {

    const cartItemId = $('#delete-cart-item-id').val();

    $.ajax({
        url: '/Cart/DeleteCartItem',
        type: 'POST',
        data: {
            cartItemId: cartItemId
        },
        success: function (res) {

            if (!res.success) {
                Swal.fire(
                    'خطا',
                    res.message,
                    'error'
                );
                return;
            }

            bootstrap.Modal
                .getInstance(
                    document.getElementById('deleteConfirmModal')
                )
                .hide();

            //----------------------------------
            // Cart Page
            //----------------------------------

            $('.cart-item[data-id="' + cartItemId + '"]')
                .fadeOut(300, function () {
                    $(this).remove();
                });

            //----------------------------------
            // Counter
            //----------------------------------

            $('.cart-count')
                .text(res.totalItems);

            //----------------------------------
            // Total Price
            //----------------------------------

            $('.cart-total-price')
                .text(res.totalPrice.toLocaleString());

            Swal.fire({
                icon: 'success',
                title: 'حذف شد',
                timer: 1500,
                showConfirmButton: false
            });

            //----------------------------------
            // اگر آخرین آیتم بود
            //----------------------------------

            if (res.totalItems === 0) {
                location.reload();
            }
        }
    });
});


$(document).ready(function () {
    // تابعی برای هماهنگ‌سازی کلاس active با رادیوباتن انتخاب شده
    function updateAddressUI() {
        $('.address-card').each(function () {
            const radio = $(this).find('input[type="radio"]');
            if (radio.is(':checked')) {
                $(this).addClass('active');
            } else {
                $(this).removeClass('active');
            }
        });
    }

    // ۱. مدیریت کلیک کاربر
    $('.address-card').on('click', function () {
        $(this).find('input[type="radio"]').prop('checked', true);
        updateAddressUI();
    });

    // ۲. حل مشکل دکمه بک (Back Button)
    // رویداد pageshow زمانی اجرا می‌شود که صفحه از کش مرورگر برگردد
    window.addEventListener('pageshow', function (event) {
        updateAddressUI();
    });

    // ۳. اجرای اولیه برای زمانی که صفحه برای اولین بار لود می‌شود
    updateAddressUI();
});


//$('.open-delete-modal').on('click', function () {
//    const orderId = $(this).data('id');
//    // تنظیم لینک حذف نهایی
//    $('#final-delete-link').attr('href', '/Cart/Delete/' + orderId);
//    var myModal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
//    dModal.show();
//});