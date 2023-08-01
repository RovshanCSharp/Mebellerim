using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Blog;
using Mebeller.Data.Context.Enums;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Data.Utilities;
using Mebeller.Models.Blog;
using Mebeller.Models.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = "Admin")]
public class BlogController : Controller
{
    private readonly IBlogPostRepository _blogPostRepository;

    public BlogController(IBlogPostRepository blogPostRepository) => _blogPostRepository = blogPostRepository;

    [HttpGet("/Admin/Blogs")]
    public async Task<IActionResult> List(string searchQuery, int pageNumber = 1)
    {
        IEnumerable<BlogPost> blogPosts = (await _blogPostRepository.GetAllAsync()).ToList();

        blogPosts = string.IsNullOrEmpty(searchQuery) switch
        {
            false => blogPosts.Where(p => p.PageTitle.Contains(searchQuery, StringComparison.OrdinalIgnoreCase) ||
                                          p.Id.ToString().Contains(searchQuery, StringComparison.OrdinalIgnoreCase)),
            _ => blogPosts
        };

        var page = new Paging<BlogPost>(blogPosts, 10, pageNumber);
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

        var pageQueryResult = page.QueryResult;

        if (string.IsNullOrEmpty(searchQuery) && !pageQueryResult.Any())
        {
            ViewBag.SearchNotFound = true;
            return View();
        }

        ViewBag.SearchNotFound = !pageQueryResult.Any();

        var notificationJson = (string)TempData["Notification"];
        if (notificationJson != null)
        {
            ViewData["Notification"] = JsonSerializer.Deserialize<Notification>(notificationJson);
        }

        // Check if no matching blog posts found
        ViewBag.SearchNotFound = !blogPosts.Any();

        return View(blogPosts);
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(Guid id)
    {
        var deleted = await _blogPostRepository.DeleteAsync(id);
        if (deleted)
        {
            var notification = new Notification
            {
                Type = NotificationType.Success, Message = "Blog was deleted successfully!"
            };
            TempData["Notification"] = JsonSerializer.Serialize(notification);
            return RedirectToAction("List", "Blog", new { area = "Admin" });
        }

        return RedirectToAction("Index", "Home");
    }

    [HttpGet("/Admin/Blogs/Add")]
    public IActionResult Add() => View();

    [HttpPost("/Admin/Blogs/Add")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Add(AddBlogPost addBlogPostRequest, IFormFile featuredImage, string tags)
    {
        ValidateAddBlogPost(addBlogPostRequest);
        if (ModelState.IsValid)
        {
            var blogPost = new BlogPost
            {
                Heading = addBlogPostRequest.Heading,
                PageTitle = addBlogPostRequest.PageTitle,
                Content = addBlogPostRequest.Content,
                ShortDescription = addBlogPostRequest.ShortDescription,
                FeaturedImageUrl = addBlogPostRequest.FeaturedImageUrl,
                UrlHandle = addBlogPostRequest.UrlHandle,
                PublishedDate = addBlogPostRequest.PublishedDate,
                Author = addBlogPostRequest.Author,
                Visible = addBlogPostRequest.Visible,
                Tags = tags.Split(',').Select(x => new Tag { Name = x.Trim() }).ToList()
            };

            // Handle the file upload and save the featured image
            if (featuredImage != null)
            {
                // Save the featured image file using the appropriate logic
                // Example: await _fileService.SaveFeaturedImageAsync(blogPost, featuredImage);
            }

            await _blogPostRepository.AddAsync(blogPost);
            var notification = new Notification { Type = NotificationType.Success, Message = "New blog created!" };
            TempData["Notification"] = JsonSerializer.Serialize(notification);
            return RedirectToAction("List", "Blog", new { area = "Admin" });
        }

        return View(addBlogPostRequest);
    }

    private void ValidateAddBlogPost(AddBlogPost addBlogPostRequest)
    {
        if (addBlogPostRequest.PublishedDate.Date < DateTime.Now.Date)
        {
            ModelState.AddModelError("AddBlogPostRequest.PublishedDate",
                "PublishedDate can only be today's date or a future date.");
        }
    }
}