using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Models.ViewModels.Account
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Please enter the desired username")]
        [RegularExpression("^[a-z0-9]*$", ErrorMessage = "Username can only contain numbers and lowercase")]
        [MaxLength(75)]
        [Remote("IsUserNameExist", "Account", HttpMethod = "POST", ErrorMessage = "This username is already taken")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Please enter your email address")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [MaxLength(256)]
        [Remote("IsEmailExist", "Account", HttpMethod = "POST", ErrorMessage = "This email is already registered")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Please enter the desired password")]
        [DataType(DataType.Password)]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        public string Password { get; set; }

        [Required(ErrorMessage = "Please enter a repeat of the password")]
        [DataType(DataType.Password)]
        [StringLength(16, MinimumLength = 8, ErrorMessage = "Password must be at least 8 characters long")]
        [Compare(nameof(Password), ErrorMessage = "Repeat password does not match password")]
        public string RePassword { get; set; }
    }
}