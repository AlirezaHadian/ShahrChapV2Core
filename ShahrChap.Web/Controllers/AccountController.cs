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
        private readonly IUserService _userService;
        private readonly IViewRenderService _view;
        private readonly IHttpContextAccessor _context;
        private readonly MessageSender _message;
        public AccountController(IUserService userService, IViewRenderService view, IHttpContextAccessor context, MessageSender message)
        {
            _userService = userService;
            _view = view;
            _context = context;
            _message = message;
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
            //Checking and validating the user inputs
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

                //MessageSender.SendOtpCode(user.Phone);
                _message.SendOtpCode(user.Phone);
                return RedirectToAction("VerifyPhone", new { actionType = "VerifyPhone" });
            }
        }
        #endregion

        #region Login
        [Route("Login")]
        public IActionResult Login(bool EditProfile =false)
        {
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

                    ViewBag.ToastrType = "Login";
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

        #region Verify Phone
        public IActionResult VerifyPhone(string actionType)
        {
            ViewBag.PhoneNumber = HttpContext.Session.GetString("UserPhone");
            ViewBag.ActionType = actionType;
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
                if (verifyPhone.ActionType == "VerifyPhone")
                {
                    if (_userService.ActivePhone(verifyPhone.PhoneNumber))
                    {
                        ViewBag.ToastrType = "Success";
                        ViewBag.ToastrMessage = "با ورود به سایت از خدمات شهر چاپ بهره مند شوید.";
                        ViewBag.ToastrTitle = "فعالسازی با موفقیت انجام شد";
                        return RedirectToAction("Index", "Home");
                    }
                }
                else if (verifyPhone.ActionType == "ForgotPassword")
                {
                    return RedirectToAction("ResetPassword", new { resetValue = verifyPhone.PhoneNumber });
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

        //This Actionresult is for the resend button in verify phone page
        //It will create a new otp, and then redirect to verify phone action
        public IActionResult ResendOtpCode(string phone, string type)
        {
            _message.SendOtpCode(phone);
            return RedirectToAction("VerifyPhone", new {actionType = type});
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
        public IActionResult ForgotPassword(ForgotPasswordViewModel forgotPassword)
        {
            if (!ModelState.IsValid)
                return View(forgotPassword);

            User user;
            if (forgotPassword.EmailOrPhone.Contains("@"))
            {
                string fixEmail = FixText.FixEmail(forgotPassword.EmailOrPhone);
                user = _userService.GetUserWithEmail(fixEmail);
                if (user == null)
                {
                    ModelState.AddModelError("EmailOrPhone", "کاربری با مشخصات وارد شده یافت نشد");
                    return View(forgotPassword);
                }
                string forgotPasswordEmailBody = _view.RenderToStringAsync("_ForgotPasswordEmail", user);
                SendEmail.Send(user.Email, "بازیابی کلمه عبور", forgotPasswordEmailBody);
                return View("SuccessForgotPasswordEmail", user);
            }
            else
            {
                user = _userService.GetUserWithPhoneNumber(forgotPassword.EmailOrPhone);
                if (user == null)
                {
                    ModelState.AddModelError("EmailOrPhone", "کاربری با مشخصات وارد شده یافت نشد");
                    return View(forgotPassword);
                }
                _message.SendOtpCode(user.Phone);
                //MessageSender.SendOtpCode(user.Phone, _userService,_context);
                return RedirectToAction("VerifyPhone", new { actionType = "ForgotPassword" });
            }
        }

        #endregion
        #region Reset Password
        public IActionResult ResetPassword(string resetValue)
        {
            //Reset value is active code or phone number
            return View(new ResetPasswordEmailViewModel()
            {
                ResetValue = resetValue
            });
        }
        [HttpPost]
        public IActionResult ResetPassword(ResetPasswordEmailViewModel resetPassword)
        {
            if (!ModelState.IsValid)
                return View(resetPassword);
            User user;
            //This condition check what is the reset value, is it active code or its phone number
            //If it's active code, so user entered email for forgoten password
            //If it's phone, so user entered phone number for forgoten password
            if (resetPassword.ResetValue.Length == 32)
            {
                user = _userService.GetUserWithActiveCode(resetPassword.ResetValue);
                if (user == null)
                    return NotFound();
            }
            else
            {
                user = _userService.GetUserWithPhoneNumber(resetPassword.ResetValue);
                if (user == null)
                    return NotFound();
            }

            string hashPassword = PasswordHelper.EncodePasswordMd5(resetPassword.Password);
            user.Password = hashPassword;
            _userService.UpdateUser(user);
            return Redirect("/Login");
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
