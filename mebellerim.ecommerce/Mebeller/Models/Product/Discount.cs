using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Mebeller.Models.Product
{
    public class Discount
    {
        [Key]
        public int DiscountId { get; set; }

        [Required(ErrorMessage = "Please enter the discount code.")]
        [StringLength(15, MinimumLength = 5, ErrorMessage = "Discount code must be between 5 and 15 characters.")]
        public string DiscountCode { get; set; }

        [Required(ErrorMessage = "Please enter the discount percentage.")]
        [Range(0.0, 100.0, ErrorMessage = "Discount percentage must be between 0 and 100.")]
        public decimal Amount { get; set; }

        // Navigation Properties
        public ICollection<Order> Orders { get; set; }

        public bool IsTrash { get; set; }
    }
}