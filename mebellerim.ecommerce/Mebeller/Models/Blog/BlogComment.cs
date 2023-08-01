using System;
using System.ComponentModel.DataAnnotations;

namespace Mebeller.Models.Blog
{
    public class BlogComment
    {
        [Key]
        public Guid Id { get; init; }
        public string Description { get; init; }
        public DateTime DateAdded { get; init; }
        public string Username { get; init; }
    }
}