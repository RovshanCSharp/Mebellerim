using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mebeller.Data.Context;

namespace Mebeller.Models.Product
{
    public class Order
    {
        public Order() => OrdersDetails = new List<OrderDetails>();

        [Key]
        public int OrderId { get; set; }
        [Required]
        public bool IsOrderCompleted { get; set; }  
        [MaxLength(150)]
        [Required]
        public string OrderStatus { get; set; }
        public bool IsOrderSeen { get; set; }
        public string OrderNote { get; set; }
        [Required]
        public DateTime OrderCreateTime { get; set; }
        public DateTime OrderPaymentTime { get; set; }

        //Navigations Properties

        public ICollection<OrderDetails> OrdersDetails { get; set; }
        public ICollection<OrderInvoiceDetails> InvoicesDetails { get; set; }    
        [Required]
        public ApplicationUser OwnerUser { get; set; }
        public ICollection<Discount> Discounts { get; set; }
        public string PaymentMethod { get; set; }
    }
}