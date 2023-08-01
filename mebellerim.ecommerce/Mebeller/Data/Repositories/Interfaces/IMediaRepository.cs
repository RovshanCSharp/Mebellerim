using System.Collections.Generic;
using System.Threading.Tasks;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Models.Blog;
using Mebeller.Models.Media;

namespace Mebeller.Data.Repositories.Interfaces
{
    public interface IMediaRepository : IGeneralRepository
    {
        Task<Comment> GetCommentWithDetailsAsync(int commentId);
        Task<Comment> GetCommentAsync(int commentId);
        void UpdateCommentAsync(Comment comment);
        Task<IEnumerable<Comment>> GetCommentsAsync();
        Task<int> GetUnreadCommentsCountAsync();
        void DeleteComments(IEnumerable<Comment> parentsComments); 
        void DeleteComment(Comment comment);
        void DeleteCommentById(int commentId);
        Task AddMessageAsync(Message message);
        void UpdateMessage(Message message);
        void DeleteMessage(Message message);
        Task<IEnumerable<Message>> GetMessagesAsync();
        Task<Message> GetMessageAsync(int messageId);
        Task<int> GetMessagesCountAsync();
        Task<int> GetUnreadMessagesCountAsync();
        Task<IEnumerable<Banner>> GetBannersAsync();
        Task<Banner> GetBannerAsync(int bannerId);
        Task AddBannerAsync(Banner banner);
        void UpdateBanner(Banner banner);
        void DeleteBanner(Banner banner);
        Task<IEnumerable<Newsletter>> GetNewslettersAsync();
        Task<bool> IsEmailExistInNewslettersCustomersAsync(string customerEmail);
        Task AddNewsletterAsync(Newsletter newsletter);
        Task DeleteNewsletterByIdAsync(int newsletterId);
        Task<IEnumerable<BlogPost>> GetBlogsAsync();
    }
}
