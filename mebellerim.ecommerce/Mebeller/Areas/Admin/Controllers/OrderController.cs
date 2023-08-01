using System;
using System.Linq;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.ViewModels.Product;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Data.Utilities;
using Mebeller.Models.Product;
using Mebeller.Models.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class OrderController : Controller
    {
        private readonly IProductService _productService;
        public OrderController(IProductService productService) => _productService = productService;

        [HttpGet("/Admin/Orders")]
        public async Task<IActionResult> Index(int pageNumber = 1, string search = null)
        {
            var ordersViewModels = await _productService.GetOrdersListViewAsync();

            ordersViewModels = !string.IsNullOrEmpty(search)
                ? ordersViewModels?.Where(p =>
                      (p.OrderName != null && p.OrderName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                      (p.OrderNote != null && p.OrderNote.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                      (p.OrderId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase) && p.NotEmpty))
                  ?? Enumerable.Empty<OrderViewModel>()
                : ordersViewModels.Where(p => p.NotEmpty);

            if (!ordersViewModels.Any())
            {
                ViewData["isEmpty"] = true;
                return View();
            }

            var page = new Paging<OrderViewModel>(ordersViewModels, 6, pageNumber);
            if (pageNumber < page.FirstPage || pageNumber > page.LastPage)
            {
                return NotFound();
            }

            var ordersViewModelsPage = page.QueryResult;
            ViewData["pageNumber"] = pageNumber;
            ViewData["firstPage"] = page.FirstPage;
            ViewData["lastPage"] = page.LastPage;
            ViewData["prevPage"] = page.PreviousPage;
            ViewData["nextPage"] = page.NextPage;
            ViewData["search"] = search;
            ViewData["isEmpty"] = false;
            return View(ordersViewModelsPage);
        }


        [HttpGet("/Admin/Orders/EditOrder")]
        public async Task<IActionResult> EditOrder(int orderId)
        {
            var order = await _productService.GetOrderForEditAsync(orderId);
            if (order == null)
            {
                return NotFound();
            }

            return View(order);
        }

        [HttpPost("/Admin/Orders/EditOrder")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditOrder(Order model)
        {
            ModelState.Remove("OwnerUser");
            return ModelState.IsValid
                ? await _productService.UpdateOrderAsync(model) ? RedirectToAction("Index") : NotFound()
                : NotFound();
        }

        [HttpGet("/Admin/Orders/DeleteOrder")]
        public async Task<IActionResult> DeleteOrder(int orderId)
        {
            var result =
                await _productService
                    .DeleteOrderByIdAsync(orderId);

            if (result)
                return RedirectToAction("Index");

            return NotFound();
        }

        [HttpGet("/Admin/Orders/Discounts")]
        public async Task<IActionResult> Discounts(int pageNumber = 1, string search = null)
        {
            var discounts = await _productService.GetDiscountsAsync();

            if (!string.IsNullOrEmpty(search))
            {
                discounts = discounts.Where(p => p.DiscountCode.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                                 p.DiscountId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase));
            }

            discounts = discounts.Where(p => !p.IsTrash);

            var filteredDiscounts = discounts.ToList();

            if (filteredDiscounts.Any())
            {
                const int pageSize = 6;
                var page = new Paging<Discount>(filteredDiscounts, pageSize, pageNumber);

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

        [HttpGet("/Admin/Orders/AddDiscount")]
        public IActionResult AddDiscount() => View();

        [HttpPost("/Admin/Orders/AddDiscount")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddDiscount(Discount model)
        {
            switch (ModelState.IsValid)
            {
                case true:
                {
                    var result =
                        await _productService
                            .AddDiscountAsync(model);

                    switch (result)
                    {
                        case AddUpdateDiscountResult.Successful:
                            ModelState.Clear();

                            ViewData["SuccessMessage"] = "The discount code was successfully added.";

                            return View();
                    }

                    ModelState.AddModelError("",
                        result == AddUpdateDiscountResult.DiscountCodeExist
                            ? "This discount code is currently available"
                            : "A problem occurred when the discount code was added.");
                    break;
                }
            }

            return View(model);
        }

        [HttpGet("/Admin/Orders/EditDiscount")]
        public async Task<IActionResult> EditDiscount(int discountId)
        {
            var discount = await _productService.GetDiscountAsync(discountId);
            if (discount == null)
            {
                return NotFound();
            }

            TempData["discountId"] = discountId;
            return View(discount);
        }

        [HttpPost("/Admin/Orders/EditDiscount")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDiscount(Discount model)
        {
            model.DiscountId = Convert.ToInt32(TempData["discountId"]);
            if (!ModelState.IsValid)
            {
                TempData.Keep("discountId");
                return View(model);
            }

            var result = await _productService.UpdateDiscountAsync(model);
            if (result == AddUpdateDiscountResult.Successful) return RedirectToAction("Discounts");
            ModelState.AddModelError("",
                result == AddUpdateDiscountResult.DiscountCodeExist
                    ? "This discount code already exists."
                    : "There was a problem updating the discount code.");
            TempData.Keep("discountId");
            return View(model);
        }

        [HttpGet("/Admin/Orders/DeleteDiscount")]
        public async Task<IActionResult> MoveDiscountToTrash(int discountId) =>
            await _productService.MoveDiscountToTrashAsync(discountId) ? RedirectToAction("Discounts") : NotFound();
    }
}