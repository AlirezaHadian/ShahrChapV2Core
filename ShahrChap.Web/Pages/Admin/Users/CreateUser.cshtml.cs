using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.DTOs;
using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Web.Pages.Admin.Users
{
    public class CreateUserModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;
        public CreateUserModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        [BindProperty]
        public CreateUserViewModel CreateUser { get; set; }
        public void OnGet()
        {
            ViewData["Roles"] = _permissionService.GetRoles();
        }

        public IActionResult OnPost(List<int> SelectedRoles)
        {
            ViewData["Roles"] = _permissionService.GetRoles();

            if (CreateUser.Email == null && CreateUser.Phone == null)
            {
                ModelState.AddModelError("CreateUser.Email", "یکی از فیلد های ایمیل و یا شماره موبایل را وارد کنید");
                ModelState.AddModelError("CreateUser.Phone", "یکی از فیلد های ایمیل و یا شماره موبایل را وارد کنید");
                return Page();
            }
            if (!ModelState.IsValid)
                return Page();

            if (CreateUser.Email != null && _userService.IsEmailOrPhoneExist(CreateUser.Email))
            {
                ModelState.AddModelError("CreateUser.Email", "ایمیل وارد شده تکراری می باشد");
                return Page();
            }
            else if (CreateUser.Phone != null && _userService.IsEmailOrPhoneExist(CreateUser.Phone))
            {
                ModelState.AddModelError("CreateUser.Phone", "شماره موبایل وارد شده تکراری می باشد");
                return Page();
            }

            //Cerate User
            int userId = _userService.AddUserForAdmin(CreateUser);

            //Add Roles to user
            _permissionService.AddRolesToUser(SelectedRoles, userId);

            return Redirect("/Admin/Users");
        }
    }
}
