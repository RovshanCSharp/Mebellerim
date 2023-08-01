using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Models.Blog;
using Mebeller.Models.Product;

namespace Mebeller.Models.ViewModels.Home
{
    public class FullScreenViewModel
    {
        public IEnumerable<Banner> Banners { get; set; }
        public IEnumerable<Models.Product.Product> Products { get; set; }
        public IEnumerable<BlogPost> BlogPosts { get; set; }
        public Order Orders { get; set; }

        [Required(ErrorMessage = "Please enter your name.")]
        [MaxLength(250, ErrorMessage = "Your name cannot exceed 250 characters.")]
        public string MessageSenderName { get; set; }

        [Required(ErrorMessage = "Please enter your email address.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [MaxLength(250, ErrorMessage = "Your email address cannot exceed 250 characters.")]
        public string MessageSenderEmail { get; set; }

        [Required(ErrorMessage = "Please enter the subject of your message.")]
        [MaxLength(250, ErrorMessage = "The subject cannot exceed 250 characters.")]
        public string MessageSubject { get; set; }

        [Required(ErrorMessage = "Please describe your message.")]
        [MaxLength(2000, ErrorMessage = "Your message cannot exceed 2000 characters.")]
        public string MessageDescription { get; set; }

        public IEnumerable<CategoryTreeView> CategoriesTreeViews { get; set; }
    }
}