using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Data.Context;
using Mebeller.Data.Repositories.Interfaces;

namespace Mebeller.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly AppDbContext _context;
    public UserRepository(AppDbContext context) => _context = context;
    public void DeleteUserDetails(UserDetails userDetail) => _context.Remove(userDetail);
    public async Task SaveAsync() => await _context.SaveChangesAsync();
}