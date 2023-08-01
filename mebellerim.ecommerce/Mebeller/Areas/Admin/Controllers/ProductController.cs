using System;
using System.Linq;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.ViewModels.Product;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Data.Utilities;
using Mebeller.Models.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Product = Mebeller.Models.Product.Product;

namespace Mebeller.Areas.Admin.Controllers;

[Authorize(Roles = "Admin")]
[Area("Admin")]
public class ProductController : Controller
{
    private readonly IProductService _productService;
    public ProductController(IProductService productService) => _productService = productService;

    [HttpGet("/Admin/Products")]
    public async Task<IActionResult> Index(int pageNumber = 1, string search = null)
    {
        var products = await _productService.GetProductsAsync();
        products = string.IsNullOrEmpty(search) switch
        {
            false => products.Where(p => p.ProductName.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                                         p.ProductId.ToString().Contains(search, StringComparison.OrdinalIgnoreCase)),
            _ => products
        };

        if (!products.Any())
        {
            ViewData["isEmpty"] = true;
            return View();
        }

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
        ViewData["search"] = search;
        ViewData["isEmpty"] = false;
        return View(productsPage);
    }

    [HttpGet("/Admin/Products/AddProduct")]
    public async Task<IActionResult> AddProduct() => View(new AddEditProductViewModel
    {
        ProductCategories = (await _productService.GetCategoriesTreeViewForAddAsync()).Skip(1),
    });

