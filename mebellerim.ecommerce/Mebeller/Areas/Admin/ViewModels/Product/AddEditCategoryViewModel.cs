using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mebeller.Areas.Admin.ViewModels.Product
{
    public class AddEditCategoryViewModel
    {
        [Key]
        public int CategoryId { get; set; }

        [Required(ErrorMessage = "Please enter a name for the category.")]
        [MaxLength(250, ErrorMessage = "The name you entered is too long.")]
        public string CategoryName { get; set; }

        public string CategoryDescription { get; set; }

        public int ParentCategoryId { get; set; }

        public IEnumerable<SelectListItem> AllCategories { get; set; }
    }
}