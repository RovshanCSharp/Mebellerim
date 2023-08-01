using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Models.Media;
using Mebeller.Models.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Data.Services;

public class FileService : IFileService
{
    private readonly IFileRepository _fileRepository;

    public FileService(IFileRepository fileRepository) => _fileRepository = fileRepository;

    public async Task<JsonResult> UploadEditorFileAsync(IFormFile file)
    {
        if (file is null || file.Length == 0)
        {
            return null;
        }

        var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName).ToLower()}";
        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Editor", fileName);

        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream);

        var url = $"/img/Editor/{fileName}";

        return new JsonResult(new SuccessfulUploadResult
        {
            Uploaded = 1,
            FileName = fileName,
            Url = url
        });
    }
    public async Task<IEnumerable<Image>> GetProductImagesByIdsAsync(IEnumerable<int> productImagesIds) => await Task.WhenAll(productImagesIds.Select(id => _fileRepository.GetImageAsync(id)));

    public async Task<bool> AddProductImagesAsync(Product product, IEnumerable<IFormFile> productImagesFiles)
    {
        var monthProductImagesDirName = $"{DateTime.Now:yyyy - MM}";
        var productImagesPath = Path.Combine(Directory.GetCurrentDirectory(), 
            "wwwroot", "img", "Products", monthProductImagesDirName);
            
        Directory.CreateDirectory(productImagesPath);

        foreach (var productImageFile in productImagesFiles) {
            var productImageFileName = $"{Guid.NewGuid()} - {productImageFile.FileName.ToLower()}";
            var fullProductImagesPath = Path.Combine(productImagesPath, productImageFileName);

            await using var stream = new FileStream(fullProductImagesPath, FileMode.Create);
            await productImageFile.CopyToAsync(stream);

            var productImage = new Image
            {
                ImagePath = $"Products/{monthProductImagesDirName}/{productImageFileName}",
                Product = product
            };

            await _fileRepository.AddImageAsync(productImage);
        }

        return true;
    }
    public async Task<bool> DeleteProductImagesByIdsAsync(IEnumerable<int> productImagesIds)
    {
        var productImages = await GetProductImagesByIdsAsync(productImagesIds);

        foreach (var productImage in productImages)
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", productImage.ImagePath);
            if (File.Exists(imagePath)) File.Delete(imagePath);
            _fileRepository.DeleteImageAsync(productImage);
        }
        return true;
    }
    public bool DeleteProductImages(IEnumerable<Image> productImages)
    {
        foreach (var productImage in productImages)
        {
            var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", productImage.ImagePath);
            if (File.Exists(imagePath))
            {
                File.Delete(imagePath);
            }

            _fileRepository.DeleteImageAsync(productImage);
        }
        return true;
    }
    public async Task<bool> AddBannerImageAsync(Banner banner, IFormFile uploadedBannerImage)
    {
        var bannerImagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", "Banners");
        Directory.CreateDirectory(bannerImagePath);

        var bannerImageFileName = $"{Guid.NewGuid()} - {uploadedBannerImage.FileName.ToLower()}";
        var fullBannerImagePath = Path.Combine(bannerImagePath, bannerImageFileName);

        using var stream = new FileStream(fullBannerImagePath, FileMode.Create);
        await uploadedBannerImage.CopyToAsync(stream);

        var bannerImage = new Image { ImagePath = $"Banners/{bannerImageFileName}" };
        await _fileRepository.AddImageAsync(bannerImage);

        banner.Image = bannerImage;
        return true;
    }
    public bool DeleteBannerImage(Image bannerImage)
    {
        var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "img", bannerImage.ImagePath);
        if (File.Exists(imagePath))
        {
            File.Delete(imagePath);
        }

        _fileRepository.DeleteImageAsync(bannerImage);
        return true;
    }
}