/*============SINGLE PRODUCT===========*/
document.addEventListener('DOMContentLoaded', function () {
    const allHoverImages = document.querySelectorAll(".hover-container img");
    const imgContainer = document.querySelector(".img-container img");

    if (!allHoverImages.length || !imgContainer) return;

    allHoverImages[0].closest(".img-box").classList.add("active");

    allHoverImages.forEach((image) => {
        image.addEventListener("mouseover", () => {

            imgContainer.src = image.src;

            document.querySelectorAll(".hover-container .img-box")
                .forEach(box => box.classList.remove("active"));

            image.closest(".img-box").classList.add("active");
        });
    });
});
/* =========PRICE CHANGE FUNCTION========= */
// تابع برای تبدیل اعداد فارسی/عربی به انگلیسی و حذف کاراکترهای اضافه
function cleanNumber(str) {
    if (!str) return 0;
    let p = ["۰", "۱", "۲", "۳", "۴", "۵", "۶", "۷", "۸", "۹"];
    let a = ["٠", "١", "٢", "٣", "٤", "٥", "٦", "٧", "٨", "٩"];
    let s = str.toString();
    for (let i = 0; i < 10; i++) {
        s = s.replace(new RegExp(p[i], 'g'), i).replace(new RegExp(a[i], 'g'), i);
    }
    // حذف هر چیزی که عدد نیست (مثل کاما و تومان)
    return parseFloat(s.replace(/[^0-9.]/g, '')) || 0;
}

// تابع برای فرمت کردن عدد به سه رقم سه رقم
function formatMoney(num) {
    return new Intl.NumberFormat('fa-IR').format(num);
}

//$(document).ready(function () {
//    $(document).on('change', '.form-select, .service-row input[type="checkbox"]', function () {
//        calculateDynamicPrice();
//    });

//    function calculateDynamicPrice() {
//        let allValid = true;
//        let formData = {
//            options: {},     
//            services: [],    
//            productId: $('#SubProduct_ProductId').val()
//        };

//        $('.form-select').each(function () {
//            let value = $(this).val();
//            if (!value || value === "" || $(this).find('option:selected').is(':disabled')) {
//                allValid = false;
//            } else {
//                let key = $(this).attr('name') || "param_" + $(this).index();
//                formData.options[key] = value;
//            }
//        });

//        $('.service-row input[type="checkbox"]:checked').each(function () {
//            formData.services.push($(this).val());
//        });

//        if (allValid) {
//            $('#desktop-price, #mobile-price').css('opacity', '0.5');

//            $.ajax({
//                url: '/Product/CalculatePrice',
//                type: 'POST',
//                data: formData,
//                success: function (res) {
//                    if (res.success) {
//                        let priceValue = cleanNumber(res.price);
//                        if (!isNaN(priceValue)) {
//                            let formatted = formatMoney(priceValue);

//                            // آپدیت همزمان در هر دو مکان
//                            $('#desktop-price').text(formatted);
//                            $('#mobile-price').text(formatted);
//                        }
//                    }
//                    $('#desktop-price, #mobile-price').css('opacity', '1');
//                },
//                error: function () {
//                    alert('خطا در محاسبه قیمت. لطفاً دوباره تلاش کنید.');
//                    $('#desktop-price, #mobile-price').css('opacity', '1');
//                }
//            });
//        }
//    }
//});



$(document).ready(function () {
    // استفاده از Delegation برای دراپ‌دان‌های داینامیک و چک‌باکس‌ها
    $(document).on('change', '.form-select, .service-check', function () {
        calculateFinalPrice();
    });

    function calculateFinalPrice() {
        let allDropdownsSelected = true;
        let formData = {
            productId: $('#SubProduct_ProductId').val(),
            options: {}, // در اینجا نام ویژگی (ValueTitle) ذخیره می‌شود
            services: [] // در اینجا آی‌دی سرویس‌ها ذخیره می‌شود
        };

        let dropdowns = $('.form-select');
        if (dropdowns.length === 0) return;

        dropdowns.each(function () {
            let selectedId = $(this).val(); // برای بررسی پر بودن فیلد
            let selectedTitle = $(this).find('option:selected').text().trim(); // دریافت نام (ValueTitle)
            let name = $(this).attr('name');

            // اگر کاربر هنوز گزینه‌ای را انتخاب نکرده باشد (گزینه disabled انتخاب شده باشد)
            if (!selectedId || selectedId === "") {
                allDropdownsSelected = false;
                return false; // خروج از حلقه each
            }

            // ارسال نام (ValueTitle) به جای آی‌دی به سرور
            formData.options[name] = selectedTitle;
        });

        // ۲. جمع‌آوری آی‌دی سرویس‌های انتخاب شده به صورت لیست عددی
        $('.service-check:checked').each(function () {
            let serviceId = parseInt($(this).val());
            if (!isNaN(serviceId)) {
                formData.services.push(serviceId);
            }
        });

        // ۳. شرط نهایی: فقط اگر تمام دراپ‌دان‌ها انتخاب شده بودند، پست انجام شود
        if (allDropdownsSelected) {
            $('#desktop-price, #mobile-price').css('opacity', '0.5');

            $.ajax({
                url: '/Product/CalculatePrice',
                type: 'POST',
                data: formData,
                success: function (res) {
                    if (res.success) {
                        // فرمت کردن قیمت با استاندارد فارسی (مثلاً ۱۵۰,۰۰۰)
                        let formatted = new Intl.NumberFormat('fa-IR').format(res.price);

                        // بروزرسانی قیمت در هر دو بخش دسکتاپ و موبایل
                        $('#desktop-price').text(formatted);
                        $('#mobile-price').text(formatted);
                    }
                    $('#desktop-price, #mobile-price').css('opacity', '1');
                },
                error: function () {
                    console.error("خطا در ارتباط با سرور");
                    $('#desktop-price, #mobile-price').css('opacity', '1');
                }
            });
        }
    }
});








//$(document).ready(function () {
//    $(document).on('change', '.form-select, .service-row input[type="checkbox"]', function () {
//        calculatePrice();
//    });

//    function calculatePrice() {
//        let allSelectsSelected = true;
//        let selectedOptions = {};
//        let selectedServices = [];

//        $('.form-select').each(function () {
//            let val = $(this).val();
//            let name = $(this).attr('name') || $(this).find('option:first').text();

//            if (!val || val === "" || $(this).find('option:selected').is(':disabled')) {
//                allSelectsSelected = false;
//            } else {
//                selectedOptions[name] = val;
//            }
//        });

//        $('.service-row input[type="checkbox"]:checked').each(function () {
//            selectedServices.push($(this).val() || $(this).closest('.service-row').find('.service-name').text());
//        });

//        if (allSelectsSelected) {
//            $('#final-price').text('در حال محاسبه...');

//            $.ajax({
//                url: '/Product/CalculatePrice',
//                type: 'POST',
//                data: {
//                    options: selectedOptions,
//                    services: selectedServices,
//                    productId: $('#SubProduct_ProductId').val()
//                },
//                success: function (response) {
//                    if (response.success) {
//                        // فرمت کردن قیمت (مثلاً 150,000)
//                        let formattedPrice = new Intl.NumberFormat().format(response.price);
//                        $('#final-price').html(formattedPrice + ' تومان <i class="uil uil-pricetag-alt"></i>');
//                    }
//                },
//                error: function () {
//                    console.error("خطا در ارتباط با سرور");
//                }
//            });
//        }
//    }
//});