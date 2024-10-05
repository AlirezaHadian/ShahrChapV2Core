using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Security;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.User;

namespace ShahrChap.Web.Pages.Admin.Roles
{
    [PermissionChecker(7)]
    public class CreateRoleModel : PageModel
    {
        private IPermissionService _permissionService;
        public CreateRoleModel(IPermissionService permissionService)
        {
            _permissionService = permissionService;
        }
        [BindProperty]
        public Role Role { get; set; }
        public void OnGet()
        {
            ViewData["Permissions"] = _permissionService.GetPermissions();
        }

        public IActionResult OnPost(List<int> SelectedPermission)
        {
            ViewData["Permissions"] = _permissionService.GetPermissions();

            if (!ModelState.IsValid)
                return Page();

            Role.IsDelete = false;
            int roleId = _permissionService.AddRole(Role);

            _permissionService.AddPermissionsToRole(roleId, SelectedPermission);

            return RedirectToPage("Index");
        }
    }
}
