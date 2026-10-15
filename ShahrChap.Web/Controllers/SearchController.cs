using Microsoft.AspNetCore.Mvc;
using ShahrChap.Core.Services.Interfaces;

namespace ShahrChap.Web.Controllers
{
    public class SearchController : Controller
    {
        private readonly ISearchService _search;
        public SearchController(ISearchService search) => _search = search;

        //Result page: /Search?q=
        [HttpGet("/Search")]
        public IActionResult Index(string? q, int? groupId, string? sort, int page = 1)
        {
            var model = _search.Search(q, groupId, sort, page);
            return View(model);
        }
        // اتوکامپلیت: /Search/Suggest?q=کار  → JSON
        [HttpGet("/Search/Suggest")]
        [ResponseCache(Duration = 30, Location = ResponseCacheLocation.Client)]
        public IActionResult Suggest(string? q)
        {
            var result = _search.Suggest(q);

            // آدرس‌ها اینجا ساخته می‌شن تا به مسیرهای پروژه وابسته باشن
            foreach (var p in result.Products)
                p.Url = Url.Action("ShowProduct", "Product", new { id = p.Id }) ?? "#";
            foreach (var g in result.Groups)
                g.Url = Url.Action("Index", "Search", new { q = result.Query, groupId = g.Id }) ?? "#";

            return Json(result);
        }
    }
}
