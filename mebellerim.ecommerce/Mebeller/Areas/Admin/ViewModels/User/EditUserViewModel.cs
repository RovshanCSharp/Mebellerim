using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mebeller.Data.Utilities.CustomValidationAttribute;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mebeller.Areas.Admin.ViewModels.User
{
    public class EditUserViewModel
    {
        public string UserId { get; set; }

        [Required()]
        [RegularExpression("^[a-z0-9]*$")]
        [MaxLength(75)]
        public string UserName { get; set; }

        [EmailAddress] [Required()] public string Email { get; set; }
        [DataType(DataType.PhoneNumber)] public string MobileNumber { get; set; }

        [DataType(DataType.Password)]
        [MaxLength(16)]
        [MinLength(8)]
        public string UserPassword { get; set; }

        public IEnumerable<SelectListItem> Roles { get; set; }
        public string UserRoleName { get; set; }
        [MaxLength(250)] public string FirstName { get; set; }
        [MaxLength(250)] public string LastName { get; set; }
        [MaxLength(250)] public string UserProvince { get; set; }

        [MaxLength(250)]
        [RequiredIfNotNull(nameof(UserProvince))]
        public string UserCity { get; set; }

        public string UserAddress { get; set; }
        public string UserZipCode { get; set; }
        public byte[] Picture { get; set; }
    }
}