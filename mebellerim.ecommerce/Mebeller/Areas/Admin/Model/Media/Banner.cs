using System.ComponentModel.DataAnnotations;
using Mebeller.Models.Media;

namespace Mebeller.Areas.Admin.Model.Media
{
    public class Banner
    {
        [Key]
        public int Id { get; set; }

        [MaxLength(200, ErrorMessage = "Banner title cannot exceed 200 characters.")]
        public string Title { get; set; }

        [MaxLength(500, ErrorMessage = "Banner link cannot exceed 500 characters.")]
        public string Link { get; set; }

        [MaxLength(500, ErrorMessage = "Banner description cannot exceed 500 characters.")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Please specify whether this is the primary banner.")]
        public bool IsPrimary { get; set; }

        // Navigation Properties
        [Required(ErrorMessage = "Please select an image for the banner.")]
        public Image Image { get; set; }
    }
}