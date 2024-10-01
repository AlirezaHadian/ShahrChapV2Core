using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.DTOs;
using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Web.Pages.Admin.Users;

public class Index : PageModel
{
    private IUserService _userService;

    public Index(IUserService userService)
    {
        _userService = userService;
    }

    public UserForAdminViewModel UserForAdminViewModel { get; set; }
    public void OnGet(int pageId=1, string filterUser="")
    {
        UserForAdminViewModel = _userService.GetUsers(pageId, filterUser);
    }
}