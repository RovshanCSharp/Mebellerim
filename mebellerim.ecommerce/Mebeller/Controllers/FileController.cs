using System.Threading.Tasks;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Data.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Controllers;

public class FileController : Controller
{
    private readonly IFileService _fileService;
    public FileController(IFileService fileService) => _fileService = fileService;

    [HttpPost("/UploadEditorFile")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> UploadEditorFile(IFormFile upload) =>
        !Request.IsLocal() ? NotFound() : await _fileService.UploadEditorFileAsync(upload);
}