using System.Linq;
using System.Threading.Tasks;
using Mebeller.Data;
using Mebeller.Data.Context;
using Mebeller.Data.Services;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Models.ViewModels.Account;
using Mebeller.Models.ViewModels.Home;
using Mebeller.Models.ViewModels.Product;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Controllers;

public class AccountController : Controller
{
    private readonly IAccountService _accountService;
    private readonly IProductService _productService;
    private readonly UserManager<ApplicationUser> _userManager;
 
    public AccountController(IAccountService accountService, IProductService productService, UserManager<ApplicationUser> userManager)
    {
        _accountService = accountService;
        _productService = productService;
        _userManager = userManager;
    }

    [HttpGet("/Account/Login")]
    public async Task<IActionResult> Login(string returnUrl = null)
    {
        return _accountService.IsUserSignedIn()
            ? RedirectToAction(nameof(HomeController.Index), "Home")
            : View(new LoginViewModel
            {
                ReturnUrl = returnUrl,
                ExternalLogins = (await _accountService.GetExternalAuthenticationSchemesAsync()).ToList()
            });
    }

    [HttpPost("Account/Login")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
    {
        if (_accountService.IsUserSignedIn()) return RedirectToAction(nameof(HomeController.Index), "Home");

        if (ModelState.IsValid)
        {
            var loginResult = await _accountService.LoginWithPasswordAsync(model);
            if (loginResult != AccountService.LoginWithPasswordResult.Successful)
            {
                ModelState.AddModelError(string.Empty,
                    loginResult == AccountService.LoginWithPasswordResult.EmailNotConfirmed
                        ? "To sign in, you need to verify your email"
                        : "The username or password is incorrect.");
                return View(new LoginViewModel
                {
                    ReturnUrl = returnUrl,
                    ExternalLogins = (await _accountService.GetExternalAuthenticationSchemesAsync()).ToList()
                });
            }

            return Redirect((string.IsNullOrEmpty(returnUrl) || !Url.IsLocalUrl(returnUrl)
                ? Url.Action(nameof(HomeController.Index), "Home")
                : returnUrl) ?? string.Empty);
        }

        return View(new LoginViewModel
        {
            ReturnUrl = returnUrl,
            ExternalLogins = (await _accountService.GetExternalAuthenticationSchemesAsync()).ToList()
        });
    }

    [HttpGet("Register")]
    public IActionResult Register() => 
        _accountService.IsUserSignedIn() ? RedirectToAction("Index", "Home") : View();

    [HttpPost("Register")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (_accountService.IsUserSignedIn())
        {
            return RedirectToAction("Index", "Home");
        }

        var user = new ApplicationUser 
        { 
            UserName = model.UserName, 
            Email = model.Email 
        };
        
        var existingUser = await _userManager.FindByEmailAsync(model.Email);
        if (existingUser != null)
        {
            // User already exists, handle the error (e.g., display an error message)
            ModelState.AddModelError(string.Empty, "User already exists.");
            return View(model);
        }

        var result = await _accountService.CreateUserAsync(user, model.Password);
        if (result.Succeeded)
        {
            await _accountService.SendEmailConfirmationAsync(user, "Account", "EmailConfirmation");
            ViewData["SuccessMessage"] = "Registration has been successful, please check your email";
        }
        else
        {
            ModelState.AddModelError("", string.Join(", ", result.Errors.Select(e => e.Description)));
        }
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> LogOut()
    {
        await _accountService.LogOutUserAsync();
        return RedirectToAction("Index", "Home");
    }


    public IActionResult ResetPassword(string email, string token) =>
        string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(token)
            ? RedirectToAction("Index", "Home")
            : View(new ResetPasswordViewModel { Email = email, Token = token });

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid && await _accountService.ResetPasswordAsync(model.Email, model.Token, model.NewPassword))
        {
            return RedirectToAction("Login");
        }

        ModelState.AddModelError("", "An error occurred while changing the password.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ExternalLogins(string provider, string returnUrl) => _accountService.ConfigureExternalLogins(provider, "Account", "ExternalLoginsCallBacks", returnUrl);

    public async Task<IActionResult> ExternalLoginsCallBacks(string returnUrl = null, string remoteError = null)
    {
        returnUrl = Url.IsLocalUrl(returnUrl) ? returnUrl : "~/";
        var result = await _accountService.ConfigureExternalLoginsCallBacksGetUriByActionAsync(remoteError);
        return result ? Redirect(returnUrl) : RedirectToAction("Login");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IsUserNameExist(string userName) =>
        await _accountService.CheckIfUserNameExistsAsync(userName);

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> IsEmailExist(string email) => await _accountService.CheckIfEmailExistsAsync(email);

    public async Task<IActionResult> EmailConfirmation(string email, string token) =>
        await _accountService.ConfirmEmailAsync(email, token) ? View() : NotFound();

    [Authorize]
    [HttpGet("/My-Account")]
    public async Task<IActionResult> ManageAccount()
    {
        var orders = await _productService.GetLoggedInUserOpenOrderAsync();

        var ManageModel = new IndexViewModel()
        {
            Orders = orders
        };

        return View(ManageModel);
    }



    [Authorize]
    [HttpGet("/My-Account/Edit-Account")]
    public async Task<IActionResult> EditAccount()
    {
        var loggedUser =
            await _accountService
                .GetLoggedUserWithDetailsAsync();

        var editAccountViewModel = new IndexViewModel()
        {
            Orders = await _productService.GetLoggedInUserOpenOrderAsync(),
            FirstName = loggedUser.UserDetails?.FirstName,
            LastName = loggedUser.UserDetails?.LastName,
            UserName = loggedUser.UserName,
            Email = loggedUser.Email,
            MobileNumber = loggedUser.MobileNumber,
            UserProvince = loggedUser.UserDetails?.UserProvince,
            UserCity = loggedUser.UserDetails?.UserCity,
            UserAddress = loggedUser.UserDetails?.UserAddress,
            UserZipCode = loggedUser.UserDetails?.UserZipCode
        };


        return View(editAccountViewModel);
    }

    [Authorize]
    [HttpPost("/My-Account/Edit-Account")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditAccount(EditAccountViewModel model)
    {
        if (ModelState.IsValid)
        {
            var result =
                await _accountService.EditAccountAsync(model);

            if (result.Succeeded)
            {
                ModelState.Clear();
                ViewData["SuccessMessage"] = "The information has been successfully edited, you will need to log in again if you change or add a password";
            }

            else
            {
                foreach (var identityError in result.Errors)
                {
                    ModelState.AddModelError("", identityError.Description);
                }
            }
        }

        return View(model);
    }

    [HttpGet("/My-Account/Favorite-Products")]
    public async Task<IActionResult> FavoriteProducts()
    {
        var viewModel = new IndexViewModel
        {
            Products = await _productService.GetLoggedUserFavoriteProductsAsync(),
            Orders = await _productService.GetLoggedInUserOpenOrderAsync()
        };

        return View(viewModel);
    }


    [Authorize]
    [HttpGet("/My-Account/Orders")]
    public async Task<IActionResult> AccountOrders()
    {
        var loggedUserOrders = await _productService.GetLoggedUserOrdersAsync();
        var filteredOrders = loggedUserOrders.Where(p => p.NotEmpty).ToList();

        var viewModel = new IndexViewModel
        {
            OrderList = filteredOrders.Select(o => new OrderViewModel {OrderId = o.OrderId, OrderStatus = o.OrderStatus, CreateTime = o.CreateTime,
                OrderTotalPrice = o.OrderTotalPrice}).ToList(),
            Orders = await _productService.GetLoggedInUserOpenOrderAsync(),
            Products = await _productService.GetProductsAsync()
        };

        return View(viewModel);
    }


    [Authorize]
    [HttpGet("/My-Account/Order/{orderId}")]
    public async Task<IActionResult> AccountOrder(int orderId)
    {
        var orderViewModel =
            await _productService
                .GetLoggedUserOrderInvoiceAsync(orderId);

        if (orderViewModel == null)
            return NotFound();

        ViewData["HeaderTitle"] = "View Order";

        ViewData["Message"] = $"Invoice No. {orderViewModel.OrderId} Mebellerim Store";

        return View("/Views/Product/OrderConfirmation.cshtml", orderViewModel);
    }
}