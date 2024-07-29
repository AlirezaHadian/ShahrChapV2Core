using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using ShahrChap.Core.Convertors;
using ShahrChap.Core.DTOs;
using ShahrChap.Core.Senders;
using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Web.Areas.UserPanel.Controllers;

[Area("UserPanel")]
[Authorize]
public class HomeController : Controller
{
    private readonly IUserService _userService;
    private readonly IViewRenderService _view;
    private readonly IHttpContextAccessor _context;
    
    public HomeController(IUserService UserService, IViewRenderService view, IHttpContextAccessor context)
    {
        _userService = UserService;
        _view = view;
        context = _context;
    }
    public IActionResult Index()
    {
        return View(_userService.GetUserInformation(User.Identity.Name));
    }

    [Route("UserPanel/EditProfile")]
    public IActionResult EditProfile()
    {
        return View(_userService.GetDataForEditProfileUser(User.Identity.Name));
    }

    [Route("UserPanel/EditProfile")]
    [HttpPost]
    public async Task<IActionResult> EditProfile(EditProfileViewModel editProfile)
    {
        string username = User.Identity.Name;
//Getting the current information to check with the new information
        var currentUserInformation = _userService.GetUserInformation(username);
        
        #region validation
        if (!ModelState.IsValid)
            return View(_userService.GetDataForEditProfileUser(username));
        if (currentUserInformation.UserName != editProfile.UserName &&_userService.IsUserNameExist(editProfile.UserName))
        {
            ModelState.AddModelError("UserName", "نام کاربری وارد شده تکراری می باشد");
            return View(editProfile);   
        }
        if (currentUserInformation.Email != editProfile.Email && _userService.IsEmailOrPhoneExist(FixText.FixEmail(editProfile.Email)))
        {
            ModelState.AddModelError("Email", "ایمیل وارد شده تکراری می باشد");
            return View(editProfile);
        }
        if (currentUserInformation.Phone != editProfile.Phone && _userService.IsEmailOrPhoneExist(editProfile.Phone))
        {
            ModelState.AddModelError("Phone", "شماره موبایل وارد شده تکراری می باشد");
            return View(editProfile);
        }
        #endregion
        
        _userService.EditProfile(currentUserInformation.UserName, editProfile);

        //If Username changed => resignin 
        #region ReSignIn User
        //get the user ispersistent option
        var authResult = await HttpContext.AuthenticateAsync();
        bool isPersistent = false;

        if (authResult.Succeeded && authResult.Properties != null)
        {
            isPersistent = authResult.Properties.IsPersistent;
        }
        
        //logged out
        Response.Cookies.Delete(CookieAuthenticationDefaults.AuthenticationScheme);
        
        var identity = (ClaimsIdentity)User.Identity;
        var claims = identity.Claims.ToList();
        
        var existingClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
        if (existingClaim != null)
        {
            identity.RemoveClaim(existingClaim);
        }
        
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        HttpContext.Session.Clear();
        
        //get the user
        var user = _userService.GetUserWithUserName(editProfile.UserName);
        if (user != null)
        {
            //create claims
            var newClaim = new Claim(ClaimTypes.Name, editProfile.UserName);
            
        // Create identity and principal
        identity.AddClaim(newClaim);
        var principal = new ClaimsPrincipal(identity);
        
        // Create authentication properties
        var properties = new AuthenticationProperties
        {
            IsPersistent = isPersistent
        };
        // Sign in the user
        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        //await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, properties);
        }
        else
        {
            Redirect("/Login");
        }
        #endregion
        
        //TODO: If email changed => send activation email
        if (currentUserInformation.Email != editProfile.Email)
        {                                                          
            //string emailBody = _view.RenderToStringAsync("_ChangeEmailActivation", user);
            string emailBody =
                _view.RenderToStringAsync("_ChangeEmailActivation", user);
            SendEmail.Send(user.Email, "ایمیل فعالسازی", emailBody);
        }
        
        //TODO: If phone number changed => redirect verify phone 
        if (currentUserInformation.Phone != editProfile.Phone)
        {
            MessageSender.SendOtpCode(user.Phone, _userService,_context);
            return RedirectToAction("VerifyPhone", new { actionType = "VerifyPhone" });
        }
        
        ViewBag.ToastrMessage = "اطلاعات شما با موفقیت ویرایش شد";
        return View("Index");
    }
    
    #region Active Changed Email
    [Route("UserPanel/ActiveChangedEmail")]
    public IActionResult ActiveChangedEmail(string id)
    {
        ViewBag.IsActive = _userService.ActiveEmail(id);
        return View();
    }
    #endregion

}