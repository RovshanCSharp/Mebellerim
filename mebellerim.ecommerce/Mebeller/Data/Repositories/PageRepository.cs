using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Data.Context;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Models.Media;
using Microsoft.EntityFrameworkCore;

namespace Mebeller.Data.Repositories
{
    public class PageRepository : IPageRepository
    {
        private readonly AppDbContext _context;
        public PageRepository(AppDbContext context) => _context = context;
        public async Task<Page> GetPageAsync(int pageId) => await _context.Pages.FindAsync(pageId);

        public async Task<Page> GetPageByPathAddressAsync(string pagePathAddress) =>
            await _context.Pages.SingleOrDefaultAsync(p => p.PagePathAddress == pagePathAddress);

        public async Task<IEnumerable<Page>> GetPagesAsync() => await _context.Pages.ToListAsync();

        public async Task<bool> IsPagePathAddressExist(string pagePathAddress) =>
            await _context.Pages.AnyAsync(p => p.PagePathAddress == pagePathAddress);

        public async Task CreatePageAsync(Page page) => await _context.AddAsync(page);
        public void UpdatePage(Page page) => _context.Update(page);
        public void DeletePage(Page page) => _context.Remove(page);
        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}