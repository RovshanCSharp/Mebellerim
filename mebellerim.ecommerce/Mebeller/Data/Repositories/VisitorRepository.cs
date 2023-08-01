using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Data.Context;
using Mebeller.Data.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Mebeller.Data.Repositories
{
    public class VisitorRepository : IVisitorRepository
    {
        private readonly AppDbContext _context;
        public VisitorRepository(AppDbContext context) => _context = context;
        public async Task<IEnumerable<Visitor>> GetVisitorsAsync() => await _context.Visitors.ToListAsync();

        public async Task<Visitor> GetVisitorByIpAddressAsync(string visitorIpAddress) =>
            await _context.Visitors.SingleOrDefaultAsync(p => p.VisitorIpAddress == visitorIpAddress);

        public async Task<int> GetNumberOfVisitsAsync() => await _context.Visitors.SumAsync(p => p.CountOfVisit);
        public async Task AddVisitorAsync(Visitor visitor) => await _context.AddAsync(visitor);
        public void UpdateVisitor(Visitor visitor) => _context.Update(visitor);
        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}