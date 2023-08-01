using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mebeller.Models.Media;

namespace Mebeller.Models.Blog
{
    public class BlogPost
    {
        [Key]
        public Guid Id { get; init; }
        public string Heading { get; set; }
        public string PageTitle { get; set; }
        public string Content { get; set; }
        public string ShortDescription { get; set; }
        public string FeaturedImageUrl { get; set; }
        public string UrlHandle { get; set; }
        public DateTimeOffset PublishedDate { get; set; }
        public string Author { get; set; }
        public bool Visible { get; set; }

        // Navigation Property
        
        public IEnumerable<BlogCategory> Categories { get; set; }
        public int BlogHits { get; set; }
        public ICollection<Tag> Tags { get; init; }
     }
}