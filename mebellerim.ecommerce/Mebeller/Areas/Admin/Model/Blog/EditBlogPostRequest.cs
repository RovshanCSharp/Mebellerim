using System;
using System.ComponentModel.DataAnnotations;
using Mebeller.Models.Blog;
using Mebeller.Models.Media;

namespace Mebeller.Areas.Admin.Model.Blog
{
    public class EditBlogPostRequest
    {
        
        public Guid Id { get; set; }

        [Required]
        public string Heading { get; set; }

        [Required]
        public string PageTitle { get; set; }

        [Required]
        public string Content { get; set; }

        [Required]
        public string ShortDescription { get; set; }

        public string FeaturedImageUrl { get; set; }

        [Required]
        public string UrlHandle { get; set; }

        [Required]
        public DateTimeOffset PublishedDate { get; set; }
        
        [Required]
        public string Author { get; set; }

        public bool Visible { get; set; }
        public Tag Tags { get; set; }
         public BlogPost BlogPost { get; set; }
    }
}