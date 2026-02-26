using Microsoft.AspNetCore.Mvc;

namespace ShahrChap.Web.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
