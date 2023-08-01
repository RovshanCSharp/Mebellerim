using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Areas.Admin.ViewModels.Page;

public class EditPageViewModel
{
    public int PageId { get; set; }

    [Required(ErrorMessage = "Please enter a title for the page.")]
    [MaxLength(300, ErrorMessage = "The title you entered is too long.")]
    public string PageTitle { get; set; }

    [Required(ErrorMessage = "Please enter an address for the page.")]
    [MaxLength(300, ErrorMessage = "The address you entered is too long.")]
    [RegularExpression("^[A-Za-z0-9/-]+$", ErrorMessage = "The page address can only include letters, numbers, -, and /.")]
    [Remote(action: "IsPagePathAddressExistForEdit", controller: "Page/Admin", areaName: "Admin",
        HttpMethod = "POST", ErrorMessage = "This address is already in use by another page.")]
    public string PagePathAddress { get; set; }

    public string PageDescription { get; set; }
}