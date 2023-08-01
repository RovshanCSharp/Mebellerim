using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mebeller.Data.Utilities.CustomValidationAttribute;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mebeller.Areas.Admin.ViewModels.User
{
    public class CreateUserViewModel
    {
        [Required(ErrorMessage = "Please enter the desired username")]
        [RegularExpression("^[a-z0-9]*$", ErrorMessage = "Username can only contain numbers and lowercase")]
        [MaxLength(75)]
        public string UserName { get; set; }

        [EmailAddress(ErrorMessage = "The entered field is not an email")]
        [Required(ErrorMessage = "Please enter your email address")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the desired password")]
        [DataType(DataType.Password)]
        [MaxLength(16)]
        [MinLength(8)]
        public string UserPassword { get; set; }

        [Required(ErrorMessage = "Please select a user role")]
        public string UserRoleName { get; set; }

        [MaxLength(250)]
        public string FirstName { get; set; }

        [MaxLength(250)]
        public string LastName { get; set; }

        [MaxLength(250)]
        public string UserProvince { get; set; }

        [MaxLength(250)]
        [RequiredIfNotNull(nameof(UserProvince),ErrorMessage = "If you choose the province, choose your city")]
        public string UserCity { get; set; }

        public string UserAddress { get; set; }
        public string UserZipCode { get; set; }
        public IEnumerable<SelectListItem> Roles { get; set; }
        [DataType(DataType.PhoneNumber)]
        public string MobileNumber { get; set; }
    }
}