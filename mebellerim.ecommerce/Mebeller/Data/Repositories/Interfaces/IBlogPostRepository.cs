using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Models.Blog;

namespace Mebeller.Data.Repositories.Interfaces
{
    public interface IBlogPostRepository
    {
        Task<IEnumerable<BlogPost>> GetAllAsync();
        Task<IEnumerable<BlogPost>> GetAllAsync(string tagName);
        Task<BlogPost> GetAsync(Guid id);
        Task<BlogPost> GetAsync(string urlHandle);
        Task<BlogPost> AddAsync(BlogPost blogPost);
        Task<BlogPost> UpdateAsync(BlogPost blogPost);
        Task<bool> DeleteAsync(Guid id);
    }
}
