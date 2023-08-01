using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Data;
using Mebeller.Data.Context;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Data.Utilities;
using Mebeller.Models.ViewModels.Product;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;
using StringContent = System.Net.Http.StringContent;

namespace Mebeller.Controllers;

public class MediaController : Controller
{
    private readonly IMediaService _mediaService;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IProductService _productService;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public MediaController(IMediaService mediaService, IProductService productService, IConfiguration configuration, UserManager<ApplicationUser> userManager)
    {
        _mediaService = mediaService;
        _productService = productService;
        _configuration = configuration;
        _userManager = userManager;
        _httpClient = new HttpClient();
    }

    [HttpGet("/Contact-Us")]
    public IActionResult ContactUs() => View();

    [HttpPost("/Contact-Us")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ContactUs(Message model)
    {
        string recaptchaResponse = Request.Form["g-recaptcha-response"];
        const string url = "https://www.google.com/recaptcha/api/siteverify";
        var response = await _httpClient.PostAsync($"{url}?secret={_configuration["reCAPTCHA:SecretKey"]}&response={recaptchaResponse}", new StringContent(""));
        dynamic jsonResponse = JObject.Parse(await response.Content.ReadAsStringAsync());

        if (jsonResponse.success != true)
            ModelState.AddModelError("", "A problem occurred when Google CAPTCHA was verified, please try later");
        else if (ModelState.IsValid && await _mediaService.AddMessageAsync(model))
        {
            ViewData["SuccessMessage"] = "Your message has been sent successfully.";
            ModelState.Clear();
            return View();
        }

        ModelState.AddModelError("", "A problem occurred while sending the message.");
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddCommentToProduct(ProductsViewModel model)
    {
        string recaptchaResponse = Request.Form["g-recaptcha-response"];
        const string url = "https://www.google.com/recaptcha/api/siteverify";
        var response = await _httpClient.PostAsync($"{url}?secret={_configuration["reCAPTCHA:SecretKey"]}&response={recaptchaResponse}", new StringContent(""));

        dynamic jsonResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
        if (jsonResponse.success != true)
        {
            ModelState.AddModelError("", "Failed to verify Google reCAPTCHA. Please try again later.");
        }
        else
        {
            var productId = Convert.ToInt32(TempData["productId"]);
            var product = await _productService.GetProductWithDetailsAsync(productId);
            model.Product = product;
            var result = await _mediaService.AddCommentToProductAsync(model);

            product.Comments = product.Comments.Where(p => p.IsConfirmed && p.ParentComment == null).ToList();

            if (result)
            {
                ModelState.Clear();
                model.CommentDescription = null;
                ViewData["SuccessMessage"] = "Your comment has been submitted successfully and will be displayed after admin approval.";
            }
            else
            {
                ModelState.AddModelError("", "Failed to submit the comment.");
            }
        }

        return View("/Views/Product/ProductDetails.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddNewsletter(string customerEmail) => 
        !ModelState.IsValid ? Content("Failed") :
        string.IsNullOrWhiteSpace(customerEmail) ? Content("EmptyError") :
        !customerEmail.IsValidEmailAddress() ? Content("InvalidEmail") :
        customerEmail.Length >= 256 ? Content("MoreThan256Character") :
        await _mediaService.IsEmailExistInNewslettersCustomersAsync(customerEmail) ? Content("CustomerEmailExist") :
        Content(await _mediaService.AddNewsletterAsync(customerEmail) ? "Successful" : "Failed");
    
    [HttpPost("/Comments/DeleteComment")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(int commentId, string returnUrl = null)
    {
        var comment = await _mediaService.GetCommentAsync(commentId);
        var user = await _userManager.GetUserAsync(User);

        if (User.Identity is { IsAuthenticated: true } && comment?.User?.Id == user?.Id)
        {
            var result = await _mediaService.DeleteCommentByIdAsync(commentId);
            
            if (result)
            {
                if (string.IsNullOrEmpty(returnUrl))
                    returnUrl = Url.Action("Index", "Product");

                if (returnUrl != null)
                    return Redirect(returnUrl);
            }
        }

        return NotFound();
    }
    
}