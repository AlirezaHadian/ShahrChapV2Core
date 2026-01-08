/*============ SINGLE PRODUCT IMAGE HOVER ===========*/
document.addEventListener('DOMContentLoaded', function () {
    const allHoverImages = document.querySelectorAll(".hover-container img");
    const imgContainer = document.querySelector(".img-container img");
    if (!allHoverImages.length || !imgContainer) return;

    allHoverImages[0].closest(".img-box").classList.add("active");
    allHoverImages.forEach((image) => {
        image.addEventListener("mouseover", () => {
            imgContainer.src = image.src;
            document.querySelectorAll(".hover-container .img-box").forEach(box => box.classList.remove("active"));
            image.closest(".img-box").classList.add("active");
        });
    });
});

/* ========= PRODUCT CORE LOGIC ========= */
$(document).ready(function () {
    const uploadModal = new bootstrap.Modal(document.getElementById('uploadModal'));
    let selectedFiles = [];

    // ۱. محاسبه قیمت
    $(document).on('change', '.form-select, .service-check', function () {
        calculateFinalPrice();
    });

    function calculateFinalPrice() {
        let allSelected = true;
        let formData = { productId: $('#SubProduct_ProductId').val(), options: {}, services: [] };

        $('.product-custom-config .form-select').each(function () {
            let val = $(this).val();
            if (!val) { allSelected = false; return false; }
            formData.options[$(this).attr('name')] = $(this).find('option:selected').text().trim();
        });

        if (!allSelected) return;

        $('.service-check:checked').each(function () {
            formData.services.push(parseInt($(this).val()));
        });

        $('#desktop-price, #mobile-price').css('opacity', '0.5');
        $.ajax({
            url: '/Product/CalculatePrice',
            type: 'POST',
            data: formData,
            success: function (res) {
                if (res.success) {
                    let formatted = new Intl.NumberFormat('fa-IR').format(res.price);
                    $('#desktop-price, #mobile-price').text(formatted);
                }
                $('#desktop-price, #mobile-price').css('opacity', '1');
            }
        });
    }

    // ۲. اعتبارسنجی و باز کردن مودال (بدون مسیج باکس)
    $(document).on('click', '.addcart-btn', function (e) {
        e.preventDefault();

        const isLoggedIn = $(this).data('is-logged-in');

        if (isLoggedIn === false) {
            const currentUrl = window.location.pathname + window.location.search;
            window.location.href = "/Login?ReturnUrl=" + encodeURIComponent(currentUrl);
            return;
        }
        if (validateForm()) {
            uploadModal.show();
        }
    });

    // ۳. مدیریت فایل‌ها
    $('#order-file').on('change', function (e) {
        const allowed = ['rar', 'zip', 'png', 'jpeg', 'jpg', 'psd', 'tiff'];
        const files = Array.from(e.target.files);

        files.forEach(file => {
            const ext = file.name.split('.').pop().toLowerCase();
            if (allowed.includes(ext)) {
                if (!selectedFiles.some(f => f.name === file.name)) {
                    selectedFiles.push(file);
                    $('#file-list').append(`
                        <div class="file-item d-flex justify-content-between align-items-center bg-light p-2 mt-2 rounded" data-filename="${file.name}">
                            <span><i class="uil uil-file-check-alt text-success"></i> ${file.name}</span>
                            <i class="uil uil-times-circle text-danger remove-file-btn" style="cursor:pointer;"></i>
                        </div>
                    `);
                }
            } else {
                showToast(`فرمت فایل ${file.name} مجاز نیست.`, "error");
            }
        });
        updateSubmitBtn();
        $(this).val('');
    });

    $(document).on('click', '.remove-file-btn', function () {
        const item = $(this).closest('.file-item');
        selectedFiles = selectedFiles.filter(f => f.name !== item.data('filename'));
        item.fadeOut(300, function () { $(this).remove(); updateSubmitBtn(); });
    });

    function updateSubmitBtn() {
        $('#final-submit-btn').prop('disabled', selectedFiles.length === 0);
    }

    // ۴. ارسال نهایی
    $('#final-submit-btn').on('click', function () {
        let btn = $(this);
        let bar = $('#upload-progress');
        btn.prop('disabled', true).html('<span class="spinner-border spinner-border-sm"></span>');
        $('.progress').removeClass('d-none');

        let finalData = new FormData();
        finalData.append("ProductId", $('#SubProduct_ProductId').val());
        finalData.append("OrderTitle", $('#order-title').val());

        let combination = [];
        $('.product-custom-config .form-select').each(function () {
            combination.push($(this).find('option:selected').text().trim());
        });
        finalData.append("FeaturesCombination", combination.join(" - "));

        $('.service-check:checked').each(function (i) {
            finalData.append(`ServiceIds[${i}]`, $(this).val());
        });

        selectedFiles.forEach(file => finalData.append("OrderFiles", file));

        $.ajax({
            url: '/Product/SubmitFinalOrder',
            type: 'POST',
            data: finalData,
            processData: false,
            contentType: false,
            xhr: function () {
                var xhr = new window.XMLHttpRequest();
                xhr.upload.addEventListener("progress", function (evt) {
                    if (evt.lengthComputable) {
                        var percent = Math.round((evt.loaded / evt.total) * 100);
                        bar.css('width', percent + '%').text(percent + '%');
                    }
                }, false);
                return xhr;
            },
            success: function (res) {
                if (res.success) {
                    btn.text('موفقیت‌آمیز');
                    setTimeout(() => window.location.href = "/User/Orders", 1000);
                } else {
                    showToast(res.message, "error");
                    resetBtn(btn);
                }
            },
            error: function () {
                showToast("خطا در آپلود فایل‌ها", "error");
                resetBtn(btn);
            }
        });
    });

    function resetBtn(btn) {
        btn.prop('disabled', false).text('ثبت نهایی');
        $('.progress').addClass('d-none');
    }
});

/* ========= GLOBAL HELPERS ========= */

// اعتبارسنجی فقط با تغییر استایل فیلدها
function validateForm() {
    let isValid = true;
    let firstError = null;

    $('.product-custom-config .form-select, #order-title').each(function () {
        if (!$(this).val() || $(this).val().trim() === "" || $(this).prop('selectedIndex') === 0) {
            $(this).addClass('is-invalid');
            isValid = false;
            if (!firstError) firstError = $(this);
        } else {
            $(this).removeClass('is-invalid');
        }
    });

    if (!isValid && firstError) {
        firstError.focus();
    }
    return isValid;
}

// توستر مخصوص خطاهای فایل (رنگ قرمز)
function showToast(msg, type = "error") {
    toastr.options = {
        "progressBar": true,
        "positionClass": "toast-top-right",
        "rtl": true,
        "timeOut": "4000"
    };
    toastr[type](msg);
}