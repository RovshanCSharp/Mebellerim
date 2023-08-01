using System.Threading.Tasks;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Models.ViewModels.Home;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Controllers;

public class HomeController : Controller
{
    private readonly IProductService _productService;
    private readonly IMediaService _mediaService;

    public HomeController(IProductService productService, IMediaService mediaService)
    {
        _productService = productService;
        _mediaService = mediaService;
    }

    [HttpGet("/")]
    public async Task<IActionResult> Index()
    {
        var indexViewModel = await GetIndexViewModel();
        return View(indexViewModel);
    }

    [HttpGet("/Slider")]
    public async Task<IActionResult> Slider()
    {
        var indexViewModel = await GetIndexViewModel();
        return View(indexViewModel);
    }
    
    [HttpGet("/Tabsy")]
    public async Task<IActionResult> Tabsy()
    {
        var indexViewModel = await GetIndexViewModel();
        return View(indexViewModel);
    }

    [HttpGet("/OnePage")]
    public async Task<IActionResult> OnePage()
    {
        var GetViewModel = await this.GetViewModel();
        return View(GetViewModel);
    }

    private async Task<IndexViewModel> GetIndexViewModel()
    {
        return new IndexViewModel()
        {
            Banners = await _mediaService.GetBannersAsync(),
            Orders = await _productService.GetLoggedInUserOpenOrderAsync(),
            BlogPosts = await _mediaService.GetBlogsAsync(),
            Products = await _productService.GetProductsAsync()
        };
    }

    private async Task<FullScreenViewModel> GetViewModel()
    {
        return new FullScreenViewModel()
        {
            Banners = await _mediaService.GetBannersAsync(),
            CategoriesTreeViews = await _productService.GetCategoriesTreeViewsAsync(),
            Orders = await _productService.GetLoggedInUserOpenOrderAsync(),
            BlogPosts = await _mediaService.GetBlogsAsync(),
            Products = await _productService.GetProductsAsync()
        };
    }
}