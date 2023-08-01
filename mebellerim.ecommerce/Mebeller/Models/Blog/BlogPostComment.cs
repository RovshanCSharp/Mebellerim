using System;
using System.ComponentModel.DataAnnotations;

namespace Mebeller.Models.Blog
{
    public class BlogPostComment
    {
        [Key]
        public Guid Id { get; set; }

        public string Description { get; init; }

        public Guid BlogPostId { get; init; }

        public Guid UserId { get; init; }

        public DateTime DateAdded { get; init; }
    }
}
