using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using ShahrChap.Core.Convertors;
using ShahrChap.Core.DTOs;
using ShahrChap.Core.Generators;
using ShahrChap.Core.Security;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.User;
using System.Security.Claims;

namespace ShahrChap.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ICartService _cartService;
        private readonly IViewRenderService _view;
        private readonly IHttpContextAccessor _context;
        private readonly ISMSService _smsService;
        private readonly IEmailService _emailService;
        private readonly IOtpService _otpService;
        public AccountController(IUserService userService, ICartService cartService, IViewRenderService view, IHttpContextAccessor context,
            ISMSService smsService, IEmailService emailService, IOtpService otpService)
        {
            _userService = userService;
            _cartService = cartService;
            _view = view;
            _context = context;
            _smsService = smsService;
            _emailService = emailService;
            _otpService = otpService;
        }
        #region Register
        [Route("Register")]
        public IActionResult Register()
        {
            return View();
        }

        [Route("Register")]
        [HttpPost]
        public async Task<IActionResult> Register(RegisterViewModel register)
        {
            //Checking and validating the user inputs
            if (!ModelState.IsValid)
                return View(register);

            string emailOrPhone = register.EmailOrPhone;

            if (emailOrPhone.Contains("@"))
                emailOrPhone = FixText.FixEmail(emailOrPhone);
            else
                emailOrPhone = FixText.FixPhone(emailOrPhone);


            if (_userService.IsUserNameExist(register.UserName))
            {
                ModelState.AddModelError(
                    "UserName",
                    "نام کاربری وارد شده تکراری می باشد");

                return View(register);
            }

            if (_userService.IsEmailOrPhoneExist(emailOrPhone))
            {
                ModelState.AddModelError(
                    "EmailOrPhone",
                    "ایمیل/شماره موبایل وارد شده تکراری می باشد");

                return View(register);
            }

            //Creating new user object
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
                user.Email = emailOrPhone;

                _userService.AddUser(user);

                string emailBody =
                    await _view.RenderToStringAsync("_ActivationEmail", user);

                _emailService.Send(user.Email, "ایمیل فعالسازی", emailBody);

                return View("SuccessEmailRegister", user);
            }

            user.Phone = emailOrPhone;

            _userService.AddUser(user);

            string otp = _otpService.GeneratePhoneOtp(emailOrPhone, "VerifyPhone");
            //send otp
            Console.WriteLine("Otp Code: " + otp);
            return RedirectToAction("VerifyPhone", new { actionType = "VerifyPhone" });
        }
        #endregion

        #region Login
        [Route("Login")]
        public IActionResult Login(bool EditProfile = false, string ReturnUrl = null)
        {
            ViewBag.ReturnUrl = ReturnUrl;
            if (EditProfile)
            {
                ViewBag.ToastrType = "EditProfile";
                ViewBag.ToastrTitle = "حساب شما با موفقیت ویرایش شد";
                ViewBag.ToastrMessage = "بدلیل ویرایش حساب و بارگزاری مجدد اطلاعات، لطفا مجددا وارد سایت شوید";
            }

            return View();
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> LoginAsync(LoginViewModel login, string ReturnUrl = null)
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
                    await HttpContext.SignInAsync(principal, properties);

                    string token = Request.Cookies["cart-token"];
                    if (!string.IsNullOrEmpty(token))
                    {
                        _cartService.AssignGuestCartToNewUser(user.UserName, token);
                        Response.Cookies.Delete("cart-token");
                    }


                    ViewBag.ToastrType = "Login";
                    ViewBag.ToastrMessage = "خوش آمدید!";
                    ViewBag.ToastrTitle = "ورود با موفقیت انجام شد";
                    if (!string.IsNullOrEmpty(ReturnUrl) && Url.IsLocalUrl(ReturnUrl))
                    {
                        return Redirect(ReturnUrl);
                    }
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

        #region Verify Phone
        public IActionResult VerifyPhone()
        {
            string? phone = _otpService.GetPhone();
            string? actionType = _otpService.GetActionType();

            if (string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(actionType))
                return RedirectToAction("Login");

            ViewBag.PhoneNumber = phone;
            ViewBag.OtpExpireTime = _otpService.GetExpireTime();
            ViewBag.OtpError = false;

            return View(new VerifyPhoneViewModel());
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult VerifyPhone(VerifyPhoneViewModel verifyPhone)
        {
            string? phone = _otpService.GetPhone();
            string? actionType = _otpService.GetActionType();

            if (string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(actionType))
            {
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                ViewBag.PhoneNumber = phone;
                ViewBag.OtpExpireTime = _otpService.GetExpireTime();
                ViewBag.OtpError = false;

                return View(verifyPhone);
            }

            bool isOtpValid = _otpService.VerifyPhoneOtp(
                phone,
                verifyPhone.Otp);

            if (!isOtpValid)
            {
                ViewBag.PhoneNumber = phone;
                ViewBag.OtpExpireTime = _otpService.GetExpireTime();
                ViewBag.OtpError = true;

                ModelState.AddModelError(
                    nameof(verifyPhone.Otp),
                    "کد تایید نامعتبر یا منقضی شده است.");

                return View(verifyPhone);
            }

            if (actionType == "VerifyPhone")
            {
                if (_userService.ActivePhone(phone))
                {
                    ViewBag.ToastrType = "Success";
                    ViewBag.ToastrMessage =
                        "با ورود به سایت از خدمات شهر چاپ بهره مند شوید.";
                    ViewBag.ToastrTitle =
                        "فعالسازی با موفقیت انجام شد";

                    return View("SuccessPhoneRegister");
                }

                return RedirectToAction("Login");
            }

            if (actionType == "ForgotPassword")
            {

                HttpContext.Session.SetString(
        "PasswordResetPhone",
        phone);

                HttpContext.Session.SetString(
                    "PasswordResetToken",
                    Guid.NewGuid().ToString("N"));

                HttpContext.Session.SetString(
                    "PasswordResetExpireTime",
                    DateTime.UtcNow.AddMinutes(10).ToString("O"));

                return RedirectToAction("ResetPassword");
            }

            return RedirectToAction("Login");
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ResendOtpCode()
        {
            string? phone = _otpService.GetPhone();
            string? actionType = _otpService.GetActionType();

            if (string.IsNullOrEmpty(phone) ||
                string.IsNullOrEmpty(actionType))
                return RedirectToAction("Login");

            DateTime? expireTime = _otpService.GetExpireTime();

            if (expireTime.HasValue &&
                DateTime.Now < expireTime.Value)
            {
                return RedirectToAction("VerifyPhone");
            }

            string otp = _otpService.GeneratePhoneOtp(phone, actionType);
            Console.WriteLine(otp);
            //_message.SendOtpCode(phone);

            return RedirectToAction("VerifyPhone");
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
        #region ForgotPassword
        [Route("ForgotPassword")]
        public IActionResult ForgotPassword()
        {
            return View();
        }
        [Route("ForgotPassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel forgotPassword)
        {
            if (!ModelState.IsValid)
                return View(forgotPassword);

            string input = forgotPassword.EmailOrPhone.Trim();

            if (input.Contains("@"))
            {
                string email = FixText.FixEmail(input);

                User? user = _userService.GetUserWithEmail(email);
                if (user == null)
                {
                    ModelState.AddModelError(
                nameof(forgotPassword.EmailOrPhone),
                "کاربری با مشخصات وارد شده یافت نشد.");
                    return View(forgotPassword);
                }
                string resetUrl = Url.Action(
    "ResetPassword",
    "Account",
    new { resetValue = user.ActiveCode },
    Request.Scheme,
    Request.Host.Value
)!;

                string forgotPasswordEmailBody = await _view.RenderToStringAsync(
                    "_ForgotPasswordEmail",
                    new
                    {
                        User = user,
                        ResetUrl = resetUrl
                    });

                _emailService.Send(user.Email, "بازیابی کلمه عبور", forgotPasswordEmailBody);
                return View("SuccessForgotPasswordEmail", user);
            }

            string phone = FixText.FixPhone(input);

            User? phoneUser = _userService.GetUserWithPhoneNumber(phone);
            if (phoneUser == null)
            {
                ModelState.AddModelError(
            nameof(forgotPassword.EmailOrPhone),
            "کاربری با مشخصات وارد شده یافت نشد.");
                return View(forgotPassword);
            }

            string otp = _otpService.GeneratePhoneOtp(
    phoneUser.Phone,
    "ForgotPassword");
            Console.WriteLine(otp);
            // TODO: پس از آماده‌شدن سرویس پیامک:
            // _smsService.SendOtpCode(phoneUser.Phone, otp);

            return RedirectToAction("VerifyPhone");
        }

        #endregion
        #region Reset Password
        [HttpGet]
        [Route("ResetPassword")]
        public IActionResult ResetPassword(string? resetValue)
        {
            if (HasValidPasswordResetGrant())
            {
                return View(new ResetPasswordEmailViewModel
                {
                    ResetValue = "PhoneOtp"
                });
            }

            // مسیر بازیابی با ایمیل: ActiveCode از لینک ایمیل می‌آید.
            if (!string.IsNullOrWhiteSpace(resetValue) && resetValue.Length == 32)
            {
                var user = _userService.GetUserWithActiveCode(resetValue);

                if (user == null)
                    return NotFound();

                return View(new ResetPasswordEmailViewModel
                {
                    ResetValue = resetValue
                });
            }

            return RedirectToAction("ForgotPassword");
        }
        [HttpPost]
        [Route("ResetPassword")]
        [ValidateAntiForgeryToken]
        public IActionResult ResetPassword(ResetPasswordEmailViewModel resetPassword)
        {
            if (!ModelState.IsValid)
                return View(resetPassword);
            User? user;

            if (HasValidPasswordResetGrant())
            {
                string? phone = HttpContext.Session.GetString("PasswordResetPhone");

                if (string.IsNullOrWhiteSpace(phone))
                    return RedirectToAction("ForgotPassword");

                user = _userService.GetUserWithPhoneNumber(phone);

                if (user == null)
                    return RedirectToAction("ForgotPassword");
            }
            else
            {
                string? resetValue = resetPassword.ResetValue;

                if (string.IsNullOrWhiteSpace(resetValue) ||
                    resetValue.Length != 32)
                {
                    return RedirectToAction("ForgotPassword");
                }

                user = _userService.GetUserWithActiveCode(resetValue);

                if (user == null)
                    return NotFound();
            }

            user.Password = PasswordHelper.EncodePasswordMd5(resetPassword.Password);
            user.ActiveCode = Guid.NewGuid().ToString("N");

            _userService.UpdateUser(user);

            HttpContext.Session.Remove("PasswordResetPhone");
            HttpContext.Session.Remove("PasswordResetToken");
            HttpContext.Session.Remove("PasswordResetExpireTime");

            _otpService.RemoveOtp();

            return Redirect("/Login");
        }
        private bool HasValidPasswordResetGrant()
        {
            string? phone = HttpContext.Session.GetString("PasswordResetPhone");
            string? token = HttpContext.Session.GetString("PasswordResetToken");
            string? expireTimeText = HttpContext.Session.GetString("PasswordResetExpireTime");

            if (string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(token) ||
                string.IsNullOrWhiteSpace(expireTimeText))
            {
                return false;
            }

            if (!DateTime.TryParse(
                    expireTimeText,
                    System.Globalization.CultureInfo.InvariantCulture,
                    System.Globalization.DateTimeStyles.RoundtripKind,
                    out DateTime expireTime))
            {
                return false;
            }

            return DateTime.UtcNow < expireTime.ToUniversalTime();
        }
        #endregion

        #region Send Otp code method
        /*public void SendOtpCode(string? phonenumber)
        {
            //Clearing the oldest otp's sessions
            HttpContext.Session.Remove("OtpCode");
            HttpContext.Session.Remove("OtpExpireTime");

            string userPhone;
            if (phonenumber != null)
                userPhone = phonenumber;
            else
                userPhone = HttpContext.Session.GetString("UserPhone");

            User user = _userService.GetUserWithPhoneNumber(userPhone);
            //If send otp was from posting register, the userphone will get from session
            //If send otp was from resend code button in verify phone, 

            string OtpCode = NameGenerator.GenerateOTP();
            MessageSender.SendWithPattern(userPhone, user.UserName, OtpCode);
            HttpContext.Session.SetString("OtpCode", OtpCode);
            HttpContext.Session.SetString("OtpExpireTime", DateTime.Now.AddMinutes(2).ToString());
            HttpContext.Session.SetString("UserPhone", user.Phone);
        }*/
        #endregion
    }
}
