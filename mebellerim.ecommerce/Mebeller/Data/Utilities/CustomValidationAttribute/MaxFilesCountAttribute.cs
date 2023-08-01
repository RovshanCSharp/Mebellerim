using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Http;

namespace Mebeller.Data.Utilities.CustomValidationAttribute;

public class MaxFilesCountAttribute : ValidationAttribute   
{
    private readonly int _maxFileCount;
    public MaxFilesCountAttribute(int maxFileCount) => _maxFileCount = maxFileCount;

    protected override ValidationResult IsValid(object value, ValidationContext validationContext) =>
        value is not IEnumerable<IFormFile> files ? ValidationResult.Success :
        files.Count() > _maxFileCount ? new ValidationResult($"Maximum {_maxFileCount} files allowed") :
        ValidationResult.Success;
}