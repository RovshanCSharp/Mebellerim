using System;
using System.ComponentModel.DataAnnotations;

namespace Mebeller.Areas.Admin.Model.Media;

public class MessageReply
{
    [Key]
    public int MessageReplyId { get; set; }

    [Required(ErrorMessage = "Please enter the message reply description.")]
    [MaxLength(2000, ErrorMessage = "The message reply description cannot exceed 2000 characters.")]
    public string MessageReplyDescription { get; init; }

    public DateTime MessageReplySubmitTime { get; init; }
}