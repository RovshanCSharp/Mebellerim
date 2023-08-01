using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Blog;
using Mebeller.Data.Context.Enums;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Models.Blog;
using Mebeller.Models.Media;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Mebeller.Pages.Blogs
{
    [Authorize(Roles = "Admin")]
    public class EditModel : PageModel
    {
        private readonly IBlogPostRepository _blogPostRepository;

        [BindProperty]
        public EditBlogPostRequest BlogPost { get; set; }

        [BindProperty]
        [Required]
        public string Tags { get; set; }

        public EditModel(IBlogPostRepository blogPostRepository) => _blogPostRepository = blogPostRepository;

        public async Task OnGet(Guid id)
        {
            var blogPostDomainModel = await _blogPostRepository.GetAsync(id);

            switch (blogPostDomainModel)
            {
                case { Tags: not null }:
                    BlogPost = new EditBlogPostRequest
                    {
                        Id = blogPostDomainModel.Id,
                        Heading = blogPostDomainModel.Heading,
                        PageTitle = blogPostDomainModel.PageTitle,
                        Content = blogPostDomainModel.Content,
                        ShortDescription = blogPostDomainModel.ShortDescription,
                        FeaturedImageUrl = blogPostDomainModel.FeaturedImageUrl,
                        UrlHandle = blogPostDomainModel.UrlHandle,
                        PublishedDate = blogPostDomainModel.PublishedDate,
                        Author = blogPostDomainModel.Author,
                        Visible = blogPostDomainModel.Visible
                    };

                    Tags = string.Join(',', blogPostDomainModel.Tags.Select(x => x.Name));
                    break;
            }
        }

        public async Task<IActionResult> OnPostEdit()
        {
            ValidateEditBlogPost();

            if (ModelState.IsValid)
            {
                try
                {
                    var blogPostDomainModel = new BlogPost
                    {
                        Id = BlogPost.Id,
                        Heading = BlogPost.Heading,
                        PageTitle = BlogPost.PageTitle,
                        Content = BlogPost.Content,
                        ShortDescription = BlogPost.ShortDescription,
                        FeaturedImageUrl = BlogPost.FeaturedImageUrl,
                        UrlHandle = BlogPost.UrlHandle,
                        PublishedDate = BlogPost.PublishedDate,
                        Author = BlogPost.Author,
                        Visible = BlogPost.Visible,
                        Tags = new List<Tag>(Tags.Split(',').Select(x => new Tag() { Name = x.Trim() }))
                    };


                    await _blogPostRepository.UpdateAsync(blogPostDomainModel);

                    ViewData["Notification"] = new Notification
                    {
                        Type = NotificationType.Success,
                        Message = "Record updated successfully!"
                    };
                }
                catch (Exception)
                {
                    ViewData["Notification"] = new Notification
                    {
                        Type = NotificationType.Error,
                        Message = "Something went wrong!"
                    };
                }

                return Page();
            }

            return Page();
        }

        public async Task<IActionResult> OnPostDelete()
        {
            var deleted = await _blogPostRepository.DeleteAsync(BlogPost.Id);
            if (deleted)
            {
                var notification = new Notification
                {
                    Type = NotificationType.Success,
                    Message = "Blog was deleted successfully!"
                };

                TempData["Notification"] = JsonSerializer.Serialize(notification);

                return RedirectToPage("/Admin/Blogs/List");
            }

            return Page();
        }

        private void ValidateEditBlogPost()
        {
            if (!string.IsNullOrWhiteSpace(BlogPost.Heading) && BlogPost.Heading.Length is < 10 or > 72)
            {
                ModelState.AddModelError("BlogPost.Heading", "Heading can only be between 10 and 72 characters.");
            }
        }
    }
}
