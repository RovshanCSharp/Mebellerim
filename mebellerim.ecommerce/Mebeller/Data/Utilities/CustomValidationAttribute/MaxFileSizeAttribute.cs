using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Http;

namespace Mebeller.Data.Utilities.CustomValidationAttribute
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;
        public MaxFileSizeAttribute(int maxFileSize) => _maxFileSize = maxFileSize;

        protected override ValidationResult IsValid(object value, ValidationContext validationContext) =>
            value is not IFormFile file
                ? ValidationResult.Success
                :
                file.Length > _maxFileSize
                    ?
                    new ValidationResult($"The file size should not exceed {_maxFileSize / 1024} KB")
                    : ValidationResult.Success;
    }
}