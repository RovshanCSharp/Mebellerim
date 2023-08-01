using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mebeller.Models.Product;
using Mebeller.Models.ViewModels.Account;
 using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mebeller.Models.ViewModels.Product
{
    public class ProductsViewModel : LoginViewModel
    {
        public ProductsViewModel()
        {
            SortList = new List<SelectListItem>
            {
                new("Sort by Newest", "Newest"),
                new("Sort by Popularity", "Popularity"),
                new("Sort by cheapest", "Price-Cheapest"),
                new("Sort by most expensive", "Price-Most-Expensive")
            };
        }

        public IEnumerable<Models.Product.Product> Products { get; set; }
        public IEnumerable<CategoryTreeView> CategoriesTreeViews { get; set; }
        public IEnumerable<SelectListItem> SortList { get; set; }

        public string SortBy { get; set; }
        public IEnumerable<int> SelectedCategories { get; set; }
        public Order Orders { get; set; }

        [Required(ErrorMessage = "Please enter your comment.")]
        public string CommentDescription { get; set; }

        public int ParentCommentId { get; set; }

        public Models.Product.Product Product { get; set; }

        public byte[] Picture { get; set; }
    }
}