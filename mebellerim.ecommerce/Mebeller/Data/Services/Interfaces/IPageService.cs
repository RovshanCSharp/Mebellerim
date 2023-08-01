using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.ViewModels.Page;
using Mebeller.Models.Media;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Data.Services.Interfaces
{
    public interface IPageService
    {
        Task<Page> GetPageAsync(int pageId);
        Task<Page> GetPageByPathAddressAsync(string pagePathAddress);
        Task<IEnumerable<Page>> GetPagesAsync();
        Task<JsonResult> IsPagePathAddressExistForAddJsonResultAsync(string pagePathAddress);
        Task<JsonResult> IsPagePathAddressExistForEditJsonResultAsync(string newPagePathAddress, int pageId);
        Task<PageCreateUpdateResult> CreatePageAsync(AddPageViewModel pageViewModel);
        Task<PageCreateUpdateResult> UpdatePageAsync(EditPageViewModel pageViewModel);
        Task<bool> DeletePageByIdAsync(int pageId);
    }
}
