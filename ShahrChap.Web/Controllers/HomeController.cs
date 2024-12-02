using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Wallet;

namespace ShahrChap.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly IUserService _userService;
        public HomeController(IUserService userService)
        {
            _userService = userService;
        }
        public IActionResult Index() => View();
        [Route("OnlinePayment/{id}")]
        public IActionResult OnlinePayment(int id)
        {
            if (HttpContext.Request.Query["Status"] != "" &&
                HttpContext.Request.Query["Status"].ToString().ToLower() == "ok" &&
                HttpContext.Request.Query["Authority"] != "")
            {
                string authority = HttpContext.Request.Query["Authority"];
                Wallet wallet = _userService.GetWalletWithWalletId(id);
                
                var payment = new ZarinpalSandbox.Payment(wallet.Amount);
                var response = payment.Verification(authority).Result;
                if (response.Status == 100)
                {
                    ViewBag.Code = response.RefId;
                    ViewBag.IsSuccess = true;
                    ViewBag.Amount = wallet.Amount;
                    wallet.IsPay = true;
                    _userService.UpdateWallet(wallet);
                }
            }

            return View();
        }

        [HttpPost]
        [Route("file-upload")]
        public IActionResult UploadImage(IFormFile upload, string CKEditorFuncNum, string CKEditor, string langCode)
        {
            if (upload.Length <= 0) return null;

            var fileName = Guid.NewGuid() + Path.GetExtension(upload.FileName).ToLower();



            var path = Path.Combine(
                Directory.GetCurrentDirectory(), "wwwroot/images",
                fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                upload.CopyTo(stream);

            }

            var url = $"{"/images/"}{fileName}";


            return Json(new { uploaded = true, url });
        }

    }
}
