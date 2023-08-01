using System.ComponentModel.DataAnnotations;

namespace Mebeller.Models.Product
{
    public class ProductInformation
    {
        [Key]
        public int ProductInformationId { get; set; }

        [Required(ErrorMessage = "Please enter the attribute name.")]
        [StringLength(250, ErrorMessage = "The attribute name cannot exceed {1} characters.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please enter the attribute value.")]
        [StringLength(300, ErrorMessage = "The attribute value cannot exceed {1} characters.")]
        public string Value { get; set; }

        // Navigation Properties
        public Product Product { get; set; }
    }
}