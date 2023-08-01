using System.Linq;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Data.Utilities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class ReportController : Controller
{
    private readonly IVisitorService _visitorService;
    public ReportController(IVisitorService visitorService) => _visitorService = visitorService;

    [HttpGet("/Admin/Visitors")]
    public async Task<IActionResult> Visitors(int pageNumber = 1, string search = null)
    {
        var visitors = await _visitorService.GetVisitorsAsync();
        var filteredVisitors = string.IsNullOrEmpty(search)
            ? visitors
            : visitors.Where(p => p.VisitorIpAddress.Contains(search));
        if (filteredVisitors.Any())
        {
            var page = new Paging<Visitor>(filteredVisitors, 6, pageNumber);
            if (pageNumber < page.FirstPage || pageNumber > page.LastPage)
            {
                return NotFound();
            }

            ViewData["pageNumber"] = pageNumber;
            ViewData["firstPage"] = page.FirstPage;
            ViewData["lastPage"] = page.LastPage;
            ViewData["prevPage"] = page.PreviousPage;
            ViewData["nextPage"] = page.NextPage;
            ViewData["search"] = search;
            ViewData["isEmpty"] = false;
            return View(page.QueryResult);
        }

        ViewData["isEmpty"] = true;
        return View();
    }
}