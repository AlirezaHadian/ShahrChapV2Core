using Humanizer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ShahrChap.Core.DTOs;
using ShahrChap.Core.Services.Interfaces;
using ShahrChap.DataLayer.Entities.Address;

namespace ShahrChap.Web.Areas.UserPanel.Controllers;

[Area("UserPanel")]
[Authorize]
public class AddressController : Controller
{
    private readonly IUserService _userService;
    public AddressController(IUserService userService)
    {
        _userService = userService;
    }

    public IActionResult Index()
    {
        return View(_userService.GetUserAdresses(User.Identity.Name));
    }

    public IActionResult AddAddress()
    {
        ViewBag.UserId = _userService.GetUserIdWithUserName(User.Identity.Name);
        ViewData["Provinces"] = new SelectList(_userService.GetAllProvince(), "ProvinceId", "ProvinceName");

        return View();
    }
    [HttpPost]
    public IActionResult AddAddress(AddressViewModel addAddress)
    {
        int userId = _userService.GetUserIdWithUserName(User.Identity.Name);
        ViewBag.UserId = userId;
        ViewData["Provinces"] = new SelectList(_userService.GetAllProvince(), "ProvinceId", "ProvinceName", addAddress.ProvinceId);
        ViewData["Cities"] = new SelectList(_userService.GetProvincCities(addAddress.ProvinceId), "CityId", "CityName", addAddress.CityId);

        if (!ModelState.IsValid)
            return View(addAddress);

        var address = new UserAddress
        {
            CityId = addAddress.CityId,
            FullAddress = addAddress.FullAddress,
            FullName = addAddress.FullName,
            HouseNumber = addAddress.HouseNumber,
            PostCode = addAddress.PostCode,
            ProvinceId = addAddress.ProvinceId,
            UserId = userId
        };

        //Add Address
        _userService.AddAddress(address);
        return RedirectToAction("Index");
    }

    public IActionResult EditAddress(int Id)
    {
        UserAddress address = _userService.GetUserAddressWithAddressId(Id);
        if (address == null)
        {
            return NotFound();
        }

        int userId = _userService.GetUserIdWithUserName(User.Identity.Name);
        ViewBag.UserId = userId;
        ViewData["Provinces"] = new SelectList(_userService.GetAllProvince(), "ProvinceId", "ProvinceName", address.ProvinceId);
        ViewData["Cities"] = new SelectList(_userService.GetProvincCities(address.ProvinceId), "CityId", "CityName", address.CityId);

        var userAddress = new AddressViewModel
        {
            CityId = address.CityId,
            FullAddress = address.FullAddress,
            FullName = address.FullName,
            HouseNumber = address.HouseNumber,
            PostCode = address.PostCode,
            ProvinceId = address.ProvinceId,
            UserId = userId,
            UserAddressId = address.UserAddressId
        };

        return View(userAddress);
    }
    [HttpPost]
    public IActionResult EditAddress(int addressId, AddressViewModel editAddress)
    {
        int userId = _userService.GetUserIdWithUserName(User.Identity.Name);
        ViewBag.UserId = userId;
        ViewData["Provinces"] = new SelectList(_userService.GetAllProvince(), "ProvinceId", "ProvinceName", editAddress.ProvinceId);
        ViewData["Cities"] = new SelectList(_userService.GetProvincCities(editAddress.ProvinceId), "CityId", "CityName", editAddress.CityId);

        if (!ModelState.IsValid)
            return View();

        var address = new UserAddress
        {
            UserAddressId = editAddress.UserAddressId,
            CityId = editAddress.CityId,
            FullAddress = editAddress.FullAddress,
            FullName = editAddress.FullName,
            HouseNumber = editAddress.HouseNumber,
            PostCode = editAddress.PostCode,
            ProvinceId = editAddress.ProvinceId,
            UserId = userId
        };
        _userService.UpdateAddress(address);
        return RedirectToAction("Index");
    }

    public IActionResult DeleteAddress(int Id)
    {
        UserAddress address = _userService.GetUserAddressWithAddressId(Id);
        if (address == null)
        {
            return NotFound();
        }

        _userService.DeleteAddress(address);

        return Json(new { success = true, message = "حذف با موفقیت انجام شد" });
    }
    // AJAX method to get cities based on selected province
    public JsonResult GetCities(int provinceId)
    {
        var cities = _userService.GetProvincCities(provinceId).Select(c => new { c.CityId, c.CityName }).ToList();
        return Json(cities);
    }
}