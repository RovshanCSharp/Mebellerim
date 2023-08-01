using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Mebeller.Models.Media;

namespace Mebeller.Models.Product
{
    public class Product
    {
        [Key]
        public int ProductId { get; init; }

        [Required(ErrorMessage = "Please enter the product name.")]
        [StringLength(200, ErrorMessage = "The product name cannot exceed {1} characters.")]
        public string ProductName { get; set; }

        public string ProductDescription { get; set; }

        [Required(ErrorMessage = "Please enter the price of the product.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid price for the product, which must be a positive integer.")]
        public int ProductPrice { get; set; }

        [Required(ErrorMessage = "Please enter the number of products.")]
        [Range(1, int.MaxValue, ErrorMessage = "Please enter a valid number of products in stock, which must be a positive integer.")]
        public int ProductQuantityInStock { get; set; }

        public int ProductHits { get; set; }
        public int ProductSalesCount { get; set; }
        public DateTime RegistrationTime { get; init; } = DateTime.UtcNow;

        // Navigation Properties
        public ICollection<Category> Categories { get; init; } = new List<Category>();
        public ICollection<ProductInformation> Informations { get; set; } = new List<ProductInformation>();
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        public ICollection<Image> Images { get; set; } = new List<Image>();

        public IEnumerable<Discount> ProductDiscount { get; set; } = new List<Discount>();

        public string UserName { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        public string UserEmail { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        public string UserPhoneNumber { get; set; }

        [NotMapped]
        public int CommentCount => Comments?.Count ?? 0;
    }
}
