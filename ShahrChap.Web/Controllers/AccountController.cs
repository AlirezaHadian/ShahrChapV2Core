using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShahrChap.Core.Security;
using ShahrChap.Core.Convertors;
using ShahrChap.Core.DTOs;
using ShahrChap.Core.Generators;
using ShahrChap.Core.Security;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.User;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication;
using ShahrChap.Core.Senders;
using static System.Net.WebRequestMethods;

namespace ShahrChap.Web.Controllers
{
    public class AccountController : Controller
    {
        private IUserService _userService;
        private IViewRenderService _view;
        public AccountController(IUserService userService, IViewRenderService view)
        {
            _userService = userService;
            _view = view;
        }
        #region Register
        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("Register")]
        [HttpPost]
        public IActionResult Register(RegisterViewModel register)
        {
            if (!ModelState.IsValid)
                View(register);

            if (_userService.IsUserNameExist(register.UserName))
            {
                ModelState.AddModelError("UserName", "نام کاربری وارد شده تکراری می باشد");
                return View(register);
            }
            if (_userService.IsEmailOrPhoneExist(FixText.FixEmail(register.EmailOrPhone)))
            {
                ModelState.AddModelError("EmailOrPhone", "ایمیل/شماره موبایل وارد شده تکراری می باشد");
                return View(register);
            }
            //TODO:: Register
            User user = new User()
            {
                UserName = register.UserName,
                Password = PasswordHelper.EncodePasswordMd5(register.Password),
                ActiveCode = NameGenerator.GenerateUniqCode(),
                IsEmailActive = false,
                IsPhoneActive = false,
                RegisterDate = DateTime.Now,
                UserAvatar = "DefaultAvatar.jpg"
            };

            //Checking input is the phone number or email
            if (register.EmailOrPhone.Contains("@"))
            {
                user.Email = FixText.FixEmail(register.EmailOrPhone);
                _userService.AddUser(user);
                string emailBody = _view.RenderToStringAsync("_ActivationEmail", user);
                SendEmail.Send(user.Email, "ایمیل فعالسازی", emailBody);
                return View("SuccessEmailRegister", user);
            }
            else
            {
                user.Phone = register.EmailOrPhone;
                _userService.AddUser(user);

                string OtpCode = NameGenerator.GenerateOTP();
                MessageSender.SendWithPattern(user.Phone, user.UserName, OtpCode);
                HttpContext.Session.SetString("OtpCode", OtpCode);
                HttpContext.Session.SetString("OtpExpireTime", DateTime.Now.AddMinutes(2).ToString());
                HttpContext.Session.SetString("UserPhone", user.Phone);

                return RedirectToAction("VerifyPhone");
            }
        }
        #endregion

        #region Login
        [Route("Login")]
        public IActionResult Login() => View();

        [HttpPost]
        [Route("Login")]
        public IActionResult Login(LoginViewModel login)
        {
            if (!ModelState.IsValid)
            {
                return View(login);
            }

            var user = _userService.LoginUser(login);
            if (user != null)
            {
                if (user.IsEmailActive || user.IsPhoneActive)
                {
                    var claims = new List<Claim>()
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.UserId.ToString()),
                        new Claim(ClaimTypes.Name, user.UserName.ToString())
                    };
                    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                    var principal = new ClaimsPrincipal(identity);
                    var properties = new AuthenticationProperties
                    {
                        IsPersistent = login.RememberMe
                    };
                    HttpContext.SignInAsync(principal, properties);

                    ViewBag.ToastrType = "Success";
                    ViewBag.ToastrMessage = "خوش آمدید!";
                    ViewBag.ToastrTitle = "ورود با موفقیت انجام شد";
                    return View();
                }
                else
                {
                    ModelState.AddModelError("EmailOrPhone", "حساب کاربری شما فعال نمی باشد");
                }
            }
            ModelState.AddModelError("EmailOrPhone", "کاربری با مشخصات وارد شده یافت نشد");
            return View(login);
        }
        #endregion
        #region Active Email Account
        public IActionResult ActiveEmail(string id)
        {
            ViewBag.IsActive = _userService.ActiveEmail(id);
            return View();
        }
        #endregion

        #region Phone Active
        public IActionResult VerifyPhone()
        {
            ViewBag.PhoneNumber = HttpContext.Session.GetString("UserPhone");
            return View();
        }
        [HttpPost]
        public IActionResult VerifyPhone(VerifyPhoneViewModel verifyPhone)
        {
            if (!ModelState.IsValid)
                return View(verifyPhone);

            string otp = HttpContext.Session.GetString("OtpCode");
            string expireTime = HttpContext.Session.GetString("OtpExpireTime");
            DateTime expirationTime = DateTime.Parse(expireTime);
            //Check the otp code
            if (otp == null && expirationTime == null)
            {
                ModelState.AddModelError("Otp", "کد اعتبار سنجی نامعتبر می باشد");
                return View(verifyPhone);
            }
            
            if (verifyPhone.Otp == otp && expirationTime > DateTime.Now)
            {
                if (_userService.ActivePhone(verifyPhone.PhoneNumber))
                {
                    ViewBag.ToastrType = "Success";
                    ViewBag.ToastrMessage = "با ورود به سایت از خدمات شهر چاپ بهره مند شوید.";
                    ViewBag.ToastrTitle = "فعالسازی با موفقیت انجام شد";
                    return RedirectToAction("Index", "Home");
                }
            }
            else if (expirationTime < DateTime.Now)
            {
                ModelState.AddModelError("Otp", "کد اعتبار سنجی منقضی شده است");
                return View(verifyPhone);
            }
            else if (verifyPhone.Otp != otp)
            {
                ModelState.AddModelError("Otp", "کد اعتبارسنجی نامعتبر می باشد");
                return View(verifyPhone);
            }
            return View();
        }

        [HttpGet]
        public IActionResult GetOtp()
        {
            var sessionOtp = HttpContext.Session.GetString("OtpCode");
            return Json(new { otp = sessionOtp });
        }
        #endregion
        #region Logout
        [Route("Logout")]
        public IActionResult Logout()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Redirect("/?loggedOut=true");
        }
        #endregion
    }
}
