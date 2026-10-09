
document.addEventListener("DOMContentLoaded", function () {
    const inputs = Array.from(
        document.querySelectorAll(".otp-input-field input")
    );

    const button = document.querySelector(
        "#verify-phone .signup-btn"
    );

    const otpField = document.getElementById("Otp");
    const verifyForm = document.getElementById("verify-phone");
    const timerElement = document.getElementById("otpTimer");
    const resendButton = document.getElementById("resendCode");
    const otpContainer = document.getElementById("otpInputs");

    if (!inputs.length || !button || !otpField || !verifyForm) {
        return;
    }

    let resendTimer = null;

    // =====================================
    // OTP INPUT
    // =====================================

    function updateOtp() {
        const otp = inputs.map(input => input.value).join("");

        otpField.value = otp;

        const isComplete =
            otp.length === inputs.length &&
            inputs.every(input => /^\d$/.test(input.value));

        button.classList.toggle("active", isComplete);
        button.disabled = !isComplete;

        return isComplete;
    }

    function focusNext(index) {
        if (index < inputs.length - 1) {
            inputs[index + 1].focus();
        }
    }

    inputs.forEach((input, index) => {
        input.addEventListener("input", function () {
            // فقط ارقام انگلیسی 0 تا 9
            const digits = this.value.replace(/[^0-9]/g, "");

            // پشتیبانی از واردکردن یک کد کامل توسط Autofill
            if (digits.length > 1) {
                const otp = digits.slice(0, inputs.length);

                inputs.forEach((item, i) => {
                    item.value = otp[i] || "";
                });

                const nextEmptyIndex = inputs.findIndex(
                    item => item.value === ""
                );

                if (nextEmptyIndex === -1) {
                    inputs[inputs.length - 1].focus();
                } else {
                    inputs[nextEmptyIndex].focus();
                }

                updateOtp();
                return;
            }

            this.value = digits;

            if (this.value !== "") {
                focusNext(index);
            }

            updateOtp();
        });

        input.addEventListener("keydown", function (event) {
            if (event.key === "Backspace") {
                event.preventDefault();

                if (this.value !== "") {
                    // ابتدا مقدار همین کادر پاک می‌شود
                    this.value = "";
                } else if (index > 0) {
                    // اگر خالی بود، به کادر قبلی برمی‌گردیم
                    inputs[index - 1].focus();
                    inputs[index - 1].value = "";
                }

                updateOtp();
                return;
            }

            if (event.key === "ArrowLeft" && index > 0) {
                event.preventDefault();
                inputs[index - 1].focus();
            }

            if (event.key === "ArrowRight" && index < inputs.length - 1) {
                event.preventDefault();
                inputs[index + 1].focus();
            }
        });

        input.addEventListener("paste", function (event) {
            event.preventDefault();

            const clipboard = event.clipboardData ||
                window.clipboardData;

            const digits = clipboard
                .getData("text")
                .replace(/[^0-9]/g, "")
                .slice(0, inputs.length);

            if (!digits) {
                return;
            }

            inputs.forEach((item, i) => {
                item.value = digits[i] || "";
            });

            const nextEmptyIndex = inputs.findIndex(
                item => item.value === ""
            );

            if (nextEmptyIndex === -1) {
                inputs[inputs.length - 1].focus();
            } else {
                inputs[nextEmptyIndex].focus();
            }

            updateOtp();
        });
    });

    // قبل از ارسال فرم، مقدار نهایی را به فیلد مخفی منتقل کن
    verifyForm.addEventListener("submit", function (event) {
        if (!updateOtp()) {
            event.preventDefault();
            return;
        }

        button.disabled = true;
    });

    // =====================================
    // HAPTIC FEEDBACK / WRONG OTP
    // =====================================

    function vibratePhone() {
        if (typeof navigator.vibrate === "function") {
            navigator.vibrate([100, 50, 100]);
        }
    }

    function showWrongOtp() {
        vibratePhone();

        inputs.forEach(input => {
            input.classList.add("wrong");
        });

        setTimeout(() => {
            inputs.forEach(input => {
                input.value = "";
                input.classList.remove("wrong");
            });

            otpField.value = "";
            button.classList.remove("active");
            button.disabled = true;

            inputs[0].focus();
        }, 500);
    }

    // این وضعیت را سرور هنگام OTP اشتباه به View می‌فرستد
    if (otpContainer?.dataset.otpError === "true") {
        showWrongOtp();
    }

    // =====================================
    // OTP TIMER
    // =====================================

    function startResendTimer() {
        if (!timerElement || !resendButton) {
            return;
        }

        clearInterval(resendTimer);

        const expireTimeString = timerElement.dataset.expireTime;

        if (!expireTimeString) {
            timerElement.textContent = "00:00";
            enableResendButton();
            return;
        }

        const expireTime = new Date(expireTimeString).getTime();

        if (Number.isNaN(expireTime)) {
            timerElement.textContent = "00:00";
            enableResendButton();
            return;
        }

        resendButton.disabled = true;
        resendButton.classList.add("disableClick");

        function updateTimer() {
            const remainingMilliseconds = expireTime - Date.now();

            if (remainingMilliseconds <= 0) {
                timerElement.textContent = "00:00";
                enableResendButton();
                clearInterval(resendTimer);
                return;
            }

            const totalSeconds = Math.ceil(
                remainingMilliseconds / 1000
            );

            const minutes = Math.floor(totalSeconds / 60);
            const seconds = totalSeconds % 60;

            timerElement.textContent =
                `${minutes}:${seconds.toString().padStart(2, "0")}`;
        }

        updateTimer();
        resendTimer = setInterval(updateTimer, 1000);
    }

    function enableResendButton() {
        if (!resendButton) {
            return;
        }

        resendButton.disabled = false;
        resendButton.classList.remove("disableClick");
    }

    // =====================================
    // INITIALIZATION
    // =====================================

    updateOtp();
    startResendTimer();

    // اگر خطای سرور وجود نداشت، اولین کادر فوکوس شود
    if (otpContainer?.dataset.otpError !== "true") {
        inputs[0].focus();
    }
});