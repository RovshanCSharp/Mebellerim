using Mebeller.Data.Repositories;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Data.Services;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Data.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Mebeller.Config;

public static class DependencyContainer
{
    public static void RegisterServices(IServiceCollection services)
    {
        // Repositories
        services.AddScoped<IFileRepository, FileRepository>();
        services.AddScoped<IMediaRepository, MediaRepository>();
        services.AddScoped<IPageRepository, PageRepository>();
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<IVisitorRepository, VisitorRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IBlogPostRepository, BlogPostRepository>();
        services.AddScoped<ITagRepository, TagRepository>();
        services.AddScoped<IBlogPostCommentRepository, BlogPostCommentRepository>();
 
        // Services
        services.AddScoped<IAccountService, AccountService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IPageService, PageService>();
        services.AddScoped<IProductService, ProductService>();
        services.AddScoped<IVisitorService, VisitorService>();
        services.AddScoped<IFileService, FileService>();

        // Utilities
        services.AddScoped<TokenUrlEncoderService>();

        // Identity
        services.AddSingleton<ISmsSender, ConsoleSmsSender>();
        services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
        services.AddScoped<IEmailSender, ConsoleEmailSender>();
        services.AddScoped<IEmailService, EmailService>();
        services.AddScoped<IdentityEmailService>();
 
        // Add HttpContextAccessor
        services.AddHttpContextAccessor();
        services.AddHttpClient();
    }
}