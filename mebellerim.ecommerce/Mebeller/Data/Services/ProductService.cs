using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.ViewModels.Product;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Data.Utilities;
using Mebeller.Models.Product;
using Mebeller.Models.ViewModels.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Configuration;
using NuGet.Packaging;
using Stripe;
using Discount = Mebeller.Models.Product.Discount;
using Order = Mebeller.Models.Product.Order;
using Product = Mebeller.Models.Product.Product;

namespace Mebeller.Data.Services;

public class ProductService : IProductService
{
    private readonly IProductRepository _productRepository;
    private readonly IFileService _fileService;
    private readonly IConfiguration _configuration;
    private readonly IMediaService _mediaService;
    private readonly IAccountService _accountService;
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _accessor;

    public ProductService(IProductRepository productRepository, IFileService fileService, IMediaService mediaService,
        IAccountService accountService, LinkGenerator linkGenerator, IHttpContextAccessor accessor,
        IConfiguration configuration)
    {
        _productRepository = productRepository;
        _fileService = fileService;
        _mediaService = mediaService;
        _accountService = accountService;
        _linkGenerator = linkGenerator;
        _accessor = accessor;
        _configuration = configuration;
    }

    public async Task<IEnumerable<Product>> GetAvailableProductsAsync() =>
        await _productRepository.GetAvailableProductsAsync();

    public async Task<Product> GetProductAsync(int productId) =>
        await _productRepository.GetProductByIdAsync(productId);

    public async Task<IEnumerable<Product>> GetProductsAsync() => await _productRepository.GetAllProductsAsync();

    public async Task<Product> GetProductWithDetailsAsync(int productId) =>
        await _productRepository.GetProductWithDetailsAsync(productId);

    public async Task<int> GetProductsCountAsync() => await _productRepository.GetProductsCountAsync();

