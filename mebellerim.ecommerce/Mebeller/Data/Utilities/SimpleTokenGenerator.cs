using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Mebeller.Data.Context;
using Microsoft.AspNetCore.Identity;

namespace Mebeller.Data.Utilities;

public abstract class SimpleTokenGenerator :
    IUserTwoFactorTokenProvider<ApplicationUser>
{
    protected virtual int CodeLength => 6;

    public virtual Task<bool> CanGenerateTwoFactorTokenAsync(
        UserManager<ApplicationUser> manager, ApplicationUser user) =>
        Task.FromResult(manager.SupportsUserSecurityStamp);

    public virtual Task<string> GenerateAsync(string purpose,
        UserManager<ApplicationUser> manager, ApplicationUser user) =>
        Task.FromResult(GenerateCode(purpose, user));

    public virtual Task<bool> ValidateAsync(string purpose, string token,
        UserManager<ApplicationUser> manager, ApplicationUser user) =>
        Task.FromResult(GenerateCode(purpose, user).Equals(token));

    private string GenerateCode(string purpose, ApplicationUser user)
    {
        var hashAlgorithm =
            new HMACSHA1(Encoding.UTF8.GetBytes(user.SecurityStamp));
        var hashCode = hashAlgorithm.ComputeHash(
            Encoding.UTF8.GetBytes(GetData(purpose, user)));
        return BitConverter.ToString(hashCode[^CodeLength..]).Replace("-", "");
    }

    private string GetData(string purpose, ApplicationUser user) => $"{purpose}{user.SecurityStamp}";
}

public class EmailConfirmationTokenGenerator : SimpleTokenGenerator
{
    protected override int CodeLength => 12;

    public static EmailConfirmationTokenGenerator CreateInstance() => new();

    public override async Task<bool> CanGenerateTwoFactorTokenAsync(
        UserManager<ApplicationUser> manager, ApplicationUser user) =>
        await base.CanGenerateTwoFactorTokenAsync(manager, user)
        && !string.IsNullOrEmpty(user.Email)
        && !user.EmailConfirmed;
}

public class PhoneConfirmationTokenGenerator : SimpleTokenGenerator
{
    protected override int CodeLength => 3;

    public static PhoneConfirmationTokenGenerator CreateInstance() => new();

    public override async Task<bool> CanGenerateTwoFactorTokenAsync(
        UserManager<ApplicationUser> manager, ApplicationUser user) =>
        await base.CanGenerateTwoFactorTokenAsync(manager, user)
        && !string.IsNullOrEmpty(user.PhoneNumber)
        && !user.PhoneNumberConfirmed;
}