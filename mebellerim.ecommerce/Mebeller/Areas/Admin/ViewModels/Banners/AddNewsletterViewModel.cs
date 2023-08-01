using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Areas.Admin.ViewModels.Banners;

public class AddNewsletterViewModel
{
    [Required(ErrorMessage = "Please enter an email address.")]
    [EmailAddress(ErrorMessage = "The email address you entered is not valid.")]
    [MaxLength(256, ErrorMessage = "The email address you entered is too long.")]
    [Remote(action: "IsNewsletterEmailExist", controller: "Media/Admin", areaName: "Admin",
        HttpMethod = "POST", ErrorMessage = "This email address is already subscribed to the newsletter.")]
    public string CustomerEmail { get; set; }
}