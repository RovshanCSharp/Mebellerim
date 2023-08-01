using System.ComponentModel.DataAnnotations;

namespace Mebeller.Models.Media
{
    public class Page
    {
        [Key]
        public int PageId { get; set; }
        
        [MaxLength(300, ErrorMessage = "The page title cannot exceed 300 characters.")]
        [Required(ErrorMessage = "Please enter the title of the page.")]
        public string PageTitle { get; set; }
        
        [MaxLength(500, ErrorMessage = "The page path address cannot exceed 500 characters.")]
        [Required(ErrorMessage = "Please enter the address of the page.")]
        public string PagePathAddress { get; set; }
        
        [MaxLength(1000, ErrorMessage = "The page description cannot exceed 1000 characters.")]
        public string PageDescription { get; set; }
    }
}