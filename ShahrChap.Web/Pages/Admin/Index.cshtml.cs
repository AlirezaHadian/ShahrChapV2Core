using Microsoft.AspNetCore.Mvc.RazorPages;
using ShahrChap.Core.Security;

namespace ShahrChap.Web.Pages.Admin;
[PermissionChecker(1)]
public class Index : PageModel
{

    public void OnGet()
    {
        
    }
}