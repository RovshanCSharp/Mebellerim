using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.ViewModels.User;
using Mebeller.Data.Context;
using Mebeller.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Mebeller.Data.Services.Interfaces
{
    public interface IAccountService
    {
        bool IsUserSignedIn();
        Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password);
        Task<bool> SendEmailConfirmationAsync(ApplicationUser user, string controllerName, string actionName);
        Task<bool> ConfirmEmailAsync(string email, string token);
        Task<JsonResult> CheckIfUserNameExistsAsync(string userName);
        Task<JsonResult> CheckIfEmailExistsAsync(string email);
        Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync();
        ChallengeResult ConfigureExternalLogins(string provider, string controllerName, string actionName,
            string returnUrl);
        Task<bool> ConfigureExternalLoginsCallBacksGetUriByActionAsync(string remoteError = null);
        Task<AccountService.LoginWithPasswordResult> LoginWithPasswordAsync(LoginViewModel loginViewModel); 
        Task<bool> LogOutUserAsync();
        Task<bool> SendResetPasswordLinkAsync(string userEmail, string returnController, string returnAction);
        Task<bool> ResetPasswordAsync(string userEmail, string token, string newPassword);
        Task<bool> DoesUserHavePasswordAsync(ApplicationUser user);
        Task<bool> DoesUserHavePasswordByIdAsync(string userId);  
        Task<bool> DoesLoggedInUserHavePasswordAsync();
        Task<IdentityResult> EditAccountAsync(EditAccountViewModel editAccountViewModel); 

        Task<IEnumerable<UserViewModel>> GetUsersAsync();
        Task<UserDetailsViewModel> GetUserAsync(string userId);
        Task<int> GetUsersCountAsync();
        Task<EditUserViewModel> GetEditableUserAsync(string userId);
        Task<bool> DeleteUserAsync(string userId);
        Task<IEnumerable<SelectListItem>> GetRolesAsync();
        Task<IdentityResult> CreateUserByAdminAsync(CreateUserViewModel userViewModel);
        Task<IdentityResult> EditUserByAdminAsync(EditUserViewModel userViewModel);
        Task<bool> UpdateUserAsync(ApplicationUser loggedUser);
        Task<ApplicationUser> GetLoggedUserAsync();
        Task<ApplicationUser> GetLoggedUserWithFavoriteProductsAsync();
        Task<ApplicationUser> GetLoggedUserWithDetailsAsync();
        Task<bool> IsProductInLoggedUserFavoriteProductsAsync(int productId);
        Task<string> GetUserRoleAsync(ApplicationUser user);
        Task<string> GetLoggedUserRoleAsync();
    }
}
