using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Mebeller.Data.Utilities.CustomValidationAttribute
{
    public class MaxFilesSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFilesSizeAttribute(int maxFileSize) => _maxFileSize = maxFileSize;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) =>
            value is not IEnumerable<IFormFile> files ? ValidationResult.Success :
            files.Any(p => p.Length > _maxFileSize) ? new ValidationResult(GetErrorMessage()) :
            ValidationResult.Success;

        private string GetErrorMessage() => $"Maximum allowable size per file is { _maxFileSize}";
    }
}
