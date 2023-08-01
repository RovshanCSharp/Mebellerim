using Mebeller.Areas.Admin.Model.Media;

namespace Mebeller.Data.Repositories.Interfaces
{
    public interface IUserRepository : IGeneralRepository
    {
        void DeleteUserDetails(UserDetails userDetail);
    }
}
