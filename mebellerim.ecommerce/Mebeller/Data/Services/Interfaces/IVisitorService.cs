using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Media;

namespace Mebeller.Data.Services.Interfaces
{
    public interface IVisitorService
    {
        Task<IEnumerable<Visitor>> GetVisitorsAsync();
        Task<int> GetNumberOfVisitsAsync();
        Task<bool> AddOrUpdateVisitorAsync(string visitorIpAddress);
    }
}
