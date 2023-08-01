using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Models.Media;
using Mebeller.Models.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Data.Services.Interfaces
{
    public interface IFileService
    {
        Task<JsonResult> UploadEditorFileAsync(IFormFile file);
        Task<IEnumerable<Image>> GetProductImagesByIdsAsync(IEnumerable<int> productImagesIds);
        Task<bool> AddProductImagesAsync(Product product, IEnumerable<IFormFile> productImagesFiles);
        Task<bool> DeleteProductImagesByIdsAsync(IEnumerable<int> productImagesIds);
        bool DeleteProductImages(IEnumerable<Image> productImages);

        Task<bool> AddBannerImageAsync(Banner banner, IFormFile uploadedBannerImage);
        bool DeleteBannerImage(Image bannerImage);
    }
}
