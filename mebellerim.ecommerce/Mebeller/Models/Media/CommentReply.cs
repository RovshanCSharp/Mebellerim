using System.ComponentModel.DataAnnotations;

namespace Mebeller.Models.Media;

public class CommentReply
{
    [Key]
    public int CommentReplyId { get; set; }
    public int CommentId { get; set; }
    public string Description { get; set; }
    public string UserId { get; set; }
}
