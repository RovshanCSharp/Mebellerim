using System.ComponentModel.DataAnnotations;

namespace Mebeller.Areas.Admin.Model.Media
{
    public class Newsletter
    {
        [Key]
        public int NewsletterId { get; set; }
        
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [MaxLength(256, ErrorMessage = "Email address cannot exceed 256 characters.")]
        [Required(ErrorMessage = "Email address is required.")]
        public string CustomerEmail { get; init; }
    }
}