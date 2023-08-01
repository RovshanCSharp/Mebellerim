using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mebeller.Models.Blog;

namespace Mebeller.Models.Product
{
    public class Category
    {
        [Key]
        public int CategoryId { get; init; }

        [Required(ErrorMessage = "Please enter a unique category name that is between 1 and 250 characters long.")]
        [MaxLength(250, ErrorMessage = "Category name cannot exceed 250 characters.")]
        public string CategoryName { get; set; }

        [MaxLength(500, ErrorMessage = "Category description cannot exceed 500 characters.")]
        public string CategoryDescription { get; set; }

        // Navigation Properties
        public ICollection<Product> Products { get; set; }
        public Category ParentCategory { get; set; }
        public ICollection<BlogPost> BlogPosts { get; set; }
        public IEnumerable<Category> Categories { get; set; }

        public Category() => Products = new List<Product>();
    }
}