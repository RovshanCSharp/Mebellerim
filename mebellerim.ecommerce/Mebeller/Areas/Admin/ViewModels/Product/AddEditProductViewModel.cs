using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mebeller.Data.Utilities.CustomValidationAttribute;
using Mebeller.Models.Media;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mebeller.Areas.Admin.ViewModels.Product
{
    public class AddEditProductViewModel
    {
        public AddEditProductViewModel()
        {
            ProductCategoriesId = new List<int>();
            ProductImagesFiles = new List<IFormFile>();
            InformationNames = new List<string>();
            InformationValues = new List<string>();
        }

        [Required(ErrorMessage = "Please enter a product name.")]
        [MaxLength(200)]
        public string ProductName { get; set; }

        [Required(ErrorMessage = "Please enter the price of the product.")]
        [Range(0, 999999999, ErrorMessage = "Price cannot be negative.")]
        public int ProductPrice { get; set; }

        [Required(ErrorMessage = "Please enter the number of products.")]
        [Range(0, 999999999, ErrorMessage = "Quantity cannot be negative.")]
        public int ProductQuantityInStock { get; set; }
        public string ProductDescription { get; set; }

        [MaxFilesCount(11, ErrorMessage = "You can upload a maximum of 11 files.")]
        [MaxFilesSize(4194304, ErrorMessage = "File size must be less than 4MB.")]
        [AllowedFilesExtensions(new[] { ".png", ".jpg", ".jpeg" }, ErrorMessage = "Only PNG, JPG, and JPEG files are allowed.")]
        public IEnumerable<IFormFile> ProductImagesFiles { get; set; }

        public IEnumerable<int> ProductCategoriesId { get; set; }
        public IEnumerable<SelectListItem> ProductCategories { get; set; }
        public IEnumerable<string> InformationNames { get; set; }
        public IEnumerable<string> InformationValues { get; set; }

        public IEnumerable<Image> CurrentProductImages { get; set; }
        public IEnumerable<int> DeletedProductImagesIds { get; set; }

        public int ProductId { get; set; }
         public string UserName { get; set; }
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Phone number is required.")]
        [Phone(ErrorMessage = "Invalid phone number.")]
        [Display(Name = "Phone Number")]
        public string UserPhoneNumber { get; set; }

     }
}
