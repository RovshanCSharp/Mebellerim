using System;

namespace Mebeller.Areas.Admin.ViewModels.User
{
    public class UserViewModel
    {
        public string UserId { get; init; }
        public string UserEmail { get; init; }
        public string UserRoleName { get; init; }

        public DateTime RegisterTime { get; init; }
        public string UserName { get; init; }
    }
}
