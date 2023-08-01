using System.Threading.Tasks;
using Mebeller.Data.Context;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Models.Media;

namespace Mebeller.Data.Repositories
{
    public class FileRepository : IFileRepository
    {
        private readonly AppDbContext _context;
        public FileRepository(AppDbContext context) => _context = context;
        public async Task<Image> GetImageAsync(int imageId) => await _context.Images.FindAsync(imageId);
        public async Task AddImageAsync(Image image) => await _context.AddAsync(image);
        public void DeleteImageAsync(Image image) => _context.Remove(image);
        public async Task SaveAsync() => await _context.SaveChangesAsync();
    }
}