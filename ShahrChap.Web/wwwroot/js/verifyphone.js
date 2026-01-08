/*============== OTP PAGE ==========*/
//const inputs = document.querySelectorAll(".otp-input-field input"),
//  button = document.querySelector(".otp .otp-verify input");

//inputs.forEach((input, index1) => {
//  input.addEventListener("keyup", (e) => {
//    const currentInput = input,
//      nextInput = input.nextElementSibling,
//      prevInput = input.previousElementSibling;

//    if (currentInput.value.length > 1) {
//      currentInput.value = "";
//      return;
//    }

//    if (
//      nextInput &&
//      nextInput.hasAttribute("disabled") &&
//      currentInput.value !== ""
//    ) {
//      nextInput.removeAttribute("disabled");
//      nextInput.focus();
//    }

//    if (e.key === "Backspace") {
//      inputs.forEach((input, index2) => {
//        if (index1 <= index2 && prevInput) {
//          input.setAttribute("disabled", true);
//          input.value = "";
//          prevInput.focus();
//        }
//      });
//    }

//    if (!inputs[4].disabled && inputs[4].value !== "") {
//      button.classList.add("active");
//      return;
//    }
//    button.classList.remove("active");
//  });
//});
//window.addEventListener("load", () => inputs[0].focus());


const inputs = document.querySelectorAll(".otp-input-field input"),
    button = document.querySelector(".otp .otp-verify input");
const otpField = document.getElementById('Otp');
let backendOtp = '';
// Fetch OTP from backend
fetch('/Account/GetOtp')
    .then(response => response.json())
    .then(data => {
        backendOtp = data.otp;
    });

inputs.forEach((input, index1) => {
    input.addEventListener("keyup", (e) => {
        const currentInput = input, nextInput = input.nextElementSibling, prevInput = input.previousElementSibling;

        if (currentInput.value.length > 1) {
            currentInput.value = "";
            return;
        }

        if (nextInput && nextInput.hasAttribute("disabled") && currentInput.value !== "") {
            nextInput.removeAttribute("disabled");
            nextInput.focus();
        }

        if (e.key === "Backspace") {
            inputs.forEach((input, index2) => {
                if (index1 <= index2 && prevInput) {
                    input.setAttribute("disabled", true);
                    input.value = "";
                    prevInput.focus();
                }
            });
        }

        if (!inputs[4].disabled && inputs[4].value !== "") {
            button.classList.add("active");
            combineOtp();
            return;
        }
        button.classList.remove("active");

        function combineOtp() {
            let otp = '';
            inputs.forEach(input => {
                otp += input.value;
            });
            otpField.value = otp;

            if (otp.length === 5) {
                validateOtp(otp);
            }
        }
        function validateOtp(otp) {
            if (otp === backendOtp) {
                document.getElementById('verify-phone').submit();
            } else {
                vibratePhone();
                addWrongClass();
                button.setAttribute("disabled", true);
            }
        }
        function vibratePhone() {
            if (navigator.vibrate) {
                navigator.vibrate(500); // Vibrate for 500ms
                window.location.href = "/Account/ResendOtp";
            }
        }

        function addWrongClass() {
            inputs.forEach(input => input.classList.add('wrong'));
            setTimeout(() => {
                inputs.forEach(input => {
                    input.value = '';
                    input.classList.remove('wrong');
                    input.setAttribute("disabled", true);
                });
                inputs[0].removeAttribute("disabled");
                inputs[0].focus();
            }, 500);
        }
    });
});
window.addEventListener("load", () => inputs[0].focus());

//OTP Countdown timer
let resendSecondsRemaining = 100;
let resendTimer;

function startResendTimer() {
    resendTimer = setInterval(updateResendTimer, 1000);
}

function updateResendTimer() {
    if (resendSecondsRemaining > 0) {
        resendSecondsRemaining--;
        const minutes = Math.floor(resendSecondsRemaining / 60);
        const seconds = resendSecondsRemaining % 60;
        document.getElementById('otpTimer').textContent = `${minutes}:${seconds.toString().padStart(2, '0')}`;
    } else {
        document.getElementById('resendCode').classList.remove('disableClick')
            = false;
        clearInterval(resendTimer);
    }
}
window.onload = startResendTimer;


//Haptic feedback => increase the number for vibration
// const vibrate = () => {
//   window.navigator.vibrate([20])
// }