using System.ComponentModel.DataAnnotations;

namespace Mebeller.Models.ViewModels.Product
{
    public class OrderTrackingViewModel
    {
        [Required(ErrorMessage = "Please enter your order ID.")]
        public int OrderId { get; set; }

        [Required(ErrorMessage = "Please enter your email.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }

        public OrderViewModel Order { get; set; }
    }
}