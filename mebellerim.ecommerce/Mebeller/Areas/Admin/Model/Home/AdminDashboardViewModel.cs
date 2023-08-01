using System.Collections.Generic;
using Mebeller.Models.Media;

namespace Mebeller.Areas.Admin.Model.Home
{
    public class AdminDashboardViewModel
    {
        public int UsersCount { get; set; }
        public int ProductsCount { get; set; }
        public int CompletedOrdersCount { get; set; }
        public int MessagesCount { get; set; }
        public int NumberOfVisits { get; set; }
        public int UncompletedOrdersCount { get; set; }
        public IEnumerable<Comment> Comments { get; set; }
    }
}