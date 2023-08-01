using System.Threading.Tasks;
using Mebeller.Models.Media;

namespace Mebeller.Data.Repositories.Interfaces
{
    public interface IFileRepository : IGeneralRepository
    {
        Task<Image> GetImageAsync(int imageId);
        Task AddImageAsync(Image image);
        void DeleteImageAsync(Image image);
    }
}
