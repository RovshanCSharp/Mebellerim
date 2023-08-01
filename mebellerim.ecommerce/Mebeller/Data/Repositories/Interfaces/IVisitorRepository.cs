using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Media;

namespace Mebeller.Data.Repositories.Interfaces
{
    public interface IVisitorRepository : IGeneralRepository
    {
        Task<IEnumerable<Visitor>> GetVisitorsAsync();  
        Task<Visitor> GetVisitorByIpAddressAsync(string visitorIpAddress);
        Task<int> GetNumberOfVisitsAsync();
        Task AddVisitorAsync(Visitor visitor);   
        void UpdateVisitor(Visitor visitor);
    }
}
