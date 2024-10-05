using Microsoft.AspNetCore.Mvc;

namespace ShahrChap.Web.Controllers
{
    public class ActionAccessController : Controller
    {
        //Controlling the actions(Pages) user want to see but its not available
        [Route("AccessDenied")]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
