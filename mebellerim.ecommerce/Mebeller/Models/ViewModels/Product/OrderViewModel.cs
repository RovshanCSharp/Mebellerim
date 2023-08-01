using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mebeller.Data.Context;
using Mebeller.Models.Product;

namespace Mebeller.Models.ViewModels.Product;

public class OrderViewModel
{
    [Key] public int Id { get; set; }

    public int OrderId { get; set; }
    public string OrderName { get; set; }
    public string CreateTime { get; set; }
    public string PaymentTime { get; set; }
    public string PaymentMethod { get; set; }
    public string OrderStatus { get; set; }
    public string OrderNote { get; set; }
    public string OrderTotalPrice { get; set; }
    public ApplicationUser OwnerUser { get; set; }
    public bool IsOrderSeen { get; init; }
    public bool NotEmpty { get; init; }
    public ICollection<Discount> Discounts { get; set; }
    public ICollection<OrderInvoiceDetails> OrderInvoicesDetails { get; set; }
}