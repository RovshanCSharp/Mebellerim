using System.ComponentModel.DataAnnotations;
using Order = Mebeller.Models.Product.Order;

namespace Mebeller.Models.ViewModels.Product
{
    public class CartCheckOutViewModel
    {
        [Key] public int Id { get; set; }

        [MaxLength(250, ErrorMessage = "Max length can't exceed 250")]
        public string FirstName { get; set; }

        [MaxLength(250, ErrorMessage = "Max length can't exceed 250")]
        public string LastName { get; set; }

        [MaxLength(250, ErrorMessage = "Max length can't exceed 250")]
        public string UserProvince { get; set; }

        [MaxLength(250, ErrorMessage = "Max length can't exceed 250")]
        public string UserCity { get; set; }

        public string UserAddress { get; set; }
        public string UserZipCode { get; set; }
        public string OrderNote { get; set; }

        [Required(ErrorMessage = "Please choose the payment method")]
        public string PaymentMethod { get; set; }

        [Compare(nameof(HiddenAcceptTheRules), ErrorMessage = "You must accept the rules")]
        public bool AcceptTheRules { get; set; }

        public bool HiddenAcceptTheRules { get; set; } = true;
        public Order Order { get; set; }
        [DataType(DataType.PhoneNumber)] public string MobileNumber { get; set; }
        public int OrderId { get; set; }
        public string Email { get; set; }
    }
}