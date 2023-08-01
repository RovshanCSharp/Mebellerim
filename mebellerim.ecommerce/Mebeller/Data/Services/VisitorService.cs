using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Data.Services.Interfaces;

namespace Mebeller.Data.Services;

public class VisitorService : IVisitorService
{
    private readonly IVisitorRepository _visitorRepository;
    public VisitorService(IVisitorRepository visitorRepository) => _visitorRepository = visitorRepository;
    public async Task<IEnumerable<Visitor>> GetVisitorsAsync() => await _visitorRepository.GetVisitorsAsync();
    public async Task<int> GetNumberOfVisitsAsync() => await _visitorRepository.GetNumberOfVisitsAsync();

    public async Task<bool> AddOrUpdateVisitorAsync(string visitorIpAddress)
    {
        if (string.IsNullOrEmpty(visitorIpAddress))
        {
            throw new ArgumentNullException(nameof(visitorIpAddress));
        }

        var visitor = await _visitorRepository.GetVisitorByIpAddressAsync(visitorIpAddress);
        if (visitor == null)
        {
            visitor = new Visitor
            {
                VisitorIpAddress = visitorIpAddress, CountOfVisit = 1, LastVisitTime = DateTime.Now
            };
            await _visitorRepository.AddVisitorAsync(visitor);
        }
        else
        {
            visitor.CountOfVisit++;
            visitor.LastVisitTime = DateTime.Now;
            _visitorRepository.UpdateVisitor(visitor);
        }

        try
        {
            await _visitorRepository.SaveAsync();
            return true;
        }
        catch (Exception ex)
        {
            // Log the error message and return false to indicate failure
            Console.WriteLine($"An error occurred while saving the visitor: {ex.Message}");
            return false;
        }
    }
}