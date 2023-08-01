using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.ViewModels.Page;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Models.Media;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Data.Services
{
    public class PageService : IPageService
    {
        private readonly IPageRepository _pageRepository;
        public PageService(IPageRepository pageRepository) => _pageRepository = pageRepository;
        public async Task<Page> GetPageAsync(int pageId) =>  await _pageRepository.GetPageAsync(pageId);
        public async Task<Page> GetPageByPathAddressAsync(string pagePathAddress) => await _pageRepository.GetPageByPathAddressAsync(pagePathAddress);
        public async Task<IEnumerable<Page>> GetPagesAsync() => await _pageRepository.GetPagesAsync();
       
        public async Task<JsonResult> IsPagePathAddressExistForAddJsonResultAsync(string pagePathAddress) =>
            await _pageRepository
                .IsPagePathAddressExist(pagePathAddress) ? new JsonResult("This page address already exists") : new JsonResult(true);

        public async Task<JsonResult> IsPagePathAddressExistForEditJsonResultAsync(string newPagePathAddress, int pageId) =>
            (await GetPageAsync(pageId)).PagePathAddress != newPagePathAddress
                ? await _pageRepository
                    .IsPagePathAddressExist(newPagePathAddress)
                    ? new JsonResult("This page address already exists")
                    : new JsonResult(true)
                : new JsonResult(true);

        public async Task<PageCreateUpdateResult> CreatePageAsync(AddPageViewModel pageViewModel)
        {
            try
            {
                var isPagePathAddressExist =
                    await _pageRepository
                        .IsPagePathAddressExist(pageViewModel.PagePathAddress);

                if (isPagePathAddressExist)
                    return PageCreateUpdateResult.PathAddressExist;

                var page = new Page()
                {
                    PageTitle = pageViewModel.PageTitle,
                    PagePathAddress = pageViewModel.PagePathAddress,
                    PageDescription = pageViewModel.PageDescription
                };

                await _pageRepository
                    .CreatePageAsync(page);

                await _pageRepository
                    .SaveAsync();

                return PageCreateUpdateResult.Successful;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return PageCreateUpdateResult.Failed;
            }
        }
        public async Task<PageCreateUpdateResult> UpdatePageAsync(EditPageViewModel pageViewModel)
        {
            try
            {
                var currentPage =
                    await GetPageAsync(pageViewModel.PageId);

                if (currentPage.PagePathAddress != pageViewModel.PagePathAddress)
                {
                    var isNewPagePathAddressExist =
                        await _pageRepository
                            .IsPagePathAddressExist(pageViewModel.PagePathAddress);

                    if (isNewPagePathAddressExist)
                    {
                        return PageCreateUpdateResult.PathAddressExist;
                    }
                }

                currentPage.PageTitle = pageViewModel.PageTitle;
                currentPage.PagePathAddress = pageViewModel.PagePathAddress;
                currentPage.PageDescription = pageViewModel.PageDescription;

                _pageRepository
                    .UpdatePage(currentPage);

                await _pageRepository
                    .SaveAsync();

                return PageCreateUpdateResult.Successful;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return PageCreateUpdateResult.Failed;
            }
        }
        public async Task<bool> DeletePageByIdAsync(int pageId)
        {
            try
            {
                var page =
                    await GetPageAsync(pageId);

                if (page == null)
                {
                    return false;
                }

                _pageRepository
                    .DeletePage(page);

                await _pageRepository
                    .SaveAsync();

                return true;
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                return false;
            }
        }
    }
}
