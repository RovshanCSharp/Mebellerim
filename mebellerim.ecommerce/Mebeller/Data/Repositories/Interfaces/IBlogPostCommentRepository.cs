using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Models.Blog;

namespace Mebeller.Data.Repositories.Interfaces
{
    public interface IBlogPostCommentRepository
    {
        Task<BlogPostComment> AddAsync(BlogPostComment blogPostComment);

        Task<IEnumerable<BlogPostComment>> GetAllAsync(Guid blogPostId);
        Task<BlogPostComment> GetAsync(Guid commentId);
        Task DeleteAsync(Guid commentId);    }
}