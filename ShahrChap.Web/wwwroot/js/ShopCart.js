$(document).ready(function () {
    const dModal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));

    // ۱. انتخاب آدرس (کد اصلی خودت)
    $('.address-card').on('click', function () {
        $('.address-card').removeClass('active');
        $(this).addClass('active');
        $(this).find('input[type="radio"]').prop('checked', true);
    });

    //$('.open-delete-modal').on('click', function () {
    //    const orderId = $(this).data('id');
    //    // تنظیم لینک حذف نهایی
    //    $('#final-delete-link').attr('href', '/Cart/Delete/' + orderId);
    //    var myModal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
    //    dModal.show();
    //});

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
        alert("در حال هدایت به درگاه ایمن بانک...");
    });
});

$(document).ready(function () {
    // باز کردن مودال حذف
    $(document).on('click', '.open-delete-modal', function () {
        const orderId = $(this).data('id');
        $('#final-delete-link').attr('href', '/Cart/Delete/' + orderId);

        // نمایش مودال بوت استرپ
        var myModal = new bootstrap.Modal(document.getElementById('deleteConfirmModal'));
        myModal.show();
    });
});