#nullable disable

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Mebeller.Data;
using Mebeller.Data.Context;
using Mebeller.Data.Utilities.CustomValidationAttribute;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mebeller.Areas.Identity.Pages.Account.Manage;

public class IndexModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public IndexModel(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
    }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public string Username { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [TempData]
    public string StatusMessage { get; set; }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    [BindProperty]
    public InputModel Input { get; set; }

    public string UserId { get; set; }

    [Required(ErrorMessage = "Please enter the desired username")]
    [RegularExpression("^[a-z0-9]*$", ErrorMessage = "Username can only contain numbers and lowercase")]
    [MaxLength(75)]
    public string UserName { get; set; }

    public string Email { get; set; }

    [DataType(DataType.PhoneNumber, ErrorMessage = "The entered mobile number is not valid")]
    [StringLength(11, MinimumLength = 11, ErrorMessage = "The entered mobile number is not valid")]
    public string MobileNumber { get; set; }

    [DataType(DataType.Password)]
    [MaxLength(16, ErrorMessage = "Up to 16 characters are allowed")]
    [MinLength(8, ErrorMessage = "At least 8 characters are allowed")]
    public string UserPassword { get; set; }

    public IEnumerable<SelectListItem> Roles { get; set; }
    public string UserRoleName { get; set; }

    [MaxLength(250)] public string FirstName { get; set; }

    [MaxLength(250)] public string LastName { get; set; }

    [MaxLength(250)] public string UserProvince { get; set; }

    [MaxLength(250)]
    [RequiredIfNotNull(nameof(UserProvince), ErrorMessage = "If you choose the province, choose your city")]
    public string UserCity { get; set; }

    public string UserAddress { get; set; }

    [StringLength(10, MinimumLength = 10)] public string UserZipCode { get; set; }

    [DataType(DataType.Password)]
    [MaxLength(16, ErrorMessage = "Up to 16 characters are allowed")]
    [MinLength(8, ErrorMessage = "At least 8 characters are allowed")]
    [RequiredIfNotNull(nameof(UserNewPassword), ErrorMessage = "Enter the current password as well")]
    public string UserCurrentPassword { get; set; }

    [DataType(DataType.Password)]
    [MaxLength(16, ErrorMessage = "Up to 16 characters are allowed")]
    [MinLength(8, ErrorMessage = "At least 8 characters are allowed")]
    [RequiredIfNotNull(nameof(UserCurrentPassword), ErrorMessage = "Enter a new password")]
    public string UserNewPassword { get; set; }

    [DataType(DataType.Password)]
    [MaxLength(16, ErrorMessage = "Up to 16 characters are allowed")]
    [MinLength(8, ErrorMessage = "At least 8 characters are allowed")]
    [Compare(nameof(UserNewPassword), ErrorMessage = "Repeat password does not match password")]
    public string ReNewPassword { get; set; }

    [DataType(DataType.Password)]
    [MaxLength(16)]
    [MinLength(8)]
    public string UserFirstPassword { get; set; }

    [DataType(DataType.Password)]
    [MaxLength(16)]
    [MinLength(8)]
    [Compare(nameof(UserFirstPassword), ErrorMessage = "Repeat password does not match password")]
    public string ReFirstPassword { get; set; }


    private async Task LoadAsync(ApplicationUser user)
    {
        var userName = await _userManager.GetUserNameAsync(user);
        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        var firstName = user.FirstName;
        var lastName = user.LastName;
        var picture = user.Picture;
        var userEmail = user.Email;


        Input = new InputModel
        {
            PhoneNumber = phoneNumber,
            FirstName = firstName,
            LastName = lastName,
            Picture = picture,
            UserName = userName,
            Email = userEmail
        };
    }

    public async Task<IActionResult> OnGetAsync()
    {
        var user = await _userManager.GetUserAsync(User);
        if (user == null) return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");

        await LoadAsync(user);
        return Page();
    }

    public async Task<IActionResult> OnPostAsync()
    {
        var user = await _userManager.GetUserAsync(User);

        var firstName = user.FirstName;
        if (Input.FirstName != firstName)
        {
            user.FirstName = Input.FirstName;
            await _userManager.UpdateAsync(user);
        }

        var lastName = user.LastName;
        if (Input.LastName != lastName)
        {
            user.FirstName = Input.FirstName;
            await _userManager.UpdateAsync(user);
        }

        var userName = user.UserName;
        if (Input.UserName != userName)
        {
            user.UserName = Input.UserName;
            await _userManager.UpdateAsync(user);
        }

        if (Request.Form.Files.Count > 0)
        {
            var file = Request.Form.Files.FirstOrDefault();
            using (var dataStream = new MemoryStream())
            {
                await file!.CopyToAsync(dataStream);
                user.Picture = dataStream.ToArray();
            }

            await _userManager.UpdateAsync(user);
        }

        if (!ModelState.IsValid)
        {
            await LoadAsync(user);
            return Page();
        }

        var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
        if (Input.PhoneNumber != phoneNumber)
        {
            var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
            if (!setPhoneResult.Succeeded)
            {
                StatusMessage = "Unexpected error when trying to set phone number.";
                return RedirectToPage();
            }
        }

        await _signInManager.RefreshSignInAsync(user);
        StatusMessage = "Your profile has been updated";
        return RedirectToPage();
    }

    /// <summary>
    ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
    ///     directly from your code. This API may change or be removed in future releases.
    /// </summary>
    public class InputModel
    {
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [Phone]
        [Display(Name = "Phone number")]
        public string PhoneNumber { get; init; }

        [Display(Name = "User Picture")] public byte[] Picture { get; init; }

        [Required] [Display(Name = "Name")] public string FirstName { get; init; }

        [Required] [Display(Name = "Surname")] public string LastName { get; init; }

        [Display(Name = "Email address")] public string Email { get; init; }

        [Required]
        [MaxLength(75)]
        [Display(Name = "Username")]
        public string UserName { get; init; }

        [MaxLength(250)]
        [Display(Name = "Province")]
        public string UserProvince { get; init; }

        [MaxLength(250)]
        [RequiredIfNotNull(nameof(UserProvince), ErrorMessage = "If you choose the province, choose your city")]
        [Display(Name = "City")]
        public string UserCity { get; init; }

        [Display(Name = "Address")] public string UserAddress { get; init; }

        [StringLength(5, MinimumLength = 5, ErrorMessage = "Postal code contains 5 characters")]
        [Display(Name = "Post code")]
        public string UserZipCode { get; init; }
    }
}