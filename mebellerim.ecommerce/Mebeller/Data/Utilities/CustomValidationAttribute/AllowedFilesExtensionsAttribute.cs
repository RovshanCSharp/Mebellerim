using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Mebeller.Data.Utilities.CustomValidationAttribute;

public class AllowedFilesExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;
    public AllowedFilesExtensionsAttribute(string[] extensions) => _extensions = extensions;

    protected override ValidationResult IsValid(
        object value, ValidationContext validationContext) {
        if (value is IEnumerable<IFormFile> files)
        {
            var invalidExtensions = files
                .Select(file => Path.GetExtension(file.FileName).ToLower())
                .Where(extension => !_extensions.Contains(extension))
                .Distinct()
                .ToList();

            if (invalidExtensions.Any())
            {
                var errorMessage = $"The following file extensions are not allowed: {string.Join(", ", invalidExtensions)}";
                return new ValidationResult(errorMessage);
            }
        }

        return ValidationResult.Success;
    }
}