using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Models.Media;

namespace Mebeller.Data.Repositories.Interfaces
{
    public interface ITagRepository
    {
        Task<IEnumerable<Tag>> GetAllAsync();
    }
}
