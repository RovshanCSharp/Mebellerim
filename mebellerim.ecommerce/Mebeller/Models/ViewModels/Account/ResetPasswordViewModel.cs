using System.ComponentModel.DataAnnotations;

namespace Mebeller.Models.ViewModels.Account
{
    public class ResetPasswordViewModel
    {
        public string Email { get; set; }
        public string Token { get; set; }

        [Required(ErrorMessage = "Please enter a new password.")]
        [DataType(DataType.Password)]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "Password should be between 8 and 16 characters.")]
        public string NewPassword { get; set; }

        [Required(ErrorMessage = "Please repeat the new password.")]
        [DataType(DataType.Password)]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "Password should be between 8 and 16 characters.")]
        [Compare(nameof(NewPassword), ErrorMessage = "Passwords do not match.")]
        public string ReNewPassword { get; set; }
    }
}