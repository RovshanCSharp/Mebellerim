using System;
using System.Linq;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.ViewModels.Page;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Data.Utilities;
using Mebeller.Models.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class PageController : Controller
    {
        private readonly IPageService _pageService;
        public PageController(IPageService pageService) => _pageService = pageService;

        [HttpGet("/Admin/Pages")]
        public async Task<IActionResult> Index(int pageNumber = 1, string search = null)
        {
            var pages = await _pageService.GetPagesAsync();

            if (!string.IsNullOrEmpty(search))
            {
                pages = pages.Where(p => p.PageTitle.Contains(search) || p.PagePathAddress.Contains(search));
            }

            if (pages.Any())
            {
                var page = new Paging<Page>(pages, 6, pageNumber);
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


        [HttpGet("/Admin/Pages/AddPage")]
        public IActionResult CreatePage() => View();

        [HttpPost("/Admin/Pages/AddPage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreatePage(AddPageViewModel model)
        {
            if (!ModelState.IsValid) return View(model);
            var result = await _pageService.CreatePageAsync(model);
            switch (result)
            {
                case PageCreateUpdateResult.Successful:
                    ModelState.Clear();
                    ViewData["SuccessMessage"] = "The page was created successfully.";
                    return View();
                case PageCreateUpdateResult.PathAddressExist:
                    ModelState.AddModelError("", "This page path address already exists.");
                    break;
                case PageCreateUpdateResult.Failed:
                    ModelState.AddModelError("", "An error occurred while creating the page.");
                    break;
                default: throw new ArgumentOutOfRangeException();
            }

            return View(model);
        }

        [HttpGet("/Admin/Pages/EditPage")]
        public async Task<IActionResult> EditPage(int pageId)
        {
            var page = await _pageService.GetPageAsync(pageId);
            if (page == null)
            {
                return NotFound();
            }

            TempData["pageId"] = pageId;
            return View(new EditPageViewModel
            {
                PageId = page.PageId,
                PageTitle = page.PageTitle,
                PagePathAddress = page.PagePathAddress,
                PageDescription = page.PageDescription
            });
        }

        [HttpPost("/Admin/Pages/EditPage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditPage(EditPageViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _pageService.UpdatePageAsync(model);
                switch (result)
                {
                    case PageCreateUpdateResult.Successful:
                        return RedirectToAction("Index");
                    default:
                        ModelState.AddModelError("",
                            result == PageCreateUpdateResult.PathAddressExist
                                ? "This page address already exists."
                                : "An error occurred while updating the page.");
                        break;
                }
            }

            TempData.Keep("pageId");
            return View(model);
        }

        [HttpGet("Admin/Pages/DeletePage")]
        public async Task<IActionResult> DeletePage(int pageId) => await _pageService.DeletePageByIdAsync(pageId)
            ? RedirectToAction("Index")
            : NotFound();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsPagePathAddressExistForAdd(string pagePathAddress) =>
            await _pageService.IsPagePathAddressExistForAddJsonResultAsync(pagePathAddress);

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IsPagePathAddressExistForEdit(string pagePathAddress, int pageId) =>
            await _pageService.IsPagePathAddressExistForEditJsonResultAsync(pagePathAddress, pageId);
    }
}