using Mebeller.Data.Context;

namespace Mebeller.Areas.Admin.ViewModels.User;

public class UserDetailsViewModel
{
    public ApplicationUser User { get; set; }
    
    public string UserRoleName { get; set; }
}