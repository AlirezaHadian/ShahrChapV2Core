/*=============INTERNET CONNECTION ============== */
const popup = document.querySelector(".popup"),
    wifiIcon = document.querySelector(".icon i"),
    popupTitle = document.querySelector(".popup .title");
let isOnline = true,
    intervalId,
    timer = 10;
const checkConnection = async () => {
    try {
        const response = await fetch("https://jsonplaceholder.typicode.com/posts");
        isOnline = response.status >= 200 && response.status < 300;
    } catch (error) {
        isOnline = false; // If there is an error, the connection is considered offline
    }
    timer = 8;
    clearInterval(intervalId);
    handlePopup(isOnline);
};
const handlePopup = (status) => {
    if (status) {
        // If the status is true (online), update icon, title, and description accordingly
        wifiIcon.className = "uil uil-wifi";
        popupTitle.innerText = "اتصال بازیابی شد";
        popup.classList.add("online");
        return setTimeout(() => popup.classList.remove("show"), 2000);
    }
    // If the status is false (offline), update the icon, title, and description accordingly
    wifiIcon.className = "uil uil-wifi-slash";
    popupTitle.innerText = "خطا در برقراری ارتباط";
    popup.className = "popup show";

    intervalId = setInterval(() => {
        // Set an interval to decrease the timer by 1 every second
        timer--;
        if (timer === 0) checkConnection(); // If the timer reaches 0, check the connection again
    }, 1000);
};
// Only if isOnline is true, check the connection status every 3 seconds
setInterval(() => isOnline && checkConnection(), 3000);
/*============== NAVIGATION ==============*/
const body = document.querySelector("body"),
  nav = document.querySelector("nav"),
  searchToggle = document.querySelector(".searchToggle"),
  sidebarOpen = document.querySelector(".sidebarOpen"),
  siderbarClose = document.querySelector(".siderbarClose");
// js code to toggle search box
searchToggle.addEventListener("click", () => {
  searchToggle.classList.toggle("active");
});
//   js code to toggle sidebar
sidebarOpen.addEventListener("click", () => {
  nav.classList.add("active");
  body.classList.add("no-scroll");
});
body.addEventListener("click", (e) => {
  let clickedElm = e.target;
  if (
    !clickedElm.classList.contains("sidebarOpen") &&
    !clickedElm.classList.contains("menu")
  ) {
    nav.classList.remove("active");
    body.classList.remove("no-scroll");
  }
});
/* ============USERPANEL DROPDOWN========== */
function dropUserPanel() {
  var dropdown = document.getElementById("userpanel-dropdown");
  if (dropdown.classList.contains("show")) {
    dropdown.classList.remove("show");
  } else {
    dropdown.classList.add("show");
  }
}
function handleClickOutside(event) {
  var dropdowns = document.getElementsByClassName("userpanel-dropdown-content");

  for (var i = 0; i < dropdowns.length; i++) {
    var openDropdown = dropdowns[i];
    if (
      openDropdown.classList.contains("show") &&
      !event.target.closest(".userpanel-dropdown")
    ) {
      openDropdown.classList.remove("show");
    }
  }
}
document.addEventListener("click", handleClickOutside);
document
  .querySelector(".userpanel")
  .addEventListener("click", function (event) {
    event.stopPropagation();
  });
/*=============SWIPER===========*/
//var swiper = new Swiper(".products-swiper", {
//  slidesPerView: 4,
//  spaceBetween: 10,
//  loop: true,
//  navigation: {
//    clickable: true,
//  },
//  autoplay: {
//    delay: 4500,
//    disableOnInteraction: false,
//  },
//  breakpoints: {
//    0: {
//      slidesPerView: 1,
//    },
//    520: {
//      slidesPerView: 2,
//    },
//    950: {
//      slidesPerView: 3,
//    },
//    1000: {
//      slidesPerView: 4,
//    },
//  },
//});

document.addEventListener('DOMContentLoaded', function () {
    //Products swiper
    const productsSwiper = document.querySelector('.products-swiper');
    if (productsSwiper) {
        var swiper = new Swiper(".products-swiper", {
            slidesPerView: 4,
            spaceBetween: 10,
            loop: false,
            navigation: {
                clickable: true,
            },
            autoplay: {
                delay: 4500,
                disableOnInteraction: false,
            },
            breakpoints: {
                0: {
                    slidesPerView: 1,
                },
                520: {
                    slidesPerView: 2,
                },
                950: {
                    slidesPerView: 3,
                },
                1000: {
                    slidesPerView: 4,
                },
            },
        });
    }

    //Sample work swiper 
    const sampleSwiper = document.querySelector('.work-sample-swiper');
    if (sampleSwiper) {
        var swiper = new Swiper(".work-sample-swiper", {
            slidesPerView: 4,
            spaceBetween: 10,
            loop: false,
            navigation: {
                clickable: true,
            },
            autoplay: {
                delay: 4500,
                disableOnInteraction: false,
            }
        });
    }
});

/* Preloader */
// window.addEventListener('load', ()=> document.querySelector('.preloader').classList.add('hide-preloader'));
/*============ SHOPCART POPOVER ============*/
const shopcartTrigger = document.querySelector(".shopcart");
const shopcartPopover = document.querySelector(".shopcart-popover");
shopcartTrigger.addEventListener("mouseover", function () {
  if (window.innerWidth > 790) {
    shopcartPopover.style.display = "block";
  }
});

document.addEventListener("mouseover", function (event) {
  if (window.innerWidth > 790) {
    if (
      !shopcartPopover.contains(event.target) &&
      !shopcartTrigger.contains(event.target)
    ) {
      shopcartPopover.style.display = "none";
    }
  }
});
/*=============REGISTER PAGE ===========*/
const container = document.querySelector(".container"),
  pwShowHide = document.querySelectorAll(".showHidePw"),
  pwFields = document.querySelectorAll(".password");

pwShowHide.forEach((eyeIcon) => {
  eyeIcon.addEventListener("click", () => {
    pwFields.forEach((pwField) => {
      if (pwField.type === "password") {
        pwField.type = "text";

        pwShowHide.forEach((icon) => {
          icon.classList.replace("uil-eye-slash", "uil-eye");
        });
      } else {
        pwField.type = "password";
        pwShowHide.forEach((icon) => {
          icon.classList.replace("uil-eye", "uil-eye-slash");
        });
      }
    });
  });
});

