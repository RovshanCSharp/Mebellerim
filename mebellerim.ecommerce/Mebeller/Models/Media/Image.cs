using System.ComponentModel.DataAnnotations;

namespace Mebeller.Models.Media
{
    public class Image
    {
        public static string DefaultImagePath => "/img/default-image.png";

        [Key]
        public int ImageId { get; set; }

        [Required]
        [MaxLength(300)]
        [Display(Name = "Upload Image")]
        public string ImagePath { get; set; }

        // Navigation Properties
        public Product.Product Product { get; set; }
    }
}