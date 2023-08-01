using System;
using System.Collections;
using System.Collections.Generic;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Areas.Admin.ViewModels.User;
using Mebeller.Models.Blog;
using Mebeller.Models.Media;
using Mebeller.Models.Product;
using Mebeller.Models.ViewModels.Account;
using Mebeller.Models.ViewModels.Product;

namespace Mebeller.Models.ViewModels.Home
{
    public class IndexViewModel : EditAccountViewModel
    {
        // Banners
        public IEnumerable<Banner> Banners { get; set; }
        
        // Products
        public IEnumerable<Models.Product.Product> Products { get; set; }
        public IEnumerable<Category> Categories { get; set; }
        public string SortBy { get; set; }
        
        public IEnumerable<OrderViewModel> OrderList { get; set; }

        // Blog Posts
        public IEnumerable<BlogPost> BlogPosts { get; set; }
        public BlogPost BlogPost { get; set; }
        public Guid BlogPostId { get; set; }
        public List<BlogComment> Comments { get; set; } 
        public string CommentDescription { get; set; }
        public List<Tag> Tags { get; set; }
        
        // Categories
        public IEnumerable<CategoryTreeView> CategoriesTreeViews { get; set; }
        public IEnumerable<int> SelectedCategories { get; set; }
        
        // Account
        public Order Orders { get; set; }
         public new string FirstName { get; set; }
        public UserDetailsViewModel UserDetailsViewModel { get; set; }
    }
}
