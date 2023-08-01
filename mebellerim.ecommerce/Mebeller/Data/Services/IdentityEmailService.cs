using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Mebeller.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Routing;

namespace Mebeller.Data.Services;

public class IdentityEmailService
{
    public IdentityEmailService(IEmailSender sender,
        UserManager<ApplicationUser> userMgr,
        IHttpContextAccessor contextAccessor,
        LinkGenerator generator,
        TokenUrlEncoderService encoder)
    {
        EmailSender = sender;
        UserManager = userMgr;
        ContextAccessor = contextAccessor;
        LinkGenerator = generator;
        TokenEncoder = encoder;
    }

    private IEmailSender EmailSender { get; }
    private UserManager<ApplicationUser> UserManager { get; }
    private IHttpContextAccessor ContextAccessor { get; }
    private LinkGenerator LinkGenerator { get; }
    private TokenUrlEncoderService TokenEncoder { get; }

    private string GetUrl(string emailAddress, string token, string page)
    {
        var safeToken = TokenUrlEncoderService.EncodeToken(token);
        Debug.Assert(ContextAccessor.HttpContext != null, "ContextAccessor.HttpContext != null");
        return LinkGenerator.GetUriByPage(ContextAccessor.HttpContext, page,
            null, new { email = emailAddress, token = safeToken }) ?? throw new InvalidOperationException();
    }

    public async Task SendPasswordRecoveryEmail(ApplicationUser user,
        string confirmationPage)
    {
        var token = await UserManager.GeneratePasswordResetTokenAsync(user);
        var url = GetUrl(user.Email, token, confirmationPage);
        await EmailSender.SendEmailAsync(user.Email, "Set Your Password",
            $"Please set your password by <a href={url}>clicking here</a>.");
    }

    public async Task SendAccountConfirmEmail(ApplicationUser user,
        string confirmationPage)
    {
        var token =
            await UserManager.GenerateEmailConfirmationTokenAsync(user);
        var url = GetUrl(user.Email, token, confirmationPage);
        await EmailSender.SendEmailAsync(user.Email,
            "Complete Your Account Setup",
            $"Please set up your account by <a href={url}>clicking here</a>.");
    }
}