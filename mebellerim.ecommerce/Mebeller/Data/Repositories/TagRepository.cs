using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mebeller.Data.Context;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Models.Media;
using Microsoft.EntityFrameworkCore;

namespace Mebeller.Data.Repositories
{
    public class TagRepository : ITagRepository
    {
        private readonly AppDbContext _appDbContext;

        public TagRepository(AppDbContext appDbContext) => _appDbContext = appDbContext;


        public async Task<IEnumerable<Tag>> GetAllAsync()
        {
            var tags = await _appDbContext.Tags.ToListAsync();

            return tags.DistinctBy(x => x.Name.ToLower());
        }
    }
}
