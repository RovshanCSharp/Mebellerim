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
    public class BlogPostCommentRepository : IBlogPostCommentRepository
    {
        private readonly AppDbContext _appDbContext;

        public BlogPostCommentRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;

        public async Task<BlogPostComment> AddAsync(BlogPostComment blogPostComment)
        {
            await _appDbContext.BlogPostComment.AddAsync(blogPostComment);
            await _appDbContext.SaveChangesAsync();
            return blogPostComment;
        }

        public async Task<IEnumerable<BlogPostComment>> GetAllAsync(Guid blogPostId) =>
            await _appDbContext.BlogPostComment.Where(x => x.BlogPostId == blogPostId)
                .ToListAsync();

        public async Task<BlogPostComment> GetAsync(Guid commentId)
        {
            var comment = await _appDbContext.BlogPostComment.FindAsync(commentId);

            return comment;
        }

        public async Task DeleteAsync(Guid commentId)
        {
            var comment = await _appDbContext.BlogPostComment.FindAsync(commentId);

            switch (comment)
            {
                case null:
                    return;
                default:
                    _appDbContext.BlogPostComment.Remove(comment);
                    await _appDbContext.SaveChangesAsync();
                    break;
            }
        }
    }
}