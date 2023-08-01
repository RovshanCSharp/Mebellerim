using System.Threading.Tasks;
using Mebeller.Data.Services.Interfaces;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Controllers;

public class ManageErrors : Controller
{
    private readonly IPageService _pageService;
    public ManageErrors(IPageService pageService) => _pageService = pageService;

    public async Task<IActionResult> Error404()
    {
        // Get the original path from the HTTP context
        var originalPath = HttpContext.Features.Get<IStatusCodeReExecuteFeature>()?.OriginalPath[1..]; 
        
        // Get the page with the given path address
        var page = await _pageService.GetPageByPathAddressAsync(originalPath); 
        
        // If the page exists, render it. Otherwise, render a default view
        return page != null ? View("/Views/Home/Page.cshtml", page) : View();
    }
}