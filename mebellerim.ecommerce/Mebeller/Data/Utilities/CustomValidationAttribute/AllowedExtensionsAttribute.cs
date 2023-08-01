using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Mebeller.Data.Utilities.CustomValidationAttribute;

public class AllowedExtensionsAttribute : ValidationAttribute
{
    private readonly string[] _extensions;
    public AllowedExtensionsAttribute(string[] extensions) => _extensions = extensions;
        
    protected override ValidationResult IsValid(object value, ValidationContext validationContext) => 
        value is not IFormFile file ? ValidationResult.Success :
        !_extensions.Contains(Path.GetExtension(file.FileName).ToLower()) ? new ValidationResult(ErrorMessage) :
        ValidationResult.Success;
}
