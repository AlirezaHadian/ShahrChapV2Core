document.addEventListener("DOMContentLoaded", function () {

    const inputs = document.querySelectorAll(".otp-input-field input");
    const button = document.querySelector(".otp .otp-verify input");
    const otpField = document.getElementById("Otp");
    const form = document.getElementById("verify-phone");

    if (!inputs.length || !otpField || !form)
        return;


    // ==============================
    // OTP INPUT
    // ==============================

    inputs[0].focus();

    inputs.forEach((input, index) => {

        input.addEventListener("input", function () {

            // فقط یک رقم
            if (this.value.length > 1) {
                this.value = this.value.slice(-1);
            }

            // اگر عدد وارد شد، برو input بعدی
            if (this.value !== "" && index < inputs.length - 1) {

                const nextInput = inputs[index + 1];

                nextInput.removeAttribute("disabled");
                nextInput.focus();
            }

            updateOtp();

        });


        input.addEventListener("keydown", function (event) {

            // Backspace
            if (event.key === "Backspace") {

                if (this.value === "" && index > 0) {

                    const previousInput = inputs[index - 1];

                    previousInput.value = "";
                    previousInput.focus();

                    // input های بعدی را غیرفعال کن
                    for (let i = index; i < inputs.length; i++) {
                        inputs[i].value = "";

                        if (i !== 0)
                            inputs[i].setAttribute("disabled", true);
                    }
                }

                updateOtp();
            }


            // Arrow Left
            if (event.key === "ArrowLeft" && index > 0) {
                inputs[index - 1].focus();
            }

            // Arrow Right
            if (event.key === "ArrowRight" && index < inputs.length - 1) {
                inputs[index + 1].focus();
            }
        });


        // Paste کردن OTP
        input.addEventListener("paste", function (event) {

            event.preventDefault();

            const pastedText = (
                event.clipboardData || window.clipboardData
            ).getData("text");

            const otp = pastedText.replace(/\D/g, "");

            if (otp.length === 0)
                return;

            const digits = otp.substring(0, inputs.length);

            inputs.forEach((input, i) => {

                input.value = "";

                if (i < digits.length) {
                    input.value = digits[i];
                    input.removeAttribute("disabled");
                }
            });

            // input های باقی مانده
            for (let i = digits.length; i < inputs.length; i++) {

                if (i !== 0)
                    inputs[i].setAttribute("disabled", true);
            }

            updateOtp();

            if (digits.length === inputs.length) {
                submitOtp();
            }
        });

    });


    // ==============================
    // COMBINE OTP
    // ==============================

    function updateOtp() {

        let otp = "";

        inputs.forEach(input => {
            otp += input.value;
        });

        otpField.value = otp;

        const isComplete = otp.length === inputs.length;

        if (isComplete) {
            button.classList.add("active");
        }
        else {
            button.classList.remove("active");
        }
    }


    // ==============================
    // SUBMIT OTP
    // ==============================

    function submitOtp() {

        updateOtp();

        if (otpField.value.length !== inputs.length)
            return;

        /*
         * مهم:
         * اینجا OTP را با Backend مقایسه نمی‌کنیم.
         *
         * Backend خودش OTP را بررسی می‌کند.
         */
        form.submit();
    }


    // وقتی روی دکمه تأیید کلیک شد
    button.addEventListener("click", function (event) {

        event.preventDefault();

        submitOtp();
    });


    // ==============================
    // WRONG OTP
    // ==============================

    function showWrongOtp() {

        vibratePhone();

        inputs.forEach(input => {
            input.classList.add("wrong");
        });

        setTimeout(() => {

            inputs.forEach((input, index) => {

                input.value = "";
                input.classList.remove("wrong");

                if (index !== 0) {
                    input.setAttribute("disabled", true);
                }
            });

            inputs[0].removeAttribute("disabled");
            inputs[0].focus();

            otpField.value = "";
            button.classList.remove("active");

        }, 500);
    }


    // ==============================
    // HAPTIC FEEDBACK
    // ==============================

    function vibratePhone() {

        if ("vibrate" in navigator) {

            // ویبره کوتاه برای خطای OTP
            navigator.vibrate([
                100,
                50,
                100
            ]);
        }
    }


    // ==============================
    // OTP TIMER
    // ==============================

    const timerElement = document.getElementById("otpTimer");
    const resendButton = document.getElementById("resendCode");

    let resendSecondsRemaining = 100;
    let resendTimer = null;


    function updateResendTimer() {

        if (!timerElement)
            return;

        if (resendSecondsRemaining <= 0) {

            timerElement.textContent = "00:00";

            if (resendButton) {
                resendButton.classList.remove("disableClick");
                resendButton.removeAttribute("disabled");
            }

            clearInterval(resendTimer);

            return;
        }

        resendSecondsRemaining--;

        const minutes = Math.floor(
            resendSecondsRemaining / 60
        );

        const seconds =
            resendSecondsRemaining % 60;

        timerElement.textContent =
            `${minutes}:${seconds
                .toString()
                .padStart(2, "0")}`;
    }


    function startResendTimer() {

        if (!timerElement)
            return;

        if (resendButton) {
            resendButton.classList.add("disableClick");
            resendButton.setAttribute("disabled", "disabled");
        }

        updateResendTimer();

        resendTimer = setInterval(
            updateResendTimer,
            1000
        );
    }


    startResendTimer();

});