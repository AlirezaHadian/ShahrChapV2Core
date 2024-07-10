using Microsoft.AspNetCore.Mvc;

namespace ShahrChap.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
        
    }
}
