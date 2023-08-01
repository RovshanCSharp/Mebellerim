using System.ComponentModel.DataAnnotations;

namespace Mebeller.Models.Product
{
    public class OrderDetails
    {
     
        [Key]
        public int OrderDetailsId { get; set; }
        
        // Navigation Properties
        public Order Order { get; init; }
        public Product Product { get; init; }
        
        [Required(ErrorMessage = "Please enter the order details total price.")]
        [Range(0, int.MaxValue, ErrorMessage = "Order details total price must be a non-negative integer.")]
        public decimal OrderDetailsTotalPrice { get; set; }

        [Required(ErrorMessage = "Please enter the order details quantity.")]
        [Range(1, int.MaxValue, ErrorMessage = "Order details quantity must be a positive integer.")]
        public int OrderDetailsQuantity { get; set; }

        public bool IsOrderDetailsProductSimple { get; set; }
    }
}