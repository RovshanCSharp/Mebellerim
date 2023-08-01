using System;
using System.ComponentModel.DataAnnotations;

namespace Mebeller.Areas.Admin.Model.Media
{
    public class Message
    {
        [Key]
        public int MessageId { get; set; }

        public DateTime SubmitTime { get; set; }

        [MaxLength(250, ErrorMessage = "Your name cannot exceed 250 characters.")]
        [Required(ErrorMessage = "Please enter your name.")]
        public string MessageSenderName { get; set; }

        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        [MaxLength(250, ErrorMessage = "Your email address cannot exceed 250 characters.")]
        [Required(ErrorMessage = "Please enter your email address.")]
        public string MessageSenderEmail { get; set; }

        [MaxLength(250, ErrorMessage = "The subject cannot exceed 250 characters.")]
        [Required(ErrorMessage = "Please enter the subject of your message.")]
        public string MessageSubject { get; set; }

        [Required(ErrorMessage = "Please describe your message.")]
        [MaxLength(2000, ErrorMessage = "Your message cannot exceed 2000 characters.")]
        public string MessageDescription { get; set; }

        public bool IsRead { get; set; }
        public bool IsReplied { get; set; }

        // Navigation Properties
        public MessageReply MessageReply { get; set; }
     }
}