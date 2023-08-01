using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Data.Context;
using Mebeller.Data.Repositories.Interfaces;
using Mebeller.Models.Blog;
using Mebeller.Models.Media;
using Microsoft.EntityFrameworkCore;

namespace Mebeller.Data.Repositories;

public class MediaRepository : IMediaRepository
{
    private readonly AppDbContext _context;
    public MediaRepository(AppDbContext context) => _context = context;

    //Comment start
        
    public async Task<Comment> GetCommentWithDetailsAsync(int commentId) => await _context.Comments
        .Include(p => p.ParentComment).Include(p => p.User).Include(p => p.Product).Include(p => p.Replies)
        .ThenInclude(p => p.User).SingleOrDefaultAsync(p => p.CommentId == commentId);

    public async Task<Comment> GetCommentAsync(int commentId) => await _context.Comments.FindAsync(commentId);
    public void UpdateCommentAsync(Comment comment) => _context.Update(comment);

    public async Task<IEnumerable<Comment>> GetCommentsAsync() => await _context.Comments.Include(p => p.Product)
        .Include(p => p.User).Include(p => p.ParentComment).ThenInclude(p => p.User).ToListAsync();

    public async Task<int> GetUnreadCommentsCountAsync() => await _context.Comments.CountAsync(p => !p.IsRead);
    public void DeleteComments(IEnumerable<Comment> comments) => _context.RemoveRange(comments);
    public void DeleteComment(Comment comment) => _context.Remove(comment);
    public async void DeleteCommentById(int commentId) => DeleteComment(await GetCommentAsync(commentId));

    //Comments end
        
    //Messages start
        
    public async Task AddMessageAsync(Message message) => await _context.AddAsync(message);
    public void UpdateMessage(Message message) => _context.Update(message);
    public void DeleteMessage(Message message) => _context.Remove(message);

    public async Task<Message> GetMessageAsync(int messageId) => await _context.Messages
        .Include(p => p.MessageReply).SingleOrDefaultAsync(p => p.MessageId == messageId);

    public async Task<int> GetMessagesCountAsync() => await _context.Messages.CountAsync();
    public async Task<IEnumerable<Message>> GetMessagesAsync() => await _context.Messages.ToListAsync();
    public async Task<int> GetUnreadMessagesCountAsync() => await _context.Messages.CountAsync(p => !p.IsRead);

    //Messages end
        
    //Banners start 
    public async Task<IEnumerable<Banner>> GetBannersAsync() =>
        await _context.Banners.Include(p => p.Image).ToListAsync();

    public async Task<Banner> GetBannerAsync(int bannerId) => await _context.Banners.Include(p => p.Image)
        .SingleOrDefaultAsync(p => p.Id == bannerId);

    public async Task AddBannerAsync(Banner banner) => await _context.AddAsync(banner);
    public void UpdateBanner(Banner banner) => _context.Update(banner);
    public void DeleteBanner(Banner banner) => _context.Remove(banner);

    //Banners end
        
    //Newsletters start
    public async Task<IEnumerable<Newsletter>> GetNewslettersAsync() => await _context.Newsletters.ToListAsync();

    public async Task<bool> IsEmailExistInNewslettersCustomersAsync(string customerEmail) =>
        await _context.Newsletters.AnyAsync(p => p.CustomerEmail == customerEmail);

    public async Task AddNewsletterAsync(Newsletter newsletter) => await _context.AddAsync(newsletter);

    public async Task DeleteNewsletterByIdAsync(int newsletterId)
    {
        var newsletter = await _context.Newsletters.FindAsync(newsletterId);
        switch (newsletter)
        {
            case null:
                throw new InvalidOperationException($"Newsletter with ID {newsletterId} not found.");
            default:
                _context.Remove(newsletter);
                await _context.SaveChangesAsync();
                break;
        }
    }

    public async Task<IEnumerable<BlogPost>> GetBlogsAsync() => await _context.BlogPosts.Include(nameof(BlogPost.Tags)).ToListAsync();

    //Newsletters end
        
    public async Task SaveAsync() => await _context.SaveChangesAsync();
}