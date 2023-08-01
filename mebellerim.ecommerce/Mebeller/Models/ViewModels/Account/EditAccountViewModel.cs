using System.ComponentModel.DataAnnotations;
using Mebeller.Data.Utilities.CustomValidationAttribute;

namespace Mebeller.Models.ViewModels.Account
{
    public class EditAccountViewModel : LoginViewModel
    {
        #region Personal Information

        [MaxLength(250)] public string FirstName { get; set; }
        [MaxLength(250)] public string LastName { get; set; }

        [Required(ErrorMessage = "Please enter the desired username")]
        [RegularExpression("^[a-z0-9]*$", ErrorMessage = "Username can only contain numbers and lowercase")]
        [MaxLength(75)]
        public string UserName { get; set; }

        public new string Email { get; set; }

        [DataType(DataType.PhoneNumber, ErrorMessage = "The entered mobile number is not valid")]
        public string MobileNumber { get; set; }

        #endregion

        #region Location Information

        [MaxLength(250)] public string UserProvince { get; set; }

        [MaxLength(250)]
        [RequiredIfNotNull(nameof(UserProvince), ErrorMessage = "If you choose the province, choose your city")]
        public string UserCity { get; set; }

        public string UserAddress { get; set; }
        public string UserZipCode { get; set; }

        #endregion

        #region Passwords

        [DataType(DataType.Password)]
        [MaxLength(16)]
        [MinLength(8)]
        [RequiredIfNotNull(nameof(UserNewPassword), ErrorMessage = "Enter the current password as well.")]
        public string UserCurrentPassword { get; set; }

        [DataType(DataType.Password)]
        [MaxLength(16)]
        [MinLength(8)]
        [RequiredIfNotNull(nameof(UserCurrentPassword), ErrorMessage = "Enter the current password as well.")]
        public string UserNewPassword { get; set; }

        [DataType(DataType.Password)]
        [MaxLength(16)]
        [MinLength(8)]
        [Compare(nameof(UserNewPassword), ErrorMessage = "Enter the current password as well.")]
        public string ReNewPassword { get; set; }

        [DataType(DataType.Password)]
        [MaxLength(16)]
        [MinLength(8)]
        public string UserFirstPassword { get; set; }

        [DataType(DataType.Password)]
        [MaxLength(16)]
        [MinLength(8)]
        [Compare(nameof(UserFirstPassword))]
        public string ReFirstPassword { get; set; }

        #endregion
    }
}