    [HttpPost("/Admin/Products/AddProduct")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddProduct(AddEditProductViewModel model)
    {
        switch (ModelState.IsValid)
        {
            case true when await _productService.AddProductAsync(model):
            {
                ModelState.Clear();
                var returnProductCategories = await _productService.GetCategoriesTreeViewForAddAsync();
                model = new AddEditProductViewModel
                {
                    ProductCategories = returnProductCategories.Skip(1)
                };
                ViewData["SuccessMessage"] = "Product added successfully.";
                return View(model);
            }
            case true:
                ModelState.AddModelError("", "There was an error adding the product.");
                break;
        }

        model.ProductCategories = (await _productService.GetCategoriesTreeViewForAddAsync()).Skip(1);
        return View(model);
    }

    [HttpGet("/Admin/Products/EditProduct")]
    public async Task<IActionResult> EditProduct(int productId)
    {
        var product = await _productService.GetProductWithDetailsAsync(productId);
        if (product == null)
        {
            return NotFound();
        }

        var productCategories = await _productService.GetCategoriesTreeViewForAddAsync();
        foreach (var productCategory in productCategories)
            productCategory.Selected =
                product.Categories.Any(p => p.CategoryId == Convert.ToInt32(productCategory.Value));
        var model = new AddEditProductViewModel
        {
            ProductName = product.ProductName,
            ProductDescription = product.ProductDescription,
            ProductPrice = product.ProductPrice,
            ProductQuantityInStock = product.ProductQuantityInStock,
            ProductCategories = productCategories.Skip(1),
            InformationNames = product.Informations.Select(p => p.Name),
            InformationValues = product.Informations.Select(p => p.Value),
            CurrentProductImages = product.Images,
            UserName = product.UserName,
            UserEmail = product.UserEmail,
            UserPhoneNumber = product.UserPhoneNumber,
        };
        TempData["productId"] = productId;
        return View(model);
    }

    [HttpPost("/Admin/Products/EditProduct")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProduct(AddEditProductViewModel model)
    {
        model.ProductId = Convert.ToInt32(TempData["productId"]);
        switch (ModelState.IsValid)
        {
            case true when await _productService.UpdateProductAsync(model):
                return RedirectToAction("Index");
            case true:
                ModelState.AddModelError("", "There was an error updating the product.");
                break;
        }

        var product = await _productService.GetProductWithDetailsAsync(model.ProductId);
        var productCategories = await _productService.GetCategoriesTreeViewForAddAsync();
        foreach (var productCategory in productCategories)
            productCategory.Selected =
                product.Categories.Any(p => p.CategoryId == Convert.ToInt32(productCategory.Value));
        model.ProductCategories = productCategories.Skip(1);
        model.CurrentProductImages = product.Images;
        TempData.Keep("productId");
        return View(model);
    }

    [HttpGet("/Admin/Products/DeleteProduct")]
    public async Task<IActionResult> DeleteProduct(int productId) =>
        await _productService.DeleteProductByIdAsync(productId) ? RedirectToAction("Index") : NotFound();

    [HttpGet("/Admin/Products/Categories")]
    public async Task<IActionResult> Categories(int pageNumber = 1, string search = null)
    {
        var categories = await _productService.GetCategoriesAsync();
        categories = string.IsNullOrEmpty(search) switch
        {
            false => categories?.Where(p =>
                         (p.CategoryName != null &&
                          p.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase)) ||
                         (p.ParentCategory != null &&
                          p.ParentCategory.CategoryName.Contains(search, StringComparison.OrdinalIgnoreCase))) ??
                     Enumerable.Empty<Category>(),
            _ => categories
        };

        if (categories.Any())
        {
            var page = new Paging<Category>(categories, 6, pageNumber);
            if (pageNumber < page.FirstPage || pageNumber > page.LastPage) return NotFound();
            var categoriesPage = page.QueryResult;
            ViewData["pageNumber"] = pageNumber;
            ViewData["firstPage"] = page.FirstPage;
            ViewData["lastPage"] = page.LastPage;
            ViewData["prevPage"] = page.PreviousPage;
            ViewData["nextPage"] = page.NextPage;
            ViewData["search"] = search;
            ViewData["isEmpty"] = false;
            return View(categoriesPage);
        }

        ViewData["isEmpty"] = true;
        return View();
    }

    [HttpGet("/Admin/Products/AddCategory")]
    public async Task<IActionResult> AddCategory() => View(new AddEditCategoryViewModel
    {
        AllCategories = await _productService.GetCategoriesTreeViewForAddAsync()
    });

    [HttpPost("/Admin/Products/AddCategory")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCategory(AddEditCategoryViewModel model)
    {
        switch (ModelState.IsValid)
        {
            case true when await _productService.AddCategoryAsync(model):
                ModelState.Clear();
                model = new AddEditCategoryViewModel()
                {
                    AllCategories = await _productService.GetCategoriesTreeViewForAddAsync()
                };
                ViewData["SuccessMessage"] = "Category added successfully.";
                return View(model);
            case true:
                ModelState.AddModelError("", "There was an error adding the category.");
                break;
        }

        model.AllCategories = await _productService.GetCategoriesTreeViewForAddAsync();
        return View(model);
    }

    [HttpGet("/Admin/Products/EditCategory")]
    public async Task<IActionResult> EditCategory(int categoryId)
    {
        var category = await _productService.GetCategoryAsync(categoryId);
        if (category == null) return NotFound();
        var model = new AddEditCategoryViewModel()
        {
            CategoryName = category.CategoryName,
            CategoryDescription = category.CategoryDescription,
            AllCategories = await _productService.GetCategoriesTreeViewForEditAsync(category),
            ParentCategoryId = category.ParentCategory?.CategoryId ?? -1
        };
        TempData["categoryId"] = categoryId;
        return View(model);
    }

    [HttpPost("/Admin/Products/EditCategory")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditCategory(AddEditCategoryViewModel model)
    {
        model.CategoryId = Convert.ToInt32(TempData["categoryId"]);
        switch (ModelState.IsValid)
        {
            case true when await _productService.UpdateCategoryAsync(model):
                return RedirectToAction("Categories");
            case true:
                ModelState.AddModelError("", "There was an error updating the category.");
                break;
        }

        var category = await _productService.GetCategoryAsync(model.CategoryId);
        model.AllCategories = await _productService.GetCategoriesTreeViewForEditAsync(category);
        model.ParentCategoryId = category.ParentCategory.CategoryId;
        TempData.Keep("categoryId");
        return View(model);
    }

    [HttpGet("/Admin/Products/DeleteCategory")]
    public async Task<IActionResult> DeleteCategory(int categoryId) =>
        await _productService.DeleteCategoryByIdAsync(categoryId) ? RedirectToAction("Categories") : NotFound();
}