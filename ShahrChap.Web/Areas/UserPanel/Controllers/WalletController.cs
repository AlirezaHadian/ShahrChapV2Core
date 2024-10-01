using Microsoft.AspNetCore.Mvc;

namespace ShahrChap.Web.Areas.UserPanel.Controllers;

public class WalletController : Controller
{
    // GET
    public IActionResult Index()
    {
        return View();
    }
}