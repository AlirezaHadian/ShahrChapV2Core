using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.DTOs;
using ShahrChap.Core.Services;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.User;
using System.Globalization;

namespace ShahrChap.Web.Pages.Admin.Users
{
    public class EditUserModel : PageModel
    {
        private IUserService _userService;
        private IPermissionService _permissionService;
        public EditUserModel(IUserService userService, IPermissionService permissionService)
        {
            _userService = userService;
            _permissionService = permissionService;
        }

        [BindProperty]
        public EditUserViewModel EditUserViewModel { get; set; }
        public void OnGet(int id)
        {
            EditUserViewModel = _userService.GetUserForShowInEditMode(id);
            ViewData["Roles"] = _permissionService.GetRoles();
        }
        public IActionResult OnPost(List<int> SelectedRoles)
        {
            var roles = _permissionService.GetRoles();
            ViewData["Roles"] = roles;
            User user = _userService.GetUserWithId(EditUserViewModel.UserId);
            EditUserViewModel.UserRoles = SelectedRoles;

            if (EditUserViewModel.Email == null && EditUserViewModel.Phone == null)
            {
                ModelState.AddModelError("EditUserViewModel.Email", "یکی از فیلد های ایمیل و یا شماره موبایل را وارد کنید");
                ModelState.AddModelError("EditUserViewModel.Phone", "یکی از فیلد های ایمیل و یا شماره موبایل را وارد کنید");
                return Page();
            }
            if (!ModelState.IsValid)
                return Page();


            if (EditUserViewModel.Email != null && EditUserViewModel.Email != user.Email && _userService.IsEmailOrPhoneExist(EditUserViewModel.Email))
            {
                ModelState.AddModelError("EditUserViewModel.Email", "ایمیل وارد شده تکراری می باشد");
                return Page();
            }

            else if (EditUserViewModel.Phone != null && EditUserViewModel.Phone != user.Phone && _userService.IsEmailOrPhoneExist(EditUserViewModel.Phone))
            {
                ModelState.AddModelError("EditUserViewModel.Phone", "شماره موبایل وارد شده تکراری می باشد");
                return Page();
            }

            _userService.EditUserForAdmin(EditUserViewModel);
            _permissionService.EditRolesUser(EditUserViewModel.UserId, SelectedRoles);

            return Redirect("/Admin/Users");
        }
    }
}
