using System.Linq;
using System.Threading.Tasks;
using Mebeller.Data.Context;
using Microsoft.AspNetCore.Identity;

namespace Mebeller.Data.Utilities;

public class EmailValidator : IUserValidator<ApplicationUser>
{
    private static readonly string[] AllowedDomains =
    {
        "example.com", "acme.com", "turn.az", "gmail.com", "outlook.com",
        "yahoo.com", "yandex.com"
    };

    private static readonly IdentityError Err = new() { Description = "Email address domain not allowed" };

    public EmailValidator(ILookupNormalizer normalizer)
    {
        Normalizer = normalizer;
    }

    private ILookupNormalizer Normalizer { get; }

    public Task<IdentityResult> ValidateAsync(UserManager<ApplicationUser> manager,
        ApplicationUser user)
    {
        var normalizedEmail = Normalizer.NormalizeEmail(user.Email);
        return Task.FromResult(AllowedDomains.Any(domain =>
            normalizedEmail.EndsWith($"@{domain}"))
            ? IdentityResult.Success
            : IdentityResult.Failed(Err));
    }
}