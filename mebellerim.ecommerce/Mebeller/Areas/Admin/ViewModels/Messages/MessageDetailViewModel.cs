using System;
using System.ComponentModel.DataAnnotations;
using Mebeller.Areas.Admin.Model.Media;

namespace Mebeller.Areas.Admin.ViewModels.Messages
{
    public class MessageDetailViewModel
    {
        public Message Message { get; set; }

        [Required(ErrorMessage = "Please provide a description for your reply.")]
        [StringLength(500, MinimumLength = 5, ErrorMessage = "Your reply must be between 5 and 500 characters long.")]
        public string MessageReplyDescription { get; set; }

        [Display(Name = "Submit Time")]
        public DateTime MessageSubmitTime { get; set; }
    }
}