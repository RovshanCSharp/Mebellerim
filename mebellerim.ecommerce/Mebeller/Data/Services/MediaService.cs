using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Areas.Admin.ViewModels.Banners;
using Mebeller.Areas.Admin.ViewModels.Messages;
using Mebeller.Data.Context;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Data.Services.Interfaces;
using Mebeller.Data.Utilities;
using Mebeller.Models.Blog;
using Mebeller.Models.Media;
using Mebeller.Models.ViewModels.Product;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace Mebeller.Data.Services;

public class MediaService : IMediaService
{
    private readonly IMediaRepository _mediaRepository;
    private readonly IAccountService _accountService;
    private readonly IEmailService _emailService;
    private readonly IHttpContextAccessor _accessor;
    private readonly IFileService _fileService;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly AppDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public MediaService(IMediaRepository mediaRepository, IAccountService accountService,IEmailService emailService, IHttpContextAccessor accessor, IFileService fileService, IHttpContextAccessor httpContextAccessor, AppDbContext context, UserManager<ApplicationUser> userManager)
    {
        _mediaRepository = mediaRepository;
        _accountService = accountService;
        _emailService = emailService;
        _accessor = accessor;
        _fileService = fileService;
        _httpContextAccessor = httpContextAccessor;
        _context = context;
        _userManager = userManager;
    }

    public async Task<IEnumerable<Comment>> GetCommentsAsync() =>
        await _mediaRepository
            .GetCommentsAsync();

    public async Task<int> GetUnreadCommentsCountAsync() =>
        await _mediaRepository
            .GetUnreadCommentsCountAsync();

    public async Task<bool> AddReplyToCommentByAdminAsync(int commentId, string newCommentReplyDescription)
    {
        try
        {
            var comment =
                await GetCommentAsync(commentId);

            if (comment == null)
                return false;

            var loggedUser =
                await _accountService
                    .GetLoggedUserAsync();

            var commentReply = new Comment()
            {
                SubmitTime = DateTime.Now,
                CommentDescription = newCommentReplyDescription,
                Product = comment.Product,
                User = loggedUser,
                IsRead = true,
                IsConfirmed = true,
                IsAdminReplied = true
            };

            comment.Replies.Add(commentReply);

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<bool> AddCommentToProductAsync(ProductsViewModel productViewModel)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(productViewModel.CommentDescription))
                return false;

            var user =
                await _accountService
                    .GetLoggedUserAsync();

            var comment = new Comment()
            {
                SubmitTime = DateTime.Now,
                CommentDescription = productViewModel.CommentDescription,
                Product = productViewModel.Product,
                User = user
            };

            productViewModel.Product.Comments.Add(comment);

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<bool> AddReplyToProductCommentAsync(ProductsViewModel productViewModel)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(productViewModel.CommentDescription))
                return false;

            var parentComment =
                await GetCommentAsync(productViewModel.ParentCommentId);

            if (parentComment == null)
                return false;

            var loggedUser =
                await _accountService
                    .GetLoggedUserAsync();

            var commentReply = new Comment()
            {
                SubmitTime = DateTime.Now,
                CommentDescription = productViewModel.CommentDescription,
                Product = productViewModel.Product,
                User = loggedUser
            };

            parentComment.Replies.Add(commentReply);

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<Comment> GetCommentAsync(int commentId) =>
        await _mediaRepository
            .GetCommentWithDetailsAsync(commentId);

