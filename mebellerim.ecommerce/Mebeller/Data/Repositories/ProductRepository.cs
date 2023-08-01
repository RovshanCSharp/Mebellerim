using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mebeller.Data.Context;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Models.Product;
using Microsoft.EntityFrameworkCore;

namespace Mebeller.Data.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context) => _context = context;

    public async Task SaveAsync() => await _context.SaveChangesAsync();

    //Products start
    public async Task<IEnumerable<Product>> GetAllProductsAsync() =>
        await _context.Products.Include(p => p.Categories)
            .Include(p => p.Images)
             .AsSplitQuery()
            .ToListAsync();

    public async Task<IEnumerable<Product>> GetAvailableProductsAsync() =>
        await _context.Products
            .Where(p => p.ProductQuantityInStock > 0).Include(p => p.Categories)
            .Include(p => p.Images)
             .AsSplitQuery()
            .ToListAsync();

    public async Task<Product> GetProductByIdAsync(int productId) => await _context.Products.FindAsync(productId);

    public async Task<Product> GetProductWithDetailsAsync(int productId)
    {
        var product = await _context.Products.Include(p => p.Comments)
            .ThenInclude(p => p.User)
            .SingleOrDefaultAsync(p => p.ProductId == productId);

        if (product != null)
        {
            await _context.Entry(product).Collection(p => p.Categories).LoadAsync();

            await _context.Entry(product).Collection(p => p.Informations).LoadAsync();

            await _context.Entry(product).Collection(p => p.Images).LoadAsync();

        }

        return product;
    }

    public async Task<int> GetProductsCountAsync() => await _context.Products.CountAsync();
        
    public async Task AddProductAsync(Product product) => await _context.AddAsync(product);
        
    public void UpdateProduct(Product product) => _context.Update(product);

    public void DeleteProduct(Product product) => _context.Remove(product);
        
    public async Task<bool> DoesProductExistAsync(int productId) => await _context.Products.AnyAsync(p => p.ProductId == productId);

    //Product section end
        
    public async Task AddProductInformationAsync(ProductInformation productInformation) => await _context.AddAsync(productInformation);
    public void DeleteProductInformation(ProductInformation productInformation) => _context.Remove(productInformation);
 
        
    //end variation section
        
    //Categories section start
        
    public async Task<IEnumerable<Category>> GetCategoriesAsync() =>
        await _context.Categories.Include(p => p.ParentCategory)
            .Include(p => p.Products)
            .AsSplitQuery()
            .ToListAsync();

    public async Task<Category> GetCategoryByIdAsync(int categoryId) =>
        await _context.Categories.Include(p => p.ParentCategory)
            .SingleOrDefaultAsync(p => p.CategoryId == categoryId);

    public async Task AddCategoryAsync(Category category) => await _context.AddAsync(category);

    public void UpdateCategory(Category category) => _context.Update(category);

    public void DeleteCategory(Category category) => _context.Remove(category);

    public async void DeleteCategoryById(int categoryId) => _context.Remove( await GetCategoryByIdAsync(categoryId));

    //End category section
        
    //Start Order Section 

    public async Task<IEnumerable<Order>> GetAllOrdersAsync() =>
        await _context.Orders.Include(p => p.OrdersDetails)
            .Include(p => p.InvoicesDetails)
            .Include(p => p.Discounts)
            .Include(p => p.OwnerUser)
            .ThenInclude(p => p.UserDetails)
            .ToListAsync();

    public async Task<IEnumerable<Order>> GetUserOrdersAsync(string userId) =>
        await _context.Orders.Include(p => p.OwnerUser)
            .ThenInclude(p => p.UserDetails)
            .Where(p => p.OwnerUser.Id == userId)
            .Include(p => p.OrdersDetails)
            .Include(p => p.Discounts)
            .Include(p => p.InvoicesDetails)
            .ToListAsync();

    public async Task<Order> GetOrderByIdAsync(int orderId)
    {
        var order =
            await _context
                .Orders
                .FindAsync(orderId);

        return order;
    }

    public async Task<Order> GetOrderWithDetailsAsync(int orderId)
    {
        var order = await _context.Orders.Include(p => p.OrdersDetails)
            .ThenInclude(p => p.Product)
            .Include(p => p.OrdersDetails)
            .SingleOrDefaultAsync(p => p.OrderId == orderId);

        if (order != null)
        {
            if (order.InvoicesDetails != null)
            {
                await _context.Entry(order).Collection(p => p.InvoicesDetails).LoadAsync();
            }

            if (order.Discounts != null)
            {
                await _context.Entry(order).Collection(p => p.Discounts).LoadAsync();
            }

            if (order.OwnerUser != null)
            {
                await _context.Entry(order).Reference(p => p.OwnerUser).LoadAsync();

                if (order.OwnerUser.UserDetails != null)
                    await _context.Entry(order.OwnerUser).Reference(p => p.UserDetails).LoadAsync();
            }
        }

        return order;
    }

    public async Task<Order> GetLoggedInUserOpenOrderAsync(string userId)
    {
        var order = await _context.Orders.Include(p => p.OwnerUser)
            .ThenInclude(p => p.UserDetails)
            .Include(p => p.OrdersDetails)
            .ThenInclude(p => p.Product)
            .ThenInclude(p => p.Images)
            .Include(p => p.OrdersDetails)
             .SingleOrDefaultAsync(p => p.OwnerUser.Id == userId && !p.IsOrderCompleted);

        if (order != null)
        {
            await _context.Entry(order).Collection(p => p.Discounts).LoadAsync();
        }

        return order;
    }

    public async Task<Order> GetUserOrderAsync(string userId, int orderId)
    {
        var order = await _context.Orders.Include(p => p.OwnerUser)
            .ThenInclude(p => p.UserDetails)
            .Include(p => p.OrdersDetails)
            .ThenInclude(p => p.Product)
            .Include(p => p.OrdersDetails)
             .SingleOrDefaultAsync(p => p.OrderId == orderId && p.OwnerUser.Id == userId);

        if (order != null)
        {
            await _context.Entry(order).Collection(p => p.InvoicesDetails).LoadAsync();

            await _context.Entry(order).Collection(p => p.Discounts).LoadAsync();
        }

        return order;
    }

    public async Task<Order> GetUserOrderByEmailAsync(string userEmail, int orderId)
    {
        var userOrder = await _context.Orders.Include(p => p.OwnerUser)
            .ThenInclude(p => p.UserDetails)
            .Include(p => p.OrdersDetails)
            .ThenInclude(p => p.Product)
            .Include(p => p.OrdersDetails)
             .SingleOrDefaultAsync(p => p.OrderId == orderId);

        if (userOrder != null)
        {
            await _context.Entry(userOrder).Collection(p => p.InvoicesDetails).LoadAsync();

            await _context.Entry(userOrder).Collection(p => p.Discounts).LoadAsync();
        }

        return userOrder;
    }

    public async Task<int> GetCompletedOrdersCountAsync() => await _context.Orders.CountAsync(p => p.IsOrderCompleted);

    public async Task<int> GetUnCompletedOrdersCountAsync() => await _context.Orders.CountAsync(p => !p.IsOrderCompleted);

    public async Task<int> GetUnSeenOrdersCountAsync() => await _context.Orders.CountAsync(p => !p.IsOrderSeen);

    public async Task AddOrderAsync(Order order) => await _context.AddAsync(order);

    public void UpdateOrder(Order order) => _context.Update(order);

    public void DeleteOrder(Order order) => _context.Remove(order);

    public async Task<IEnumerable<OrderDetails>> GetUnFinishedOrdersDetailsAsync() =>
        await _context.OrdersDetails.Where(p => !p.Order.IsOrderCompleted)
            .Include(p => p.Product)
             .ToListAsync();

    public async Task<IEnumerable<OrderDetails>> GetOrdersDetailsByProductIdAsync(int productId) =>
        await _context.OrdersDetails.Where(p => p.Product.ProductId == productId)
            .Include(p => p.Order)
             .ToListAsync();

    public async Task<OrderDetails> GetOrderDetailsByIdAsync(int orderDetailsId) => await _context.OrdersDetails.FindAsync(orderDetailsId);

    public async Task<int> GetUserOpenOrderDetailsCountAsync(string userId) =>
        await _context.OrdersDetails.CountAsync(
            p => p.Order.OwnerUser.Id == userId && !p.Order.IsOrderCompleted);

    public async Task AddOrderDetailsAsync(OrderDetails orderDetails) => await _context.AddAsync(orderDetails);

    public void UpdateOrderDetails(OrderDetails orderDetails) => _context.Update(orderDetails);

    public void DeleteOrderDetails(OrderDetails orderDetails) => _context.Remove(orderDetails);

    public async Task AddInvoiceDetailsAsync(OrderInvoiceDetails invoiceDetails) => await _context.AddAsync(invoiceDetails);

    //End Orders section
        
    //Discounts start
    public async Task<IEnumerable<Discount>> GetDiscountsAsync() => await _context.Discounts.ToListAsync();
    public async Task<Discount> GetDiscountAsync(int discountId) => await _context.Discounts.Include(p => p.Orders).SingleOrDefaultAsync(p => p.DiscountId == discountId);
    public async Task<Discount> GetDiscountByCodeAsync(string discountCode) => await _context.Discounts.SingleOrDefaultAsync(p => p.DiscountCode == discountCode);
    public async Task<bool> DoesDiscountCodeExistAsync(string discountCode) => await _context.Discounts.AnyAsync(p => p.DiscountCode == discountCode);

    public async Task AddDiscountAsync(Discount discount) => await _context.AddAsync(discount);
    public void UpdateDiscount(Discount discount) => _context.Update(discount);

    public void DeleteDiscount(Discount discount) => _context.Remove(discount);

    //End Discounts Section
}