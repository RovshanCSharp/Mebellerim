using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.ViewModels.Product;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Data.Utilities;
using Mebeller.Models.Product;
using Mebeller.Models.ViewModels.Home;
using Mebeller.Models.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Controllers
{
    [Authorize]
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        public ProductController(IProductService productService) => _productService = productService;

        [HttpGet("/Products/")]
        public async Task<IActionResult> Index(int pageNumber = 1, string sortBy = null, string search = null,
            IEnumerable<int> selectedCategories = null, int minimumPrice = 0, int maximumPrice = 0)
        {
            var products = await _productService.GetProductsAsync();
            products = string.IsNullOrEmpty(search) switch
            {
                false => products.Where(p => p.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                             p.ProductId.ToString()
                                                 .Contains(search, StringComparison.OrdinalIgnoreCase)),
                _ => products
            };

            if (selectedCategories != null && selectedCategories.Any())
            {
                products = products.Where(p => selectedCategories.Any(t => p.Categories.Any(c => c.CategoryId == t)));
            }

            if (minimumPrice != 0 || maximumPrice != 0)
            {
                products = products.Where(p =>
                {
                    var result = p.ProductQuantityInStock != 0 &&
                                 (p.ProductPrice >= minimumPrice && p.ProductPrice <= maximumPrice);
                    return result;
                });
            }

            ProductsViewModel productsViewModel;
            var categoriesTreeViews = await _productService.GetCategoriesTreeViewsAsync();
            ViewData["search"] = search;
            ViewData["minimumPrice"] = minimumPrice;
            ViewData["maximumPrice"] = maximumPrice;
            if (!products.Any())
            {
                ViewData["isEmpty"] = true;
                productsViewModel = new ProductsViewModel()
                {
                    CategoriesTreeViews = categoriesTreeViews,
                    SelectedCategories = selectedCategories,
                    SortBy = sortBy
                };
                return View(productsViewModel);
            }

            products = sortBy == "Newest" || string.IsNullOrEmpty(sortBy)
                ? products.OrderByDescending(p => Convert.ToBoolean(p.ProductQuantityInStock))
                    .ThenByDescending(p => p.RegistrationTime)
                : sortBy switch
                {
                    "Price-Cheapest" => products.OrderBy(p => Convert.ToBoolean(p.ProductQuantityInStock))
                        .ThenBy(p => p.ProductPrice),
                    "Price-Most-Expensive" => products
                        .OrderByDescending(p => Convert.ToBoolean(p.ProductQuantityInStock))
                        .ThenByDescending(p => p.ProductPrice),
                    "Popularity" => products.OrderByDescending(p => Convert.ToBoolean(p.ProductQuantityInStock))
                        .ThenByDescending(p => p.ProductHits),
                    _ => products
                };

            var page = new Paging<Product>(products, 6, pageNumber);
            if (pageNumber < page.FirstPage || pageNumber > page.LastPage)
            {
                return NotFound();
            }

            var productsPage = page.QueryResult;
            ViewData["pageNumber"] = pageNumber;
            ViewData["firstPage"] = page.FirstPage;
            ViewData["lastPage"] = page.LastPage;
            ViewData["prevPage"] = page.PreviousPage;
            ViewData["nextPage"] = page.NextPage;
            ViewData["isEmpty"] = false;
            productsViewModel = new ProductsViewModel()
            {
                Products = productsPage,
                CategoriesTreeViews = categoriesTreeViews,
                SelectedCategories = selectedCategories,
                SortBy = sortBy
            };
            return View(productsViewModel);
        }

        [HttpGet("/ProductList/")]
        public async Task<IActionResult> ProductList(int pageNumber = 1, string sortBy = null, string search = null,
            IEnumerable<int> selectedCategories = null, int minimumPrice = 0, int maximumPrice = 0)
        {
            var products = await _productService.GetProductsAsync();
            products = string.IsNullOrEmpty(search) switch
            {
                false => products.Where(p =>
                    p.ProductName.Contains(search) || p.Categories.Any(t => t.CategoryName.Contains(search))),
                _ => products
            };

            if (selectedCategories != null && selectedCategories.Any())
            {
                products = products.Where(p => selectedCategories.Any(t => p.Categories.Any(c => c.CategoryId == t)));
            }

            if (minimumPrice != 0 || maximumPrice != 0)
            {
                products = products.Where(p =>
                {
                    var result = p.ProductQuantityInStock != 0 &&
                                 (p.ProductPrice >= minimumPrice && p.ProductPrice <= maximumPrice);
                    return result;
                });
            }

            ProductsViewModel productsViewModel;
            var categoriesTreeViews = await _productService.GetCategoriesTreeViewsAsync();
            ViewData["search"] = search;
            ViewData["minimumPrice"] = minimumPrice;
            ViewData["maximumPrice"] = maximumPrice;
            if (!products.Any())
            {
                ViewData["isEmpty"] = true;
                productsViewModel = new ProductsViewModel()
                {
                    CategoriesTreeViews = categoriesTreeViews,
                    SelectedCategories = selectedCategories,
                    SortBy = sortBy
                };
                return View(productsViewModel);
            }

            products = sortBy == "Newest" || string.IsNullOrEmpty(sortBy)
                ? products.OrderByDescending(p => Convert.ToBoolean(p.ProductQuantityInStock))
                    .ThenByDescending(p => p.RegistrationTime)
                : sortBy switch
                {
                    "Price-Cheapest" => products.OrderBy(p => Convert.ToBoolean(p.ProductQuantityInStock))
                        .ThenBy(p => p.ProductPrice),
                    "Price-Most-Expensive" => products
                        .OrderByDescending(p => Convert.ToBoolean(p.ProductQuantityInStock))
                        .ThenByDescending(p => p.ProductPrice),
                    "Popularity" => products.OrderByDescending(p => Convert.ToBoolean(p.ProductQuantityInStock))
                        .ThenByDescending(p => p.ProductHits),
                    _ => products
                };

            var page = new Paging<Product>(products, 6, pageNumber);
            if (pageNumber < page.FirstPage || pageNumber > page.LastPage) return NotFound();
            var productsPage = page.QueryResult;
            ViewData["pageNumber"] = pageNumber;
            ViewData["firstPage"] = page.FirstPage;
            ViewData["lastPage"] = page.LastPage;
            ViewData["prevPage"] = page.PreviousPage;
            ViewData["nextPage"] = page.NextPage;
            ViewData["isEmpty"] = false;
            productsViewModel = new ProductsViewModel()
            {
                Products = productsPage,
                CategoriesTreeViews = categoriesTreeViews,
                SelectedCategories = selectedCategories,
                SortBy = sortBy
            };
            return View(productsViewModel);
        }

        [HttpGet("Categories")]
        public async Task<IActionResult> Categories(IEnumerable<int> selectedCategories = null, int pageNumber = 1,
            string search = null)
        {
            var categoriesQuery = await _productService.GetCategoriesAsync();
            categoriesQuery = string.IsNullOrEmpty(search) switch
            {
                false => categoriesQuery?.Where(p =>
                             (p.CategoryName != null &&
                              p.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                             (p.ParentCategory != null &&
                              p.ParentCategory.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase))) ??
                         Enumerable.Empty<Category>(),
                _ => categoriesQuery
            };
            if (selectedCategories != null && selectedCategories.Any())
            {
                categoriesQuery = categoriesQuery.Where(p =>
                    selectedCategories.Any(t => p.Categories.Any(c => c.CategoryId == t)));
            }

            var categories = categoriesQuery.ToList();
            var categoriesTreeViews = await _productService.GetCategoriesTreeViewsAsync();
            var categoryViewModel = new IndexViewModel()
            {
                CategoriesTreeViews = categoriesTreeViews, SelectedCategories = selectedCategories,
            };
            ViewData["search"] = search;
            if (!categories.Any())
            {
                ViewData["isEmpty"] = true;
                return View(categoryViewModel);
            }

            var page = new Paging<Category>(categories, 6, pageNumber);
            if (pageNumber < page.FirstPage || pageNumber > page.LastPage)
            {
                return NotFound();
            }

            ViewData["pageNumber"] = pageNumber;
            ViewData["firstPage"] = page.FirstPage;
            ViewData["lastPage"] = page.LastPage;
            ViewData["prevPage"] = page.PreviousPage;
            ViewData["nextPage"] = page.NextPage;
            ViewData["isEmpty"] = false;
            categoryViewModel.Categories = page.QueryResult;
            return View(categoryViewModel);
        }

        [HttpGet("/Products/{productId}")]
        public async Task<IActionResult> ProductDetails(int productId)
        {
            var product = await _productService.GetProductWithDetailsAsync(productId);
            if (product == null)
            {
                return NotFound();
            }

            await _productService.AddHitsToProductAsync(product);
            product.Comments = product.Comments.Where(p => p.IsConfirmed && p.ParentComment == null).ToList();
            var model = new ProductsViewModel()
            {
                Products = await _productService.GetLoggedUserFavoriteProductsAsync(),
                Orders = await _productService.GetLoggedInUserOpenOrderAsync(),
                Product = product,
            };
            return View(model);
        }

        [HttpGet("/Shopping-Cart")]
        public async Task<IActionResult> ShoppingCart() => View(await _productService.GetLoggedInUserOpenOrderAsync());

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddProductToCart(int productId, int requestedQuantity, int productVariationId)
        {
            var product = await _productService.GetProductWithDetailsAsync(productId);
            if (product == null) return NotFound();
            var result = await _productService.AddProductToCartAsync(product, requestedQuantity, productVariationId);
            switch (result)
            {
                case AddProductToCartResult.Successful:
                    return RedirectToAction("ShoppingCart");
                case AddProductToCartResult.OutOfStock:
                {
                    ViewData["OutOfStockError"] = "Out of stock";
                    product.Comments = product.Comments.Where(p => p.IsConfirmed && p.ParentComment == null).ToList();
                    var model = new ProductsViewModel { Product = product, };
                    return View("ProductDetails", model);
                }
                default:
                    return NotFound();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCart(IEnumerable<int> orderDetailsQuantities)
        {
            if (ModelState.IsValid)
            {
                var loggedUserOpenOrder = await _productService.GetLoggedInUserOpenOrderAsync();
                var result = await _productService.UpdateCartAsync(loggedUserOpenOrder, orderDetailsQuantities);
                if (result)
                {
                    ViewData["Message"] = "Cart updated successfully";
                    return View("ShoppingCart", loggedUserOpenOrder);
                }
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveProductFormCart(int orderDetailsId) =>
            ModelState.IsValid && await _productService.DeleteOrderDetailsByIdAsync(orderDetailsId)
                ? RedirectToAction("ShoppingCart")
                : NotFound();

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApplyDiscountOnCart(string discountCode)
        {
            var loggedUserOpenOrder =
                await _productService
                    .GetLoggedInUserOpenOrderAsync();

            if (ModelState.IsValid)
            {
                var result =
                    await _productService
                        .AddDiscountToCartAsync(loggedUserOpenOrder, discountCode);

                ViewData["Message"] = result switch
                {
                    AddDiscountToCartResult.Successful => "The discount was successfully applied",
                    AddDiscountToCartResult.AlreadyApplied => "Discount code has already been applied",
                    _ => "Discount code not valid"
                };
            }

            return View("ShoppingCart", loggedUserOpenOrder);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProceedToCheckout(IEnumerable<int> orderDetailsQuantities)
        {
            return ModelState.IsValid &&
                   await _productService.UpdateCartAsync(await _productService.GetLoggedInUserOpenOrderAsync(),
                       orderDetailsQuantities)
                ? RedirectToAction("CartCheckOut")
                : NotFound();
        }

        [HttpGet("/Cart-CheckOut")]
        public async Task<IActionResult> CartCheckOut() =>
            await _productService.GetLoggedUserCartCheckOutAsync() == null
                ? NotFound()
                : View(await _productService.GetLoggedUserCartCheckOutAsync());

        [HttpPost("/Cart-CheckOut")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CartCheckOut(CartCheckOutViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _productService.PaymentProcessAsync(model);
                return !string.IsNullOrEmpty(result) ? Redirect(result) : NotFound();
            }

            return View(model);
        }

        [HttpGet("/Order-Confirmation")]
        public async Task<IActionResult> OrderConfirmation()
        {
            var loggedUserOpenOrder = await _productService.GetLoggedInUserOpenOrderAsync();
            if (loggedUserOpenOrder == null)
            {
                return NotFound();
            }

            var result = await _productService.OrderConfirmationAsync(loggedUserOpenOrder);
            if (result)
            {
                ViewData["HeaderTitle"] = "Order Confirmation";
                ViewData["Message"] = "Thanks, your order was received.";
                var orderViewModel = _productService.GetConfirmedOrderInvoiceAsync(loggedUserOpenOrder);
                return View(orderViewModel);
            }

            return NotFound();
        }

        [HttpGet("/Order-Tracking")]
        public IActionResult OrderTracking() => View();

        [HttpPost("/Order-Tracking")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> OrderTracking(OrderTrackingViewModel model)
        {
            if (ModelState.IsValid)
            {
                var orderViewModel = await _productService.OrderTrackingByUserEmailAsync(model.Email, model.OrderId);
                if (orderViewModel != null)
                {
                    ModelState.Clear();
                    model = new OrderTrackingViewModel() { Order = orderViewModel };
                    return View(model);
                }

                ModelState.AddModelError("",
                    "Sorry, order not found. If you have trouble finding your order details, please contact us.");
            }

            return View(model);
        }

        [HttpGet("/Products/AddToFavoriteProducts")]
        public async Task<IActionResult> AddProductToUserFavoriteProducts(int favoriteProductId) =>
            await _productService.AddProductByIdToLoggedUserFavoriteProductsAsync(favoriteProductId)
                ? Redirect(Request.Headers["Referer"].ToString())
                : NotFound();

        [HttpGet("/Products/RemoveFromFavoriteProducts")]
        public async Task<IActionResult> RemoveProductFromUserFavoriteProducts(int favoriteProductId)
        {
            var result = await _productService.RemoveProductByIdFromLoggedUserFavoriteProductsAsync(favoriteProductId);
            if (result)
            {
                var returnUrl = Request.Headers["Referer"].ToString();
                return Redirect(returnUrl);
            }

            return NotFound();
        }
    }
}