    public async Task<bool> UpdateCommentAsync(Comment comment)
    {
        try
        {
            _mediaRepository
                .UpdateCommentAsync(comment);

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<bool> SetCommentAsReadAsync(Comment comment)
    {
        try
        {
            if (comment.IsRead)
                return true;

            comment.IsRead = true;

            _mediaRepository
                .UpdateCommentAsync(comment);

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public bool DeleteCommentsByParents(IEnumerable<Comment> parentsComments)
    {
        try
        {
            foreach (var comment in parentsComments) 
            {
                if (comment.Replies.NotNullOrEmpty())
                {
                    _mediaRepository
                        .DeleteComments(comment.Replies);
                }
            }

            _mediaRepository
                .DeleteComments(parentsComments);

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<bool> DeleteCommentAsync(Comment comment)
    {
        try
        {
            if (comment.Replies.NotNullOrEmpty())
            {
                _mediaRepository
                    .DeleteComments(comment.Replies);
            }

            _mediaRepository
                .DeleteComment(comment);

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    
    public async Task<bool> AddReplyToCommentAsync(int commentId, string newCommentReplyDescription)
    {
        var comment = await _context.Comments.FindAsync(commentId);

        if (comment == null)
        {
            return false;
        }

        var user = await _userManager.GetUserAsync(_httpContextAccessor.HttpContext?.User);

        if (user == null)
        {
            return false;
        }

        var commentReply = new CommentReply
        {
            CommentId = commentId,
            Description = newCommentReplyDescription,
            UserId = user.Id
        };

        _context.CommentReplies.Add(commentReply);
        await _context.SaveChangesAsync();

        return true;
    }

    public async Task<bool> DeleteCommentByIdAsync(int commentId)
    {
        try
        {
            var comment =
                await GetCommentAsync(commentId);

            await DeleteCommentAsync(comment);

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<bool> ConfirmCommentAsync(int commentId)
    {
        try
        {
            var comment =
                await GetCommentAsync(commentId);

            if (comment == null)
                return false;

            comment.IsRead = true;
            comment.IsConfirmed = true;

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<bool> RejectCommentAsync(int commentId)
    {
        try
        {
            var comment =
                await GetCommentAsync(commentId);

            if (comment.Replies.Any())
            {
                foreach (var commentReply in comment.Replies)
                {
                    commentReply.IsConfirmed = false;
                }
            }

            comment
                .IsConfirmed = false;

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
        
    public async Task<bool> AddMessageAsync(Message message)
    {
        try
        {
            message.SubmitTime = 
                DateTime.Now;

            await _mediaRepository
                .AddMessageAsync(message);

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<bool> DeleteMessageByIdAsync(int messageId)
    {
        try
        {
            var message = 
                await GetMessageAsync(messageId);

            if (message == null)
                return false;

            _mediaRepository
                .DeleteMessage(message);

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<int> GetMessagesCountAsync() =>
        await _mediaRepository
            .GetMessagesCountAsync();

    public async Task<Message> GetMessageAsync(int messageId) =>
        await _mediaRepository
            .GetMessageAsync(messageId);

    public async Task<IEnumerable<Message>> GetMessagesAsync() =>
        await _mediaRepository
            .GetMessagesAsync();

    public async Task<int> GetUnreadMessagesCountAsync() =>
        await _mediaRepository
            .GetUnreadMessagesCountAsync();

    public async Task<bool> ReplyToMessageAsync(string messageReplyDescription, int messageId)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(messageReplyDescription))
                return false;

            var message =
                await GetMessageAsync(messageId);

            if (message == null)
                return false;

            var emailMessageViewModel = new ReplyToMessageEmailTemplateViewModel()
            {
                MessageSubject = message.MessageSubject,
                MessageReplyDescription = messageReplyDescription
            };

            var emailMessage =
                await ViewToStringRenderer
                    .RenderViewToStringAsync(_accessor.HttpContext?.RequestServices,
                        "~/Views/Emails/ReplyToMessageTemplate.cshtml", emailMessageViewModel);
            await
                _emailService.SendEmailAsync
                    (message.MessageSenderEmail, "Answer your question", emailMessage, true);

            var messageReply = new MessageReply()
            {
                MessageReplySubmitTime = DateTime.Now,
                MessageReplyDescription = messageReplyDescription
            };

            message.MessageReply = messageReply;
            message.IsReplied = true;

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<bool> SetMessageAsReadAsync(Message message)
    {
        try
        {
            if (message.IsRead)
                return true;

            message.IsRead = true;

            _mediaRepository
                .UpdateMessage(message);

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<IEnumerable<Banner>> GetBannersAsync() =>
        await _mediaRepository
            .GetBannersAsync();

    public async Task<EditBannerViewModel> GetBannerForEditAsync(int bannerId)
    {
        var banner =
            await _mediaRepository
                .GetBannerAsync(bannerId);

        if (banner == null)
            return null;

        var bannerViewModel = new EditBannerViewModel()
        {
            BannerId = bannerId,
            BannerCurrentImagePath = banner.Image.ImagePath,
            BannerLink = banner.Link,
            BannerTitle = banner.Title,
            BannerDescription = banner.Description,
            IsPrimaryBanner = banner.IsPrimary
        };

        return bannerViewModel;
    }
    public async Task<bool> AddBannerAsync(AddBannerViewModel bannerViewModel)
    {
        try
        {
            var banner = new Banner()
            {
                Title = bannerViewModel.BannerTitle,
                Description = bannerViewModel.BannerDescription,
                Link = bannerViewModel.BannerLink,
                IsPrimary = bannerViewModel.IsPrimaryBanner
            };

            var addBannerImageResult =
                await _fileService
                    .AddBannerImageAsync(banner,bannerViewModel.BannerImage);

            if (!addBannerImageResult)
                return false;

            await _mediaRepository
                .AddBannerAsync(banner);

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<bool> UpdateBannerAsync(EditBannerViewModel bannerViewModel)
    {
        try
        {
            var currentBanner =
                await _mediaRepository
                    .GetBannerAsync(bannerViewModel.BannerId);

            if (currentBanner == null)
                return false;

            if (bannerViewModel.BannerImage != null)
            {
                var deleteBannerImageResult =
                    _fileService
                        .DeleteBannerImage(currentBanner.Image);

                if (!deleteBannerImageResult)
                    return false;

                var addBannerImageResult =
                    await _fileService
                        .AddBannerImageAsync(currentBanner, bannerViewModel.BannerImage);

                if (!addBannerImageResult)
                    return false;
            }

            currentBanner
                    .Title =
                bannerViewModel
                    .BannerTitle;

            currentBanner
                    .Link =
                bannerViewModel
                    .BannerLink;

            currentBanner
                    .Description =
                bannerViewModel
                    .BannerDescription;

            currentBanner
                    .IsPrimary =
                bannerViewModel
                    .IsPrimaryBanner;

            _mediaRepository
                .UpdateBanner(currentBanner);

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<bool> DeleteBannerByIdAsync(int bannerId)
    {
        try
        {
            var banner =
                await _mediaRepository
                    .GetBannerAsync(bannerId);

            if (banner == null)
                return false;

            var deleteBannerImageResult =
                _fileService
                    .DeleteBannerImage(banner.Image);

            if (!deleteBannerImageResult)
                return false;

            _mediaRepository
                .DeleteBanner(banner);

            await _mediaRepository
                .SaveAsync();

            return true;

        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<IEnumerable<Newsletter>> GetNewslettersAsync() =>
        await _mediaRepository
            .GetNewslettersAsync();

    public async Task<bool> IsEmailExistInNewslettersCustomersAsync(string customerEmail) =>
        await _mediaRepository
            .IsEmailExistInNewslettersCustomersAsync(customerEmail);

    public async Task<bool> AddNewsletterAsync(string customerEmail)
    {
        try
        {
            var newsletter = new Newsletter()
            {
                CustomerEmail = customerEmail
            };

            await _mediaRepository
                .AddNewsletterAsync(newsletter);

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }
    public async Task<bool> DeleteNewsletterByIdAsync(int newsletterId)
    {
        try
        {
            await _mediaRepository
                .DeleteNewsletterByIdAsync(newsletterId);

            await _mediaRepository
                .SaveAsync();

            return true;
        }
        catch (Exception error)
        {
            Console.WriteLine(error.Message);
            return false;
        }
    }

    public async Task<IEnumerable<BlogPost>> GetBlogsAsync() => await _mediaRepository.GetBlogsAsync();
}