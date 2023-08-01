using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mebeller.Data;
using Mebeller.Data.Context;

namespace Mebeller.Models.Media
{
    public class Comment
    {
        public Comment() => Replies = new List<Comment>();

        [Key]
        public int CommentId { get; set; }

        [Required(ErrorMessage = "Please enter your comment.")]
        public string CommentDescription { get; set; }

        public DateTime SubmitTime { get; set; }
        public bool IsRead { get; set; }
        public bool IsConfirmed { get; set; }
        public bool IsAdminReplied { get; set; }

        // Navigation Properties
        [Required(ErrorMessage = "Please select a product.")]
        public Product.Product Product { get; set; }

        [Required(ErrorMessage = "Please select a user.")]
        public ApplicationUser User { get; set; }

        public Comment ParentComment { get; set; }
        public ICollection<Comment> Replies { get; set; }
    }
}