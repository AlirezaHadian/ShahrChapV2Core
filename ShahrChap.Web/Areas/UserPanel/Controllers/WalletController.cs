using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ShahrChap.Core.DTOs;
using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Web.Areas.UserPanel.Controllers;
[Authorize]
[Area("UserPanel")]
public class WalletController : Controller
{
    private readonly IUserService _userService;

    public WalletController(IUserService userService)
    {
        _userService = userService;
    }
    [Route("UserPanel/Wallet")]
    public IActionResult Index()
    {
        ViewBag.WalletDetails = _userService.GetWalletDetailUser(User.Identity.Name);
        ViewData["TotalCash"] = _userService.BalanceUserWallet(User.Identity.Name);
        return View();
    }

    [HttpPost]
    [Route("UserPanel/Wallet")]
    public IActionResult Index(ChargeWalletViewModel charge)
    {
        if (!ModelState.IsValid)
        {
        ViewBag.WalletDetails = _userService.GetWalletDetailUser(User.Identity.Name);
        return View(charge);
        }
        
        int walletId = _userService.ChargeWallet(User.Identity.Name, charge.Amount, "شارژ حساب");

        #region Online payment

        var payment = new ZarinpalSandbox.Payment(charge.Amount);
        var response = payment.PaymentRequest("شارژ کیف پول", "https://localhost:7254/OnlinePayment/" + walletId);
        if (response.Result.Status == 100)
        {
            return Redirect("https://sandbox.zarinpal.com/pg/StartPay/" + response.Result.Authority);
        }
        #endregion 
        return null;
    }
}