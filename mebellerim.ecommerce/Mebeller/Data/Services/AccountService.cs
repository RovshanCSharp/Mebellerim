using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Areas.Admin.ViewModels.User;
using Mebeller.Config;
using Mebeller.Data.Context;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Data.Utilities;
using Mebeller.Models.ViewModels.Account;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;

namespace Mebeller.Data.Services;

public class AccountService : IAccountService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserRepository _userRepository;
    private readonly LinkGenerator _linkGenerator;
    private readonly IHttpContextAccessor _accessor;
    private readonly IEmailService _emailService;

    public AccountService(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager,
        LinkGenerator linkGenerator, IHttpContextAccessor accessor, IEmailService emailService,
        RoleManager<IdentityRole> roleManager, IUserRepository userRepository)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _linkGenerator = linkGenerator ?? throw new ArgumentNullException(nameof(linkGenerator));
        _accessor = accessor ?? throw new ArgumentNullException(nameof(accessor));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
        _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
    }

    public async Task<IdentityResult> CreateUserAsync(ApplicationUser user, string password)
    {
        user.RegisterTime = DateTime.Now;
        user.UserDetails = new UserDetails();
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded) return result;
        await _userManager.AddToRoleAsync(user, SeedUsers.UserRoleName);
        return result;
    }

    public async Task<bool> SendEmailConfirmationAsync(ApplicationUser user, string returnController,
        string returnAction)
    {
        try
        {
            var emailConfirmationToken = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            var url = _linkGenerator.GetUriByAction(_accessor.HttpContext ?? throw new InvalidOperationException(),
                returnAction, returnController, new { email = user.Email, token = emailConfirmationToken },
                _accessor.HttpContext?.Request.Scheme);
            var emailMessage = await ViewToStringRenderer.RenderViewToStringAsync(
                _accessor.HttpContext?.RequestServices, "~/Views/Emails/EmailConfirmationTemplate.cshtml", url);
            await _emailService.SendEmailAsync(user.Email, "Email Verification", emailMessage, true);
            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }

    public async Task<bool> ConfirmEmailAsync(string email, string token)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null) return false;

        var result = await _userManager.ConfirmEmailAsync(user, token);
        return result.Succeeded;
    }

    public async Task<JsonResult> CheckIfUserNameExistsAsync(string userName)
    {
        var isUserExist = await _userManager.Users.AnyAsync(p => p.UserName == userName);
        return isUserExist ? new JsonResult("Unable to use this username") : new JsonResult(true);
    }

    public async Task<JsonResult> CheckIfEmailExistsAsync(string userEmail)
    {
        var isUserExist = await _userManager.Users.AnyAsync(p => p.Email == userEmail);
        return isUserExist ? new JsonResult("Unable to use this email") : new JsonResult(true);
    }

    public bool IsUserSignedIn()
    {
        var isUserSignedIn = _signInManager.IsSignedIn(_accessor.HttpContext?.User);
        return isUserSignedIn;
    }

    public async Task<IEnumerable<AuthenticationScheme>> GetExternalAuthenticationSchemesAsync()
    {
        var externalLogins = await _signInManager.GetExternalAuthenticationSchemesAsync();
        return externalLogins;
    }

    public ChallengeResult ConfigureExternalLogins(string provider, string controllerName, string actionName,
        string returnUrl = null)
    {
        var redirectUrl = _linkGenerator.GetUriByAction(_accessor.HttpContext ?? throw new InvalidOperationException(),
            actionName, controllerName, new { returnurl = returnUrl }, _accessor.HttpContext?.Request.Scheme);
        var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
        return new ChallengeResult(provider, properties);
    }

    public async Task<bool> ConfigureExternalLoginsCallBacksGetUriByActionAsync(string remoteError = null)
    {
        if (remoteError != null) return false;

        var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();
        if (externalLoginInfo == null) return false;

        var signInResult = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider,
            externalLoginInfo.ProviderKey, false, true);
        if (signInResult.Succeeded) return true;

        var userEmail = externalLoginInfo.Principal.FindFirstValue(ClaimTypes.Email);
        if (userEmail == null) return false;

        var userName = userEmail.Split("@")[0];
        var userDetails = new UserDetails();
        var user = new ApplicationUser
        {
            UserName = userName.Length <= 10 ? userName : userName[..7],
            Email = userEmail,
            EmailConfirmed = true,
            RegisterTime = DateTime.Now,
            UserDetails = userDetails
        };
        await _userManager.CreateAsync(user);
        await _userManager.AddToRoleAsync(user, SeedUsers.UserRoleName);
        await _userManager.AddLoginAsync(user, externalLoginInfo);
        await _signInManager.SignInAsync(user, false);
        return true;
    }

    public async Task<LoginWithPasswordResult> LoginWithPasswordAsync(LoginViewModel loginViewModel)
    {
        try
        {
            var user = await _userManager.FindByEmailAsync(loginViewModel.Email);
            if (user == null) return LoginWithPasswordResult.Failed;

            if (!user.EmailConfirmed) return LoginWithPasswordResult.EmailNotConfirmed;

            var signInResult = await _signInManager.PasswordSignInAsync(user.UserName, loginViewModel.Password,
                loginViewModel.RememberMe, false);
            if (signInResult.Succeeded) return LoginWithPasswordResult.Successful;

            if (signInResult.IsLockedOut) return LoginWithPasswordResult.LockedOut;

            if (signInResult.RequiresTwoFactor) return LoginWithPasswordResult.RequiresTwoFactor;

            if (signInResult.IsNotAllowed) return LoginWithPasswordResult.NotAllowed;

            return LoginWithPasswordResult.Failed;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return LoginWithPasswordResult.Failed;
        }
    }

    public enum LoginWithPasswordResult
    {
        Successful,
        Failed,
        LockedOut,
        RequiresTwoFactor,
        NotAllowed,
        EmailNotConfirmed
    }

    public async Task<bool> LogOutUserAsync()
    {
        await _signInManager.SignOutAsync();
        return true;
    }

    public async Task<bool> SendResetPasswordLinkAsync(string userEmail, string returnController, string returnAction)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null) return false;
        var restPasswordToken = await _userManager.GeneratePasswordResetTokenAsync(user);
        var resetPasswordUrl = _linkGenerator.GetUriByAction(
            _accessor.HttpContext ?? throw new InvalidOperationException(), returnAction, returnController,
            new { email = user.Email, token = restPasswordToken }, _accessor.HttpContext?.Request.Scheme);
        var emailMessage = CreateResetPasswordEmailMessage(resetPasswordUrl);
        await _emailService.SendEmailAsync(user.Email, "Change password", await emailMessage, true);
        return true;
    }

    private async Task<string> CreateResetPasswordEmailMessage(string resetPasswordUrl)
    {
        var emailTemplatePath = "~/Views/Emails/RestPasswordEmailTemplate.cshtml";
        return await ViewToStringRenderer.RenderViewToStringAsync(_accessor.HttpContext?.RequestServices,
            emailTemplatePath, resetPasswordUrl);
    }

    public async Task<bool> ResetPasswordAsync(string userEmail, string token, string newPassword)
    {
        var user = await _userManager.FindByEmailAsync(userEmail);
        if (user == null) return false;
        var result = await _userManager.ResetPasswordAsync(user, token, newPassword);
        if (result.Succeeded) return true;
        foreach (var error in result.Errors)
            Console.WriteLine($"Error resetting password for user {userEmail}: {error.Description}");

        return false;
    }

    public async Task<bool> DoesUserHavePasswordAsync(ApplicationUser user)
    {
        return await _userManager.HasPasswordAsync(user);
    }

    public async Task<bool> DoesUserHavePasswordByIdAsync(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return false;
        return await DoesUserHavePasswordAsync(user);
    }

    public async Task<bool> DoesLoggedInUserHavePasswordAsync()
    {
        var loggedUser = await GetLoggedUserAsync();
        return await DoesUserHavePasswordAsync(loggedUser);
    }

    public async Task<IdentityResult> EditAccountAsync(EditAccountViewModel editAccountViewModel)
    {
        var loggedUser = await GetLoggedUserWithDetailsAsync();
        IdentityResult result;
        if (!string.IsNullOrEmpty(editAccountViewModel.UserCurrentPassword) ||
            !string.IsNullOrEmpty(editAccountViewModel.UserFirstPassword))
        {
            var isLoggedUserHasPassword = await DoesUserHavePasswordAsync(loggedUser);
            if (isLoggedUserHasPassword)
            {
                var isCurrentPasswordValid =
                    await _userManager.CheckPasswordAsync(loggedUser, editAccountViewModel.UserCurrentPassword);
                if (!isCurrentPasswordValid)
                {
                    var errorMessage = new IdentityError { Description = "Password is incorrect" };
                    result = IdentityResult.Failed(errorMessage);
                    return result;
                }

                result = await _userManager.RemovePasswordAsync(loggedUser);
                if (!result.Succeeded) return result;
                result = await _userManager.AddPasswordAsync(loggedUser, editAccountViewModel.UserNewPassword);
                if (!result.Succeeded) return result;
            }
            else
            {
                result = await _userManager.AddPasswordAsync(loggedUser, editAccountViewModel.UserFirstPassword);
                if (!result.Succeeded) return result;
            }
        }

        if (loggedUser != null)
        {
            loggedUser.UserName = editAccountViewModel.UserName;
            loggedUser.MobileNumber = editAccountViewModel.MobileNumber;
            if (loggedUser.UserDetails != null)
            {
                loggedUser.UserDetails.FirstName = editAccountViewModel.FirstName;
                loggedUser.UserDetails.LastName = editAccountViewModel.LastName;
                loggedUser.UserDetails.UserProvince = editAccountViewModel.UserProvince;
                loggedUser.UserDetails.UserCity = editAccountViewModel.UserCity;
                loggedUser.UserDetails.UserAddress = editAccountViewModel.UserAddress;
                loggedUser.UserDetails.UserZipCode = editAccountViewModel.UserZipCode;
            }
        }

        result = await _userManager.UpdateAsync(loggedUser);
        return result;
    }

    public Task<IEnumerable<UserViewModel>> GetUsersAsync()
    {
        var users = _userManager.Users.ToList();
        var result = users.Select(p => new UserViewModel
        {
            UserId = p.Id,
            UserName = p.UserName,
            UserEmail = p.Email,
            UserRoleName = GetUserRoleAsync(p).Result,
            RegisterTime = p.RegisterTime
        });
        return Task.FromResult(result);
    }

    public async Task<UserDetailsViewModel> GetUserAsync(string userId)
    {
        var requestedUser = await _userManager.Users.Include(u => u.UserDetails).Include(u => u.UserComments)
            .ThenInclude(c => c.Product).SingleOrDefaultAsync(u => u.Id == userId);
        var requestedUserRole = await GetUserRoleAsync(requestedUser);
        var loggedUserRole = await GetLoggedUserRoleAsync();
        const string adminRoleName = SeedUsers.AdminRoleName;
        const string userRoleName = SeedUsers.UserRoleName;
        if (!(loggedUserRole == adminRoleName && requestedUserRole == adminRoleName) &&
            requestedUserRole != userRoleName)
            return null;

        return new UserDetailsViewModel { User = requestedUser, UserRoleName = requestedUserRole };
    }

    public async Task<int> GetUsersCountAsync()
    {
        return await _userManager.Users.CountAsync();
    }

    public async Task<EditUserViewModel> GetEditableUserAsync(string userId)
    {
        var requestedUser = await _userManager.Users
            .Include(p => p.UserDetails).SingleOrDefaultAsync(p => p.Id == userId);
        if (requestedUser == null)
            // Handle the case where the user is not found
            return null;

        var requestedUserRole = await GetUserRoleAsync(requestedUser);

        var roles = await GetRolesAsync();
        var user = new EditUserViewModel
        {
            UserId = requestedUser.Id,
            UserName = requestedUser.UserName,
            Email = requestedUser.Email,
            MobileNumber = requestedUser.MobileNumber,
            Roles = roles,
            UserRoleName = requestedUserRole,
            FirstName = requestedUser.UserDetails?.FirstName,
            LastName = requestedUser.UserDetails?.LastName,
            UserProvince = requestedUser.UserDetails?.UserProvince,
            UserCity = requestedUser.UserDetails?.UserCity,
            UserAddress = requestedUser.UserDetails?.UserAddress,
            UserZipCode = requestedUser.UserDetails?.UserZipCode
        };
        return user;
    }

    public async Task<bool> DeleteUserAsync(string userId)
    {
        try
        {
            var requestedUser = await _userManager.Users.Include(u => u.UserDetails)
                .FirstOrDefaultAsync(u => u.Id == userId);
            if (requestedUser == null) return false;

            _userRepository.DeleteUserDetails(requestedUser.UserDetails);
            var result = await _userManager.DeleteAsync(requestedUser);
            if (!result.Succeeded) return false;

            await _userRepository.SaveAsync();
            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }

    public async Task<IEnumerable<SelectListItem>> GetRolesAsync()
    {
        var roles = await _roleManager.Roles.OrderBy(role => role.Name).ToListAsync();
        var result = roles.Select(role => new SelectListItem(role.Name, role.Name));
        return result;
    }

    public async Task<IdentityResult> CreateUserByAdminAsync(CreateUserViewModel userViewModel)
    {
        var userDetail = new UserDetails
        {
            FirstName = userViewModel.FirstName,
            LastName = userViewModel.LastName,
            UserProvince = userViewModel.UserProvince,
            UserCity = userViewModel.UserCity,
            UserAddress = userViewModel.UserAddress,
            UserZipCode = userViewModel.UserZipCode
        };
        var user = new ApplicationUser
        {
            UserName = userViewModel.UserName,
            Email = userViewModel.Email,
            MobileNumber = userViewModel.MobileNumber,
            EmailConfirmed = true,
            RegisterTime = DateTime.UtcNow,
            UserDetails = userDetail
        };
        var result = await _userManager.CreateAsync(user, userViewModel.UserPassword);
        if (!result.Succeeded) return result;

        var loggedUserRole = await GetLoggedUserRoleAsync();
        var userRole = DetermineUserRole(loggedUserRole, userViewModel.UserRoleName);
        await _userManager.AddToRoleAsync(user, userRole);
        return result;
    }

    private static string DetermineUserRole(string loggedUserRole, string requestedRole)
    {
        const string admin = SeedUsers.AdminRoleName;
        const string user = SeedUsers.UserRoleName;
        if ((loggedUserRole == admin && requestedRole is admin) || requestedRole != admin) return user;

        return requestedRole;
    }

    public async Task<IdentityResult> EditUserByAdminAsync(EditUserViewModel userViewModel)
    {
        var user = await _userManager.Users.Include(p => p.UserDetails)
            .SingleOrDefaultAsync(p => p.Id == userViewModel.UserId);
        user.UserDetails ??= new UserDetails();
        user.UserName = userViewModel.UserName;
        user.Email = userViewModel.Email;
        user.MobileNumber = userViewModel.MobileNumber;
        user.UserDetails.FirstName = userViewModel.FirstName;
        user.UserDetails.LastName = userViewModel.LastName;
        user.UserDetails.UserProvince = userViewModel.UserProvince;
        user.UserDetails.UserCity = userViewModel.UserCity;
        user.UserDetails.UserAddress = userViewModel.UserAddress;
        user.UserDetails.UserZipCode = userViewModel.UserZipCode;
        var oldUserRole = await GetUserRoleAsync(user);
        if (oldUserRole != userViewModel.UserRoleName)
        {
            var loggedUserRole = await GetLoggedUserRoleAsync();
            if (!((loggedUserRole == SeedUsers.AdminRoleName &&
                   userViewModel.UserRoleName == SeedUsers.AdminRoleName) ||
                  (userViewModel.UserRoleName != SeedUsers.UserRoleName &&
                   userViewModel.UserRoleName != SeedUsers.UserRoleName)))
            {
                await _userManager.RemoveFromRoleAsync(user, oldUserRole);
                await _userManager.AddToRoleAsync(user, userViewModel.UserRoleName);
            }
        }

        if (!string.IsNullOrWhiteSpace(userViewModel.UserPassword))
        {
            var isUserHasPassword = await DoesUserHavePasswordAsync(user);
            if (isUserHasPassword) await _userManager.RemovePasswordAsync(user);

            await _userManager.AddPasswordAsync(user, userViewModel.UserPassword);
        }

        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded) return result;
        await _userManager.UpdateSecurityStampAsync(user);
        return result;
    }

    public async Task<bool> UpdateUserAsync(ApplicationUser user)
    {
        var result = await _userManager.UpdateAsync(user);
        return result.Succeeded;
    }

    public async Task<ApplicationUser> GetLoggedUserAsync()
    {
        var userId = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId)) return null;
        return await _userManager.FindByIdAsync(userId);
    }

    public async Task<ApplicationUser> GetLoggedUserWithFavoriteProductsAsync()
    {
        var loggedUserId = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var loggedUserWithFavoriteProducts = await _userManager.Users.Include(p => p.FavoriteProducts)
            .SingleOrDefaultAsync(p => p.Id == loggedUserId);
        return loggedUserWithFavoriteProducts;
    }

    public async Task<ApplicationUser> GetLoggedUserWithDetailsAsync()
    {
        var loggedUserId = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var loggedUser = await _userManager.Users.Include(p => p.UserDetails).Include(p => p.UserOrders)
            .Include(p => p.FavoriteProducts).ThenInclude(p => p.Images)
            .SingleOrDefaultAsync(p => p.Id == loggedUserId);
        return loggedUser;
    }

    public async Task<bool> IsProductInLoggedUserFavoriteProductsAsync(int productId)
    {
        var loggedUserId = _accessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier);
        var isProductInLoggedUserFavoriteProducts = await _userManager.Users.Include(p => p.FavoriteProducts)
            .AnyAsync(p => p.Id == loggedUserId && p.FavoriteProducts.Any(product => product.ProductId == productId));
        return isProductInLoggedUserFavoriteProducts;
    }

    public async Task<string> GetUserRoleAsync(ApplicationUser user)
    {
        var userRoles = await _userManager.GetRolesAsync(user);
        var userRole = userRoles.FirstOrDefault();
        return userRole;
    }

    public async Task<string> GetLoggedUserRoleAsync()
    {
        var user = await GetLoggedUserAsync();
        var userRoles = await _userManager.GetRolesAsync(user);
        var userRole = userRoles.FirstOrDefault();
        return userRole;
    }
}