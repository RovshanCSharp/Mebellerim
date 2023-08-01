using System.ComponentModel.DataAnnotations;
using Mebeller.Data.Utilities.CustomValidationAttribute;
using Microsoft.AspNetCore.Http;

namespace Mebeller.Areas.Admin.ViewModels.Banners
{
    public class AddBannerViewModel
    {
        [MaxLength(200)]
        public string BannerTitle { get; set; }

        [Url(ErrorMessage = "Invalid banner link")]
        public string BannerLink { get; set; }

        public string BannerDescription { get; set; }

        [AllowedExtensions(new[] { ".png", ".jpg", ".jpeg", ".gif" }, ErrorMessage = "Only png, jpg, jpeg, gif formats are allowed")]
        [MaxFileSize(4194304, ErrorMessage = "Maximum banner photo size is 4 MB")]
        [Required(ErrorMessage = "Please select a banner photo")]
        [DataType(DataType.Upload)]
        public IFormFile BannerImage { get; set; }

        [Required(ErrorMessage = "Please specify if this is the primary banner")]
        public bool IsPrimaryBanner { get; set; }
     }
}