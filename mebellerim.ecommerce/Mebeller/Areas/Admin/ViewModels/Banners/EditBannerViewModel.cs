using System.ComponentModel.DataAnnotations;
using Mebeller.Data.Utilities.CustomValidationAttribute;
using Microsoft.AspNetCore.Http;

namespace Mebeller.Areas.Admin.ViewModels.Banners
{
    public class EditBannerViewModel
    {
        [MaxLength(200)]
        public string BannerTitle { get; set; }

        public string BannerLink { get; set; }

        public string BannerDescription { get; set; }

        [AllowedExtensions(new[] { ".png", ".jpg", ".jpeg", ".gif" }, ErrorMessage = "Only PNG, JPG, JPEG, and GIF formats are allowed.")]
        [MaxFileSize(4194304, ErrorMessage = "The maximum allowed size for the banner photo is 4 MB.")]
        [DataType(DataType.Upload)]
        public IFormFile BannerImage { get; set; }

        [Required(ErrorMessage = "Please select whether this banner is primary or not.")]
        public bool IsPrimaryBanner { get; set; }

        public int BannerId { get; set; }

        public string BannerCurrentImagePath { get; set; }
    }
}