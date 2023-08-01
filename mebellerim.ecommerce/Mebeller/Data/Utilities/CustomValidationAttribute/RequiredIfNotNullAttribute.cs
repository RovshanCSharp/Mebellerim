using System.ComponentModel.DataAnnotations;

namespace Mebeller.Data.Utilities.CustomValidationAttribute
{
    public class RequiredIfNotNullAttribute : ValidationAttribute
    {
        private readonly RequiredAttribute _innerAttribute;
        private readonly string _principleProperty;

        public RequiredIfNotNullAttribute(string principleProperty)
        {
            _innerAttribute= new RequiredAttribute();
            _principleProperty = principleProperty;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var principleProperty = validationContext.ObjectType.GetProperty(_principleProperty);
            var principlePropertyValue = principleProperty?.GetValue(validationContext.ObjectInstance, null);
            return principlePropertyValue == null ? ValidationResult.Success :
                !_innerAttribute.IsValid(value) ? new ValidationResult(ErrorMessage) : ValidationResult.Success;
        }
    }
}
