using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Models.Media;

namespace Mebeller.Data.Repositories.Interfaces
{
    public interface IPageRepository : IGeneralRepository
    {
        Task<Page> GetPageAsync(int pageId);
        Task<Page> GetPageByPathAddressAsync(string pagePathAddress);
        Task<IEnumerable<Page>> GetPagesAsync();
        Task<bool> IsPagePathAddressExist(string pagePathAddress);
        Task CreatePageAsync(Page page);
        void UpdatePage(Page page);
        void DeletePage(Page page);
    }
}
