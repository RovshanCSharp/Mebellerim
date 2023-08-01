using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mebeller.Data.Context;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Models.Blog;
using Microsoft.EntityFrameworkCore;

namespace Mebeller.Data.Repositories
{
    public class BlogPostRepository : IBlogPostRepository
    {
        private readonly AppDbContext _appDbContext;

        public BlogPostRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;

        public async Task<BlogPost> AddAsync(BlogPost blogPost)
        {
            await _appDbContext.BlogPosts.AddAsync(blogPost);
            await _appDbContext.SaveChangesAsync();
            return blogPost;
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var existingBlog = await _appDbContext.BlogPosts.FindAsync(id);

            if (existingBlog != null)
            {
                _appDbContext.BlogPosts.Remove(existingBlog);
                await _appDbContext.SaveChangesAsync();
                return true;
            }

            return false;
        }

        public async Task<IEnumerable<BlogPost>> GetAllAsync() => await _appDbContext.BlogPosts.Include(nameof(BlogPost.Tags)).ToListAsync();

        public async Task<IEnumerable<BlogPost>> GetAllAsync(string tagName) =>
            await (_appDbContext.BlogPosts.Include(nameof(BlogPost.Tags))
                    .Where(x => x.Tags.Any(x => x.Name == tagName)))
                .ToListAsync();

        public async Task<BlogPost> GetAsync(Guid id) =>
            await _appDbContext.BlogPosts.Include(nameof(BlogPost.Tags))
                .FirstOrDefaultAsync(x => x.Id == id);

        public async Task<BlogPost> GetAsync(string urlHandle) =>
            await _appDbContext.BlogPosts.Include(nameof(BlogPost.Tags))
                .FirstOrDefaultAsync(x => x.UrlHandle == urlHandle);

        public async Task<BlogPost> UpdateAsync(BlogPost blogPost)
        {
            var existingBlogPost = await _appDbContext.BlogPosts.Include(nameof(BlogPost.Tags))
                .FirstOrDefaultAsync(x => x.Id == blogPost.Id);

            if (existingBlogPost != null)
            {
                existingBlogPost.Heading = blogPost.Heading;
                existingBlogPost.PageTitle = blogPost.PageTitle;
                existingBlogPost.Content = blogPost.Content;
                existingBlogPost.ShortDescription = blogPost.ShortDescription;
                existingBlogPost.FeaturedImageUrl = blogPost.FeaturedImageUrl;
                existingBlogPost.UrlHandle = blogPost.UrlHandle;
                existingBlogPost.PublishedDate = blogPost.PublishedDate;
                existingBlogPost.Author = blogPost.Author;
                existingBlogPost.Visible = blogPost.Visible;

                if (blogPost.Tags !=null && blogPost.Tags.Any())
                {
                    // Delete the existing tags
                    _appDbContext.Tags.RemoveRange(existingBlogPost.Tags);

                    // Add new tags
                    blogPost.Tags.ToList().ForEach(x => x.BlogPostId = existingBlogPost.Id);
                    await _appDbContext.Tags.AddRangeAsync(blogPost.Tags);
                }
            }

            await _appDbContext.SaveChangesAsync();
            return existingBlogPost;
        }
    }
}
