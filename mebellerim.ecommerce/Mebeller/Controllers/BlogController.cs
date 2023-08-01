using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Mebeller.Data;
using Mebeller.Data.Context;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Data.Utilities;
using Mebeller.Models.Blog;
using Mebeller.Models.ViewModels.Home;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json.Linq;

namespace Mebeller.Controllers;

public class BlogController : Controller
{
    private readonly IBlogPostRepository _blogPostRepository;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IProductService _productService;
    private readonly IBlogPostCommentRepository _blogPostCommentRepository;
    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public BlogController(IBlogPostRepository blogPostRepository, SignInManager<ApplicationUser> signInManager,
        UserManager<ApplicationUser> userManager, IBlogPostCommentRepository blogPostCommentRepository,
        IConfiguration configuration, HttpClient httpClient, IProductService productService)
    {
        _blogPostRepository = blogPostRepository;
        _signInManager = signInManager;
        _userManager = userManager;
        _blogPostCommentRepository = blogPostCommentRepository;
        _configuration = configuration;
        _httpClient = httpClient;
        _productService = productService;
        _blogPostRepository = blogPostRepository;
    }


    [HttpGet("/blog/{urlHandle}")]
    public async Task<IActionResult> Details(string urlHandle) => View(await LoadBlog(urlHandle));

    [HttpPost("/blog/{urlHandle}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AddComment(string urlHandle, Guid blogPostId, string commentDescription)
    {
        string recaptchaResponse = Request.Form["g-recaptcha-response"];
        const string url = "https://www.google.com/recaptcha/api/siteverify";
        var response = await _httpClient.PostAsync(
            $"{url}?secret={_configuration["reCAPTCHA:SecretKey"]}&response={recaptchaResponse}",
            new StringContent(""));
        dynamic jsonResponse = JObject.Parse(await response.Content.ReadAsStringAsync());
        if (jsonResponse.success != true)
        {
            ModelState.AddModelError("", "Failed to verify Google CAPTCHA. Please try again later.");
            return RedirectToAction(nameof(Details), new { urlHandle });
        }

        if (_signInManager.IsSignedIn(User) && !string.IsNullOrWhiteSpace(commentDescription))
        {
            var userId = _userManager.GetUserId(User);
            var comment = new BlogPostComment
            {
                BlogPostId = blogPostId,
                Description = commentDescription,
                DateAdded = DateTime.Now,
                UserId = Guid.Parse(userId)
            };
            await _blogPostCommentRepository.AddAsync(comment);
        }

        return RedirectToAction(nameof(Details), new { urlHandle });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteComment(string urlHandle, Guid commentId)
    {
        var comment = await _blogPostCommentRepository.GetAsync(commentId);
        if (comment != null)
        {
            // Check if the user is authorized to delete the comment
            var userId = _userManager.GetUserId(User);
            if (comment.UserId == Guid.Parse(userId)) await _blogPostCommentRepository.DeleteAsync(commentId);
        }

        return RedirectToAction(nameof(Details), new { urlHandle });
    }

    private async Task<IndexViewModel> LoadBlog(string urlHandle)
    {
        var blogPost = await _blogPostRepository.GetAsync(urlHandle);
        if (blogPost == null)
        {
            return null;
        }

        var model = new IndexViewModel { BlogPost = blogPost, BlogPostId = blogPost.Id };
        if (_signInManager.IsSignedIn(User))
        {
            model.Comments = await LoadComments(blogPost.Id);
            model.Orders = await _productService.GetLoggedInUserOpenOrderAsync();
        }

        return model;
    }

    [HttpGet("/Blog/")]
    public async Task<IActionResult> List(int pageNumber = 1, string sortBy = null, string search = null,
        IEnumerable<int> selectedCategories = null) {
        var blogPosts = await _blogPostRepository.GetAllAsync();

        if (!string.IsNullOrEmpty(search))
        {
            blogPosts = blogPosts.Where(p =>
                p.PageTitle.Contains(search, StringComparison.OrdinalIgnoreCase) ||
                p.Id.ToString().Contains(search, StringComparison.OrdinalIgnoreCase));
        }

        if (selectedCategories != null && selectedCategories.Any())
        {
            blogPosts = blogPosts.Where(p =>
                p.Categories.Any(c => selectedCategories.Contains(c.Id)));
        }

        var categoriesTreeViews = await _productService.GetCategoriesTreeViewsAsync();
        var blogModel = new IndexViewModel
        {
            CategoriesTreeViews = categoriesTreeViews,
            SelectedCategories = selectedCategories,
            SortBy = sortBy
        };

        if (!blogPosts.Any())
        {
            ViewData["isEmpty"] = true;
            return View(blogModel);
        }

        const int pageSize = 12;
        var page = new Paging<BlogPost>(blogPosts, pageSize, pageNumber);

        if (pageNumber < page.FirstPage || pageNumber > page.LastPage)
        {
            return NotFound();
        }

        ViewData["pageNumber"] = pageNumber;
        ViewData["firstPage"] = page.FirstPage;
        ViewData["lastPage"] = page.LastPage;
        ViewData["prevPage"] = page.PreviousPage;
        ViewData["nextPage"] = page.NextPage;
        ViewData["isEmpty"] = false;

        blogModel.BlogPosts = page.QueryResult;

        return View(blogModel);
    }

    private async Task<List<BlogComment>> LoadComments(Guid blogPostId)
    {
        var blogPostComments = await _blogPostCommentRepository.GetAllAsync(blogPostId);
        var blogCommentsViewModel = new List<BlogComment>();
        foreach (var blogPostComment in blogPostComments)
        {
            var user = await _userManager.FindByIdAsync(blogPostComment.UserId.ToString());
            var blogComment = new BlogComment
            {
                Id = blogPostComment.Id,
                DateAdded = blogPostComment.DateAdded,
                Description = blogPostComment.Description,
                Username = user.UserName
            };
            blogCommentsViewModel.Add(blogComment);
        }

        return blogCommentsViewModel;
    }
}