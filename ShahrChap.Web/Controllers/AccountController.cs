using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using ShahrChap.Core.Security;
using ShahrChap.Core.Convertors;
using ShahrChap.Core.DTOs;
using ShahrChap.Core.Generators;
using ShahrChap.Core.Security;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.User;

namespace ShahrChap.Web.Controllers
{
    public class AccountController : Controller
    {
        private IUserService _userService;
        public AccountController(IUserService userService)
        {
            _userService = userService;
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

            if(_userService.IsUserNameExist(register.UserName))
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
                return View("SuccessEmailRegister", user);
            }
            else
            {
                user.Phone = register.EmailOrPhone;
               _userService.AddUser(user);
                //OTP Section
                //Sending message, Storing the otp code
                return RedirectToAction("OTP", user);
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
            if (user!=null)
            {
                if(user.IsEmailActive || user.IsPhoneActive)
                {
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
        #region Active Account
        public IActionResult ActiveEmail(string id)
        {
            ViewBag.IsActive = _userService.ActiveEmail(id);
            return View();
        }
        #endregion
    }
}