    public async Task<bool> AddProductAsync(AddEditProductViewModel productViewModel)
    {
        var product = new Product()
        {
            ProductName = productViewModel.ProductName,
            ProductDescription = productViewModel.ProductDescription,
            Categories = new List<Category>(),
            RegistrationTime = DateTime.Now,
            UserName = productViewModel.UserName,
            UserEmail = productViewModel.UserEmail,
            UserPhoneNumber = productViewModel.UserPhoneNumber
        };
        if (productViewModel.ProductCategoriesId.Any())
        {
            await AddProductCategoriesAsync(product, productViewModel.ProductCategoriesId);
        }

        if (productViewModel.ProductImagesFiles.Any())
        {
            await _fileService.AddProductImagesAsync(product, productViewModel.ProductImagesFiles);
        }

        if (productViewModel.InformationNames.Any() && productViewModel.InformationValues.Any())
        {
            await AddProductsInformationAsync(product, productViewModel.InformationNames,
                productViewModel.InformationValues);
        }

        await _productRepository.AddProductAsync(product);
        product.ProductPrice = productViewModel.ProductPrice;
        product.ProductQuantityInStock = productViewModel.ProductQuantityInStock;
        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<bool> UpdateProductAsync(AddEditProductViewModel productViewModel)
    {
        var product = await GetProductWithDetailsAsync(productViewModel.ProductId);
        product.ProductName = productViewModel.ProductName;
        product.ProductDescription = productViewModel.ProductDescription;
        if (productViewModel.ProductCategoriesId.Any())
        {
            await UpdateProductCategoriesAsync(product, productViewModel.ProductCategoriesId);
        }
        else
        {
            product.Categories.Clear();
        }

        if (productViewModel.ProductImagesFiles.Any())
        {
            await _fileService.AddProductImagesAsync(product, productViewModel.ProductImagesFiles);
        }

        if (productViewModel.DeletedProductImagesIds != null)
        {
            await _fileService.DeleteProductImagesByIdsAsync(productViewModel.DeletedProductImagesIds);
        }

        if (productViewModel.InformationNames.Any() && productViewModel.InformationValues.Any())
        {
            await UpdateProductsInformationAsync(product, productViewModel.InformationNames,
                productViewModel.InformationValues);
        }
        else
        {
            DeleteProductInformation(product);
        }

        product.ProductPrice = productViewModel.ProductPrice;
        product.ProductQuantityInStock = productViewModel.ProductQuantityInStock;
        _productRepository.UpdateProduct(product);
        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<bool> DeleteProductAsync(Product product)
    {
        if (product == null)
        {
            return false;
        }

        if (product.Images.Any())
        {
            _fileService.DeleteProductImages(product.Images);
        }

        if (product.Informations.Any())
        {
            DeleteProductInformation(product);
        }

        var productParentsComments = product.Comments.Where(p => p.ParentComment == null);
        _mediaService.DeleteCommentsByParents(productParentsComments);
        _productRepository.DeleteProduct(product);
        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<bool> DeleteProductByIdAsync(int productId) =>
        await DeleteProductAsync(await GetProductWithDetailsAsync(productId));

    public async Task<bool> AddHitsToProductAsync(Product product)
    {
        ++product.ProductHits;
        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<IEnumerable<Category>> GetCategoriesAsync() => await _productRepository.GetCategoriesAsync();

    public async Task<Category> GetCategoryAsync(int categoryId) =>
        await _productRepository.GetCategoryByIdAsync(categoryId);

    public async Task<bool> AddCategoryAsync(AddEditCategoryViewModel categoryViewModel)
    {
        var category = new Category()
        {
            CategoryName = categoryViewModel.CategoryName,
            CategoryDescription = categoryViewModel.CategoryDescription
        };
        if (categoryViewModel.ParentCategoryId != -1 &&
            await GetCategoryAsync(categoryViewModel.ParentCategoryId) != null)
        {
            category.ParentCategory = await GetCategoryAsync(categoryViewModel.ParentCategoryId);
        }

        await _productRepository.AddCategoryAsync(category);
        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<bool> UpdateCategoryAsync(AddEditCategoryViewModel categoryViewModel)
    {
        var category = await GetCategoryAsync(categoryViewModel.CategoryId);
        category.CategoryName = categoryViewModel.CategoryName;
        category.CategoryDescription = categoryViewModel.CategoryDescription;
        var parentCategory = categoryViewModel.ParentCategoryId == -1
            ? null
            : await GetCategoryAsync(categoryViewModel.ParentCategoryId);
        category.ParentCategory = parentCategory;
        _productRepository.UpdateCategory(category);
        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<bool> DeleteCategoryByIdAsync(int categoryId)
    {
        await DeleteCascadeCategoryByIdAsync(categoryId);
        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<IEnumerable<CategoryTreeView>> GetCategoriesTreeViewsAsync()
    {
        var categories = await _productRepository.GetCategoriesAsync();
        var items = new List<CategoryTreeView>();
        var stack = new Stack<(Category category, int level)>();

        // Add top-level categories to the stack
        var topLevelCategories = categories.Where(c => c.ParentCategory == null);
        foreach (var category in topLevelCategories)
        {
            stack.Push((category, 0));
        }

        while (stack.Count > 0)
        {
            var (currentCategory, level) = stack.Pop();

            // Create the category tree view for the current category
            var categoryTreeView = new CategoryTreeView
            {
                CategoryId = currentCategory.CategoryId,
                CategoryName = currentCategory.CategoryName,
                CategoryLevel = level,
                CategoryProductsCount = currentCategory.Products?.Count ?? 0,
                BlogsCount = currentCategory.BlogPosts?.Count ?? 0
            };
            items.Add(categoryTreeView);

            // Find subcategories of the current category
            var subCategories = categories.Where(c => c.ParentCategory?.CategoryId == currentCategory.CategoryId);
            foreach (var subCategory in subCategories)
            {
                stack.Push((subCategory, level + 1));
            }
        }

        return items;
    }

    public async Task<IEnumerable<SelectListItem>> GetCategoriesTreeViewForAddAsync()
    {
        var categories = await _productRepository.GetCategoriesAsync();
        var items = new List<SelectListItem> { new("Select", "-1") };
        var stack = new Stack<(Category category, int level)>();

        // Add top-level categories to the stack
        var topLevelCategories = categories.Where(c => c.ParentCategory == null);
        foreach (var category in topLevelCategories)
        {
            stack.Push((category, 0));
        }

        while (stack.Count > 0)
        {
            var (currentCategory, level) = stack.Pop();

            // Create the select list item for the current category
            var prefix = new string('─', level * 2);
            var item = new SelectListItem(prefix + " " + currentCategory.CategoryName,
                currentCategory.CategoryId.ToString());
            items.Add(item);

            // Find subcategories of the current category
            var subCategories = categories.Where(c => c.ParentCategory?.CategoryId == currentCategory.CategoryId);
            foreach (var subCategory in subCategories)
            {
                stack.Push((subCategory, level + 1));
            }
        }

        return items;
    }

    public async Task<IEnumerable<SelectListItem>> GetCategoriesTreeViewForEditAsync(Category selfCategory)
    {
        var categoriesTreeViewForAdd = await GetCategoriesTreeViewForAddAsync();
        return categoriesTreeViewForAdd.Where(p => p.Value != selfCategory.CategoryId.ToString());
    }

    public async Task<bool> AddProductByIdToLoggedUserFavoriteProductsAsync(int favoriteProductId)
    {
        var favoriteProduct = await GetProductAsync(favoriteProductId);
        if (favoriteProduct == null)
        {
            return false;
        }

        var loggedUser = await _accountService.GetLoggedUserWithFavoriteProductsAsync();
        if (loggedUser.FavoriteProducts.Contains(favoriteProduct))
        {
            return false;
        }

        loggedUser.FavoriteProducts.Add(favoriteProduct);
        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<bool> RemoveProductByIdFromLoggedUserFavoriteProductsAsync(int favoriteProductId)
    {
        var favoriteProduct = await GetProductAsync(favoriteProductId);
        if (favoriteProduct == null)
        {
            return false;
        }

        var loggedUser = await _accountService.GetLoggedUserWithDetailsAsync();
        if (loggedUser.FavoriteProducts.All(p => p.ProductId != favoriteProductId))
        {
            return false;
        }

        loggedUser.FavoriteProducts.Remove(favoriteProduct);
        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<IEnumerable<Product>> GetLoggedUserFavoriteProductsAsync() =>
        (await _accountService.GetLoggedUserWithDetailsAsync()).FavoriteProducts;

    //Start Order Section

    #region Orders

    public async Task<IEnumerable<Order>> GetOrdersAsync() => await _productRepository.GetAllOrdersAsync();

    public async Task<IEnumerable<OrderViewModel>> GetOrdersListViewAsync()
    {
        var orders = await _productRepository.GetAllOrdersAsync();
        var ordersViewModel = orders.Select(p =>
        {
            var orderName = $"#{p.OrderId} {p.OwnerUser?.UserDetails?.FirstName} {p.OwnerUser?.UserDetails?.LastName}";
            var orderDate = p.OrderPaymentTime == default
                ? p.OrderCreateTime.ToSolarWithTime()
                : p.OrderPaymentTime.ToSolarWithTime();
            decimal totalOrderPrice = 0;
            if (p.OrdersDetails != null && p.OrdersDetails.Any())
            {
                totalOrderPrice = p.OrdersDetails.Sum(orderDetails => orderDetails.OrderDetailsTotalPrice) -
                                  (p.Discounts?.Sum(discount => discount.Amount) ?? 0);
            }
            else if (p.InvoicesDetails != null && p.InvoicesDetails.Any())
            {
                totalOrderPrice =
                    p.InvoicesDetails.Sum(orderInvoiceDetails => orderInvoiceDetails.InvoiceDetailsTotalPrice) -
                    (p.Discounts?.Sum(discount => discount.Amount) ?? 0);
            }

            var notEmpty = p.OrdersDetails.NotNullOrEmpty() || p.InvoicesDetails.NotNullOrEmpty();
            var orderViewModel = new OrderViewModel()
            {
                OrderId = p.OrderId,
                OrderName = orderName,
                CreateTime = orderDate,
                OrderStatus = p.OrderStatus,
                IsOrderSeen = p.IsOrderSeen,
                NotEmpty = notEmpty,
                OrderTotalPrice = totalOrderPrice > 0 ? $"{totalOrderPrice:C} " : "0.00"
            };
            return orderViewModel;
        });
        return ordersViewModel;
    }

    public async Task<IEnumerable<OrderViewModel>> GetLoggedUserOrdersAsync()
    {
        var loggedUserId = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var loggedUserOrders = await _productRepository.GetUserOrdersAsync(loggedUserId);
        var ordersViewModel = loggedUserOrders.Select(p =>
        {
            var orderName = $"#{p.OrderId} ";
            if (p.OwnerUser?.UserDetails != null)
            {
                orderName += $"{p.OwnerUser.UserDetails.FirstName} {p.OwnerUser.UserDetails.LastName}";
            }

            var orderDate = p.OrderPaymentTime == default
                ? p.OrderCreateTime.ToSolarWithTime()
                : p.OrderPaymentTime.ToSolarWithTime();
            var notEmpty = p.OrdersDetails.NotNullOrEmpty() || p.InvoicesDetails.NotNullOrEmpty();
            var totalOrderPrice = p.OrdersDetails.NotNullOrEmpty()
                ? p.OrdersDetails.Sum(orderDetails => orderDetails.OrderDetailsTotalPrice) -
                  p.Discounts.Sum(discount => discount.Amount)
                : p.InvoicesDetails.Sum(orderInvoiceDetails => orderInvoiceDetails.InvoiceDetailsTotalPrice) -
                  p.Discounts.Sum(discount => discount.Amount);
            var orderViewModel = new OrderViewModel()
            {
                OrderId = p.OrderId,
                OrderName = orderName,
                CreateTime = orderDate,
                OrderStatus = p.OrderStatus,
                OrderTotalPrice = totalOrderPrice > 0 ? $"{totalOrderPrice:C} " : "0.00",
                NotEmpty = notEmpty
            };
            return orderViewModel;
        });
        return ordersViewModel;
    }

    public async Task<OrderViewModel> GetOrderForEditAsync(int orderId)
    {
        var order = await _productRepository.GetOrderWithDetailsAsync(orderId);
        if (order == null) return null;
        var orderName = order.OwnerUser is { UserDetails: not null }
            ? $"#{order.OrderId} {order.OwnerUser.UserDetails.FirstName} {order.OwnerUser.UserDetails.LastName}"
            : $"#{order.OrderId}";
        var createTime = order.OrderCreateTime.ToSolarWithTime();
        var paymentTime = order.OrderPaymentTime != default ? order.OrderPaymentTime.ToSolarWithTime() : "-";
        var paymentMethod = order.IsOrderCompleted ? "Stripe" : "-";
        var orderViewModel = new OrderViewModel
        {
            OrderName = orderName,
            OrderId = order.OrderId,
            CreateTime = createTime,
            PaymentTime = paymentTime,
            PaymentMethod = paymentMethod,
            OrderStatus = order.OrderStatus,
            OrderNote = order.OrderNote,
            OwnerUser = order.OwnerUser,
            Discounts = order.Discounts
        };
        decimal totalOrderPrice;
        if (order.OrdersDetails != null && order.OrdersDetails.Any())
        {
            totalOrderPrice = order.OrdersDetails.Sum(p => p.OrderDetailsTotalPrice) -
                              (order.Discounts?.Sum(p => p.Amount) ?? 0);
            orderViewModel.OrderTotalPrice = totalOrderPrice > 0 ? $"{totalOrderPrice:C}" : "0.00";
            orderViewModel.OrderInvoicesDetails = order.OrdersDetails.Select(p =>
            {
                var orderInvoiceDetails = new OrderInvoiceDetails
                {
                    InvoiceDetailsProductName = p.Product.ProductName,
                    InvoiceDetailsTotalPrice = p.OrderDetailsTotalPrice,
                    InvoiceDetailsQuantity = p.OrderDetailsQuantity,
                };
                return orderInvoiceDetails;
            }).ToList();
        }
        else
        {
            if (order.InvoicesDetails != null && order.Discounts != null)
            {
                totalOrderPrice = order.InvoicesDetails.Sum(p => p.InvoiceDetailsTotalPrice) -
                                  order.Discounts.Sum(p => p.Amount);
            }
            else if (order.InvoicesDetails != null)
            {
                totalOrderPrice = order.InvoicesDetails.Sum(p => p.InvoiceDetailsTotalPrice);
            }
            else if (order.Discounts != null)
            {
                totalOrderPrice = -order.Discounts.Sum(p => p.Amount);
            }
            else
            {
                totalOrderPrice = 0;
            }

            orderViewModel.OrderTotalPrice = totalOrderPrice > 0 ? $"{totalOrderPrice:C}" : "0.00";
            orderViewModel.OrderInvoicesDetails = order.InvoicesDetails;
        }

        await SetOrderAsSeenAsync(order);
        return orderViewModel;
    }

    public async Task<Order> GetLoggedInUserOpenOrderAsync()
    {
        var loggedInUser = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var loggedInUserOpenOrder = await _productRepository.GetLoggedInUserOpenOrderAsync(loggedInUser);
        return loggedInUserOpenOrder;
    }

    public async Task<OrderViewModel> OrderTrackingByUserEmailAsync(string userEmail, int orderId)
    {
        var userOrder = await _productRepository.GetUserOrderByEmailAsync(userEmail, orderId);
        if (userOrder == null)
        {
            return null;
        }

        var orderName =
            $"#{userOrder.OrderId} {userOrder.OwnerUser.UserDetails.FirstName} {userOrder.OwnerUser.UserDetails.LastName}";
        var createTime = userOrder.OrderCreateTime.ToSolarWithTime();
        var paymentTime = userOrder.OrderPaymentTime != default ? userOrder.OrderPaymentTime.ToSolarWithTime() : "-";
        var paymentMethod = userOrder.IsOrderCompleted ? "Stripe" : "-";
        var orderViewModel = new OrderViewModel()
        {
            OrderName = orderName,
            OrderId = userOrder.OrderId,
            CreateTime = createTime,
            PaymentTime = paymentTime,
            PaymentMethod = paymentMethod,
            OrderStatus = userOrder.OrderStatus,
            OrderNote = userOrder.OrderNote,
            OwnerUser = userOrder.OwnerUser,
            Discounts = userOrder.Discounts
        };
        decimal totalOrderPrice;
        if (userOrder.OrdersDetails != null)
        {
            totalOrderPrice = userOrder.OrdersDetails.Sum(p => p.OrderDetailsTotalPrice) -
                              userOrder.Discounts.Sum(p => p.Amount);
            orderViewModel.OrderTotalPrice = totalOrderPrice > 0 ? $"{totalOrderPrice:C} " : "0.00";
            orderViewModel.OrderInvoicesDetails = userOrder.OrdersDetails.Select(p =>
            {
                var orderInvoiceDetails = new OrderInvoiceDetails()
                {
                    InvoiceDetailsProductName = p.Product.ProductName,
                    InvoiceDetailsTotalPrice = p.OrderDetailsTotalPrice,
                    InvoiceDetailsQuantity = p.OrderDetailsQuantity,
                };
                return orderInvoiceDetails;
            }).ToList();
        }
        else
        {
            totalOrderPrice = userOrder.InvoicesDetails.Sum(p => p.InvoiceDetailsTotalPrice) -
                              userOrder.Discounts.Sum(p => p.Amount);
            orderViewModel.OrderTotalPrice = totalOrderPrice > 0 ? $"{totalOrderPrice:C} " : "0.00";
            orderViewModel.OrderInvoicesDetails = userOrder.InvoicesDetails;
        }

        return orderViewModel;
    }

    public OrderViewModel GetConfirmedOrderInvoiceAsync(Order confirmedOrder)
    {
        var orderName = "";
        if (confirmedOrder?.OwnerUser?.UserDetails != null)
        {
            orderName = $"#{confirmedOrder.OrderId} {confirmedOrder.OwnerUser.UserDetails.FirstName}" +
                        $" {confirmedOrder.OwnerUser.UserDetails.LastName}";
        }

        var createTime = confirmedOrder?.OrderCreateTime.ToSolarWithTime();
        var paymentTime = confirmedOrder != null && confirmedOrder.OrderPaymentTime != default
            ? confirmedOrder.OrderPaymentTime.ToSolarWithTime()
            : "-";
        var paymentMethod = confirmedOrder is { IsOrderCompleted: true } ? "Stripe" : "-";
        var totalOrderPrice = confirmedOrder != null
            ? confirmedOrder.InvoicesDetails.Sum(p => p.InvoiceDetailsTotalPrice) -
              confirmedOrder.Discounts.Sum(p => p.Amount)
            : 0;
        var orderViewModel = new OrderViewModel()
        {
            OrderName = orderName,
            OrderId = confirmedOrder?.OrderId ?? 0,
            CreateTime = createTime,
            PaymentTime = paymentTime,
            PaymentMethod = paymentMethod,
            OrderStatus = confirmedOrder?.OrderStatus,
            OrderNote = confirmedOrder?.OrderNote,
            OrderTotalPrice = totalOrderPrice > 0 ? $"{totalOrderPrice:C} " : "0.00",
            OwnerUser = confirmedOrder?.OwnerUser,
            Discounts = confirmedOrder?.Discounts,
            OrderInvoicesDetails = confirmedOrder?.InvoicesDetails
        };
        return orderViewModel;
    }

    public async Task<OrderViewModel> GetLoggedUserOrderInvoiceAsync(int orderId)
    {
        var loggedUserId = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userOrder = await _productRepository.GetUserOrderAsync(loggedUserId, orderId);
        if (userOrder == null ||
            (!userOrder.OrdersDetails.NotNullOrEmpty() && !userOrder.InvoicesDetails.NotNullOrEmpty()))
        {
            return null;
        }

        var orderName = userOrder.OwnerUser?.UserDetails != null
            ? $"#{userOrder.OrderId} {userOrder.OwnerUser.UserDetails.FirstName} {userOrder.OwnerUser.UserDetails.LastName}"
            : $"#{userOrder.OrderId}";
        var createTime = userOrder.OrderCreateTime.ToSolarWithTime();
        var paymentTime = userOrder.OrderPaymentTime != default ? userOrder.OrderPaymentTime.ToSolarWithTime() : "-";
        var paymentMethod = userOrder.IsOrderCompleted ? "Stripe" : "-";
        var orderViewModel = new OrderViewModel()
        {
            OrderName = orderName,
            OrderId = userOrder.OrderId,
            CreateTime = createTime,
            PaymentTime = paymentTime,
            PaymentMethod = paymentMethod,
            OrderStatus = userOrder.OrderStatus,
            OrderNote = userOrder.OrderNote,
            OwnerUser = userOrder.OwnerUser,
            Discounts = userOrder.Discounts
        };
        decimal totalOrderPrice;
        if (userOrder.OrdersDetails.NotNullOrEmpty())
        {
            totalOrderPrice = userOrder.OrdersDetails.Sum(p => p.OrderDetailsTotalPrice) -
                              userOrder.Discounts.Sum(p => p.Amount);
            orderViewModel.OrderTotalPrice = totalOrderPrice > 0 ? $"{totalOrderPrice:C} " : "0.00";
            orderViewModel.OrderInvoicesDetails = userOrder.OrdersDetails.Select(p =>
            {
                var orderInvoiceDetails = new OrderInvoiceDetails()
                {
                    InvoiceDetailsProductName = p.Product.ProductName,
                    InvoiceDetailsTotalPrice = p.OrderDetailsTotalPrice,
                    InvoiceDetailsQuantity = p.OrderDetailsQuantity,
                };
                return orderInvoiceDetails;
            }).ToList();
        }
        else
        {
            totalOrderPrice = userOrder.InvoicesDetails.Sum(p => p.InvoiceDetailsTotalPrice) -
                              userOrder.Discounts.Sum(p => p.Amount);
            orderViewModel.OrderTotalPrice = totalOrderPrice > 0 ? $"{totalOrderPrice:C}" : "0.00";
            orderViewModel.OrderInvoicesDetails = userOrder.InvoicesDetails;
        }

        return orderViewModel;
    }

    public async Task<int> GetCompletedOrdersCountAsync() => await _productRepository.GetCompletedOrdersCountAsync();

    public async Task<int> GetUnCompletedOrdersCountAsync() =>
        await _productRepository.GetUnCompletedOrdersCountAsync();

    public async Task<int> GetUnSeenOrdersCountAsync() => await _productRepository.GetUnSeenOrdersCountAsync();

    public async Task<bool> UpdateOrderAsync(Order order)
    {
        var currentOrder = await _productRepository.GetOrderByIdAsync(order.OrderId);
        if (currentOrder == null)
        {
            return false;
        }

        currentOrder.OrderStatus = order.OrderStatus;
        _productRepository.UpdateOrder(currentOrder);
        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<bool> SetOrderAsSeenAsync(Order order)
    {
        if (order.IsOrderSeen)
        {
            return true;
        }

        order.IsOrderSeen = true;
        _productRepository.UpdateOrder(order);
        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<bool> DeleteOrderByIdAsync(int orderId)
    {
        var order = await _productRepository.GetOrderByIdAsync(orderId);
        if (order == null)
        {
            return false;
        }

        _productRepository.DeleteOrder(order);
        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<int> GetLoggedUserOpenOrderDetailsCountAsync()
    {
        var loggedUserId = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var loggedUserOpenOrderDetailsCount = await _productRepository.GetUserOpenOrderDetailsCountAsync(loggedUserId);
        return loggedUserOpenOrderDetailsCount;
    }

    public async Task<bool> DeleteOrderDetailsByIdAsync(int orderDetailsId)
    {
        var orderDetails = await _productRepository.GetOrderDetailsByIdAsync(orderDetailsId);
        if (orderDetails is null)
        {
            return false;
        }

        _productRepository.DeleteOrderDetails(orderDetails);
        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<bool> OrderConfirmationAsync(Order loggedUserOpenOrder)
    {
        if (loggedUserOpenOrder == null) return false;
        if (loggedUserOpenOrder.PaymentMethod == "InPersonPayment")
        {
            loggedUserOpenOrder.OrderStatus = OrderStatus.AwaitingPayment.GetDescription();
            loggedUserOpenOrder.OrderPaymentTime = DateTime.Now;
            loggedUserOpenOrder.IsOrderCompleted = false;
            _productRepository.UpdateOrder(loggedUserOpenOrder);
            await _productRepository.SaveAsync();
            return true;
        }

        var totalOrderPrice = loggedUserOpenOrder.OrdersDetails.Sum(p => p.OrderDetailsTotalPrice) -
                              loggedUserOpenOrder.Discounts.Sum(p => p.Amount);
        var stripeUniqueKey = _accessor.HttpContext?.Request.Query["id"].ToString();

        // Check if the query string has the correct parameters
        var query = _accessor.HttpContext?.Request.Query;
        List<OrderInvoiceDetails> orderInvoiceDetailsList;
        if (query != null && query["status"] == "10" && !string.IsNullOrEmpty(query["track_id"]) &&
            query["order_id"] == loggedUserOpenOrder.OrderId.ToString())
        {
            var options = new PaymentIntentCreateOptions
            {
                Amount = (long)totalOrderPrice * 100,
                Currency = "usd",
                PaymentMethodTypes = new List<string> { "card" },
                Metadata = new Dictionary<string, string> { { "order_id", loggedUserOpenOrder.OrderId.ToString() } }
            };
            var service = new PaymentIntentService();
            var paymentIntent = await service.CreateAsync(options);
            if (paymentIntent.Status != "requires_confirmation") return false;
            var confirmedPaymentIntent = await service.ConfirmAsync(paymentIntent.Id,
                new PaymentIntentConfirmOptions { PaymentMethod = stripeUniqueKey });
            if (!confirmedPaymentIntent.Status.Equals("succeeded"))
            {
                return false;
            }

            loggedUserOpenOrder.OrderPaymentTime = DateTime.Now;
            loggedUserOpenOrder.OrderStatus = OrderStatus.AwaitingPayment.GetDescription();
            loggedUserOpenOrder.IsOrderCompleted = true;
            foreach (var orderDetails in loggedUserOpenOrder.OrdersDetails)
            {
                var product = orderDetails.Product;
                var quantity = orderDetails.OrderDetailsQuantity;
                product.ProductQuantityInStock -= quantity;
                product.ProductSalesCount += quantity;
                _productRepository.UpdateProduct(product);
                switch (product.ProductQuantityInStock)
                {
                    case <= 0:
                        _productRepository.DeleteOrderDetails(orderDetails);
                        break;
                    default:
                        orderDetails.OrderDetailsQuantity = product.ProductQuantityInStock;
                        break;
                }
            }

            await _productRepository.SaveAsync();
            orderInvoiceDetailsList = loggedUserOpenOrder.OrdersDetails.Select(od => new OrderInvoiceDetails
            {
                InvoiceDetailsProductName = od.Product.ProductName,
                InvoiceDetailsTotalPrice = od.OrderDetailsTotalPrice,
                InvoiceDetailsQuantity = od.OrderDetailsQuantity,
                Order = loggedUserOpenOrder
            }).ToList();
            await Task.WhenAll(orderInvoiceDetailsList.Select(orderInvoiceDetails =>
                _productRepository.AddInvoiceDetailsAsync(orderInvoiceDetails)));
            loggedUserOpenOrder.OrdersDetails = null;
            _productRepository.UpdateOrder(loggedUserOpenOrder);
            await _productRepository.SaveAsync();
            return true;
        }

        if (totalOrderPrice <= 0)
        {
            return false;
        }

        loggedUserOpenOrder.OrderPaymentTime = DateTime.Now;
        loggedUserOpenOrder.OrderStatus = OrderStatus.AwaitingPayment.GetDescription();
        loggedUserOpenOrder.IsOrderCompleted = true;
        foreach (var orderDetails in loggedUserOpenOrder.OrdersDetails)
        {
            var product = orderDetails.Product;
            var availableQuantity = Math.Max(0, product.ProductQuantityInStock - orderDetails.OrderDetailsQuantity);
            product.ProductQuantityInStock = availableQuantity;
            product.ProductSalesCount += orderDetails.OrderDetailsQuantity;
            var matchingOrderDetails = loggedUserOpenOrder.OrdersDetails.Where(d => d != orderDetails &&
                d.IsOrderDetailsProductSimple == orderDetails.IsOrderDetailsProductSimple &&
                (d.Product?.ProductId == product.ProductId)).ToList();
            foreach (var matchingOrderDetail in matchingOrderDetails)
            {
                var remainingQuantity = Math.Max(0, matchingOrderDetail.OrderDetailsQuantity - availableQuantity);
                if (remainingQuantity > 0)
                {
                    matchingOrderDetail.OrderDetailsQuantity = remainingQuantity;
                    continue;
                }

                loggedUserOpenOrder.OrdersDetails.Remove(matchingOrderDetail);
            }
        }

        orderInvoiceDetailsList = loggedUserOpenOrder.OrdersDetails.Select(ordersDetails => new OrderInvoiceDetails
        {
            InvoiceDetailsProductName = ordersDetails.Product.ProductName,
            InvoiceDetailsTotalPrice = ordersDetails.OrderDetailsTotalPrice,
            InvoiceDetailsQuantity = ordersDetails.OrderDetailsQuantity,
            Order = loggedUserOpenOrder,
        }).ToList();
        await Task.WhenAll(orderInvoiceDetailsList.Select(detail => _productRepository.AddInvoiceDetailsAsync(detail)));
        loggedUserOpenOrder.OrdersDetails = null;
        _productRepository.UpdateOrder(loggedUserOpenOrder);
        await _productRepository.SaveAsync();
        return true;
    }

    #endregion

    public async Task<AddProductToCartResult> AddProductToCartAsync(Product product, int requestedQuantity = 1,
        int productVariationId = -1)
    {
        if (product.ProductQuantityInStock < requestedQuantity || requestedQuantity < 1)
        {
            return AddProductToCartResult.Failed;
        }

        var loggedUserOrder = await GetLoggedInUserOpenOrderAsync();
        var loggedUser = await _accountService.GetLoggedUserAsync();
        if (loggedUserOrder == null)
        {
            loggedUserOrder = new Order
            {
                IsOrderCompleted = false,
                OrderCreateTime = DateTime.Now,
                OrderStatus = OrderStatus.AwaitingPayment.GetDescription(),
                OwnerUser = loggedUser
            };
            await _productRepository.AddOrderAsync(loggedUserOrder);
        }

        var orderDetails = loggedUserOrder.OrdersDetails.SingleOrDefault(p => p.Product.ProductId == product.ProductId);
        if (orderDetails != null)
        {
            var finalOrderDetailsQuantity = orderDetails.OrderDetailsQuantity + requestedQuantity;
            if (finalOrderDetailsQuantity > product.ProductQuantityInStock)
            {
                return AddProductToCartResult.OutOfStock;
            }

            orderDetails.OrderDetailsTotalPrice = product.ProductPrice * finalOrderDetailsQuantity;
            orderDetails.OrderDetailsQuantity = finalOrderDetailsQuantity;
            _productRepository.UpdateOrderDetails(orderDetails);
        }
        else
        {
            var orderDetailsTotalPrice = product.ProductPrice * requestedQuantity;
            orderDetails = new OrderDetails
            {
                OrderDetailsTotalPrice = orderDetailsTotalPrice,
                OrderDetailsQuantity = requestedQuantity,
                Order = loggedUserOrder,
                Product = product
            };
            await _productRepository.AddOrderDetailsAsync(orderDetails);
        }

        await _productRepository.SaveAsync();
        return AddProductToCartResult.Successful;
    }

    public async Task<bool> UpdateCartAsync(Order loggedUserOpenOrder, IEnumerable<int> orderDetailsQuantities)
    {
        if (orderDetailsQuantities.Any(p => p < 1))
        {
            return false;
        }

        for (var i = 0; i < loggedUserOpenOrder.OrdersDetails.Count; i++)
        {
            var currentOrderDetails = loggedUserOpenOrder.OrdersDetails.ElementAtOrDefault(i);
            if (currentOrderDetails != null)
            {
                var orderDetailsQuantity = currentOrderDetails.Product.ProductQuantityInStock;
                if (orderDetailsQuantity < orderDetailsQuantities.ElementAtOrDefault(i))
                {
                    return false;
                }

                currentOrderDetails.OrderDetailsQuantity = orderDetailsQuantities.ElementAtOrDefault(i);
                currentOrderDetails.OrderDetailsTotalPrice = currentOrderDetails.OrderDetailsQuantity *
                                                             currentOrderDetails.Product.ProductPrice;
                _productRepository.UpdateOrderDetails(currentOrderDetails);
            }
        }

        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<CartCheckOutViewModel> GetLoggedUserCartCheckOutAsync()
    {
        var loggedUserOpenOrder = await GetLoggedInUserOpenOrderAsync();
        if (loggedUserOpenOrder == null || !loggedUserOpenOrder.OrdersDetails.NotNullOrEmpty())
        {
            return null;
        }

        var userDetails = loggedUserOpenOrder.OwnerUser.UserDetails;
        var cartCheckOutViewModel = new CartCheckOutViewModel()
        {
            FirstName = userDetails?.FirstName ?? "",
            LastName = userDetails?.LastName ?? "",
            MobileNumber = loggedUserOpenOrder.OwnerUser.MobileNumber,
            UserProvince = userDetails?.UserProvince ?? "",
            UserCity = userDetails?.UserCity ?? "",
            UserAddress = userDetails?.UserAddress ?? "",
            UserZipCode = userDetails?.UserZipCode ?? "",
            Order = loggedUserOpenOrder
        };
        return cartCheckOutViewModel;
    }

    public async Task<string> PaymentProcessAsync(CartCheckOutViewModel cartCheckOutViewModel)
    {
        var loggedUserOpenOrder = await GetLoggedInUserOpenOrderAsync();
        if (loggedUserOpenOrder == null)
        {
            return null;
        }

        switch (cartCheckOutViewModel.PaymentMethod)
        {
            case "InPersonPayment":
                loggedUserOpenOrder.OrderStatus = OrderStatus.AwaitingPayment.GetDescription();
                break;
            case "StripePayment":
                var apiKey = _configuration["Stripe:ApiKey"];
                StripeConfiguration.ApiKey = apiKey;
                var totalOrderPrice = loggedUserOpenOrder.OrdersDetails.Sum(p => p.OrderDetailsTotalPrice) -
                                      loggedUserOpenOrder.Discounts.Sum(p => p.Amount);
                if (_accessor.HttpContext == null)
                {
                    return null;
                }

                var options = new PaymentIntentCreateOptions
                {
                    Amount = (long)(totalOrderPrice * 100),
                    Currency = "usd",
                    Metadata = new Dictionary<string, string>
                    {
                        { "firstName", cartCheckOutViewModel.FirstName },
                        { "lastName", cartCheckOutViewModel.LastName },
                        { "mobileNumber", cartCheckOutViewModel.MobileNumber },
                        { "userProvince", cartCheckOutViewModel.UserProvince },
                        { "userCity", cartCheckOutViewModel.UserCity },
                        { "userAddress", cartCheckOutViewModel.UserAddress },
                        { "userZipCode", cartCheckOutViewModel.UserZipCode }
                    }
                };
                try
                {
                    var service = new PaymentIntentService();
                    var paymentIntent = await service.CreateAsync(options);
                    var clientSecret = paymentIntent.ClientSecret;
                    if (loggedUserOpenOrder.OwnerUser?.UserDetails != null)
                    {
                        loggedUserOpenOrder.OwnerUser.UserDetails.FirstName = cartCheckOutViewModel.FirstName;
                        loggedUserOpenOrder.OwnerUser.UserDetails.LastName = cartCheckOutViewModel.LastName;
                        loggedUserOpenOrder.OwnerUser.MobileNumber = cartCheckOutViewModel.MobileNumber;
                        loggedUserOpenOrder.OwnerUser.UserDetails.UserProvince = cartCheckOutViewModel.UserProvince;
                        loggedUserOpenOrder.OwnerUser.UserDetails.UserCity = cartCheckOutViewModel.UserCity;
                        loggedUserOpenOrder.OwnerUser.UserDetails.UserAddress = cartCheckOutViewModel.UserAddress;
                        loggedUserOpenOrder.OwnerUser.UserDetails.UserZipCode = cartCheckOutViewModel.UserZipCode;
                        await _accountService.UpdateUserAsync(loggedUserOpenOrder.OwnerUser);
                    }

                    loggedUserOpenOrder.OrderNote = cartCheckOutViewModel.OrderNote;
                    _productRepository.UpdateOrder(loggedUserOpenOrder);
                    await _productRepository.SaveAsync();
                    var returnUrl = totalOrderPrice > 0
                        ? _linkGenerator.GetUriByAction(_accessor.HttpContext, "OrderConfirmation", "Product",
                            new { payment_intent = clientSecret }, "https")
                        : _linkGenerator.GetUriByAction(_accessor.HttpContext, "OrderConfirmation", "Product", new { },
                            "https");
                    return returnUrl;
                }
                catch (StripeException)
                {
                    // handle the exception here
                    return null;
                }
            default: return null;
        }

        await _productRepository.SaveAsync();
        return _linkGenerator.GetUriByAction(_accessor.HttpContext!, "OrderConfirmation", "Product", new { }, "https");
    }

    public async Task<AddDiscountToCartResult> AddDiscountToCartAsync(Order loggedUserOpenOrder, string discountCode)
    {
        try
        {
            var discount = await _productRepository.GetDiscountByCodeAsync(discountCode);
            if (discount == null || discount.IsTrash) return AddDiscountToCartResult.Failed;
            var doesLoggedUserOpenOrderHasThisDiscount = loggedUserOpenOrder.Discounts.Contains(discount);
            if (doesLoggedUserOpenOrderHasThisDiscount) return AddDiscountToCartResult.AlreadyApplied;
            loggedUserOpenOrder.Discounts.Add(discount);
            _productRepository.UpdateOrder(loggedUserOpenOrder);
            await _productRepository.SaveAsync();
            return AddDiscountToCartResult.Successful;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return AddDiscountToCartResult.Failed;
        }
    }

    public async Task<IEnumerable<Discount>> GetDiscountsAsync()
    {
        var discounts = await _productRepository.GetDiscountsAsync();
        return discounts;
    }

    public async Task<Discount> GetDiscountAsync(int discountId) =>
        await _productRepository.GetDiscountAsync(discountId);

    public async Task<bool> MoveDiscountToTrashAsync(int discountId)
    {
        var discount = await GetDiscountAsync(discountId);
        if (discount == null)
        {
            return false;
        }

        var doesDiscountHasAnyFinishedOrder = discount.Orders.Any(p => p.IsOrderCompleted);
        if (!doesDiscountHasAnyFinishedOrder)
        {
            _productRepository.DeleteDiscount(discount);
        }
        else
        {
            var unfinishedDiscountOrders = discount.Orders.Where(p => !p.IsOrderCompleted);
            for (var i = unfinishedDiscountOrders.Count() - 1; i >= 0; i--)
            {
                discount.Orders.Remove(unfinishedDiscountOrders.ElementAtOrDefault(i));
            }

            discount.IsTrash = true;
            _productRepository.UpdateDiscount(discount);
        }

        await _productRepository.SaveAsync();
        return true;
    }

    public async Task<AddUpdateDiscountResult> AddDiscountAsync(Discount discount)
    {
        try
        {
            var discounts = await GetDiscountsAsync();
            var isDiscountCodeExistForAdd = discounts.Any(p => p.DiscountCode == discount.DiscountCode);
            if (isDiscountCodeExistForAdd)
            {
                return AddUpdateDiscountResult.DiscountCodeExist;
            }

            await _productRepository.AddDiscountAsync(discount);
            await _productRepository.SaveAsync();
            return AddUpdateDiscountResult.Successful;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return AddUpdateDiscountResult.Failed;
        }
    }

    public async Task<AddUpdateDiscountResult> UpdateDiscountAsync(Discount discount)
    {
        var currentDiscount = await GetDiscountAsync(discount.DiscountId);
        if (currentDiscount.DiscountCode != discount.DiscountCode)
        {
            var isDiscountCodeExistForEdit = await _productRepository.DoesDiscountCodeExistAsync(discount.DiscountCode);
            if (isDiscountCodeExistForEdit)
            {
                return AddUpdateDiscountResult.DiscountCodeExist;
            }

            currentDiscount.DiscountCode = discount.DiscountCode;
        }

        currentDiscount.Amount = discount.Amount;
        _productRepository.UpdateDiscount(currentDiscount);
        return AddUpdateDiscountResult.Successful;
    }

    //End Order Section

    //Utilities Methods

    private async Task<bool> DeleteCascadeCategoryByIdAsync(int categoryId)
    {
        var category = await GetCategoryAsync(categoryId);
        if (category == null)
        {
            return false;
        }

        var allCategories = await _productRepository.GetCategoriesAsync();
        await DeleteChildrenCategories(category, allCategories);
        _productRepository.DeleteCategory(category);
        return true;
    }

    private async Task DeleteChildrenCategories(Category parentCategory, IEnumerable<Category> allCategories)
    {
        var childrenCategories = allCategories.Where(p => p.ParentCategory?.CategoryId == parentCategory.CategoryId)
            .ToList();
        foreach (var childCategory in childrenCategories)
        {
            await DeleteChildrenCategories(childCategory, allCategories);
            _productRepository.DeleteCategory(childCategory);
        }
    }

    private async Task AddProductCategoriesAsync(Product product, IEnumerable<int> productCategoriesId)
    {
        var categoryTasks = productCategoriesId.Select(GetCategoryAsync);
        var categories = await Task.WhenAll(categoryTasks);
        product.Categories.AddRange(categories);
    }

    private async Task UpdateProductCategoriesAsync(Product product, IEnumerable<int> productCategoriesId)
    {
        product.Categories.Clear();
        await AddProductCategoriesAsync(product, productCategoriesId);
    }

    private async Task<bool> AddProductsInformationAsync(Product product, IEnumerable<string> informationNames,
        IEnumerable<string> informationValues)
    {
        var names = informationNames.ToList();
        var values = informationValues.ToList();
        for (var i = 0; i < names.Count; i++)
        {
            if (string.IsNullOrEmpty(names[i]) && string.IsNullOrEmpty(values[i]))
            {
                continue;
            }

            var productInformation = new ProductInformation() { Name = names[i], Value = values[i], Product = product };
            await _productRepository.AddProductInformationAsync(productInformation);
        }

        return true;
    }

    private bool DeleteProductInformation(Product product)
    {
        foreach (var productInformation in product.Informations)
            _productRepository.DeleteProductInformation(productInformation);
        return true;
    }

    private async Task<bool> UpdateProductsInformationAsync(Product product, IEnumerable<string> informationNames,
        IEnumerable<string> informationValues)
    {
        if (product.Informations.Any())
        {
            DeleteProductInformation(product);
        }

        await AddProductsInformationAsync(product, informationNames, informationValues);
        return true;
    }
}