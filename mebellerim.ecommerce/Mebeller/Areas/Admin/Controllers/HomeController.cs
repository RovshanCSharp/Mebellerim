using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Home;
using Mebeller.Data.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mebeller.Areas.Admin.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Admin")]
    public class HomeController : Controller
    {
        private readonly IAccountService _accountService;
        private readonly IMediaService _mediaService;
        private readonly IProductService _productService;
        private readonly IVisitorService _visitorService;

        public HomeController(IAccountService accountService, IMediaService mediaService,
            IProductService productService, IVisitorService visitorService)
        {
            _accountService = accountService;
            _mediaService = mediaService;
            _productService = productService;
            _visitorService = visitorService;
        }

        public async Task<IActionResult> Index()
        {
            var usersCount = await _accountService.GetUsersCountAsync() - 1;
            var productsCount = await _productService.GetProductsCountAsync();
            var completedOrdersCount = await _productService.GetCompletedOrdersCountAsync();
            var messagesCount = await _mediaService.GetMessagesCountAsync();
            var numberOfVisits = await _visitorService.GetNumberOfVisitsAsync();
            var uncompletedOrdersCount = await _productService.GetUnCompletedOrdersCountAsync();
            var comments = await _mediaService.GetCommentsAsync();
            var adminDashboardViewModel = new AdminDashboardViewModel
            {
                UsersCount = usersCount,
                ProductsCount = productsCount,
                CompletedOrdersCount = completedOrdersCount,
                MessagesCount = messagesCount,
                NumberOfVisits = numberOfVisits,
                UncompletedOrdersCount = uncompletedOrdersCount,
                Comments = comments
            };
            return View(adminDashboardViewModel);
        }
    }
}