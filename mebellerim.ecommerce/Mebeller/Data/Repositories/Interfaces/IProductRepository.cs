using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Models.Product;

namespace Mebeller.Data.Repositories.Interfaces
{
    public interface IProductRepository : IGeneralRepository
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<IEnumerable<Product>> GetAvailableProductsAsync();
        Task<Product> GetProductByIdAsync(int productId);
        Task<Product> GetProductWithDetailsAsync(int productId);
        Task<int> GetProductsCountAsync();
        Task<bool> DoesProductExistAsync(int productId);
        Task AddProductAsync(Product product);
        Task AddProductInformationAsync(ProductInformation productInformation);
        void DeleteProductInformation(ProductInformation productInformation);
        void UpdateProduct(Product product);
        void DeleteProduct(Product product);
      
        Task<IEnumerable<Category>> GetCategoriesAsync();
        Task<Category> GetCategoryByIdAsync(int categoryId);
        Task AddCategoryAsync(Category category);
        void DeleteCategory(Category category);
        void DeleteCategoryById(int categoryId);
        void UpdateCategory(Category category);

        //Start Order Section

        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<IEnumerable<Order>> GetUserOrdersAsync(string userId);
        Task<Order> GetOrderByIdAsync(int orderId);
        Task<Order> GetOrderWithDetailsAsync(int orderId);
        Task<Order> GetLoggedInUserOpenOrderAsync(string userId);
        Task<Order> GetUserOrderAsync(string userId, int orderId);
        Task<Order> GetUserOrderByEmailAsync(string userEmail, int orderId);
        Task<int> GetCompletedOrdersCountAsync();
        Task<int> GetUnCompletedOrdersCountAsync();
        Task<int> GetUnSeenOrdersCountAsync();
        Task AddOrderAsync(Order order);
        void UpdateOrder(Order order);
        void DeleteOrder(Order order);
        Task<IEnumerable<OrderDetails>> GetUnFinishedOrdersDetailsAsync();
        Task<IEnumerable<OrderDetails>> GetOrdersDetailsByProductIdAsync(int productId);
        Task<OrderDetails> GetOrderDetailsByIdAsync(int orderDetailsId);
        Task<int> GetUserOpenOrderDetailsCountAsync(string userId);
        Task AddOrderDetailsAsync(OrderDetails orderDetails);
        void UpdateOrderDetails(OrderDetails orderDetails);
        void DeleteOrderDetails(OrderDetails orderDetails);
        Task AddInvoiceDetailsAsync(OrderInvoiceDetails invoiceDetails);              
        Task<IEnumerable<Discount>> GetDiscountsAsync();
        Task<Discount> GetDiscountAsync(int discountId);
        Task<Discount> GetDiscountByCodeAsync(string discountCode);
        Task<bool> DoesDiscountCodeExistAsync(string discountCode);
        Task AddDiscountAsync(Discount discount);
        void UpdateDiscount(Discount discount);
        void DeleteDiscount(Discount discount);

        //End Order Section
    }
}
