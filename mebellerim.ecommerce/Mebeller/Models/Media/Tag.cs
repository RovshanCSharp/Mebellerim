using System;
using System.ComponentModel.DataAnnotations;

namespace Mebeller.Models.Media
{
    public class Tag
    {
        [Key]
        public Guid Id { get; set; }
        public string Name { get; set; }
        public Guid BlogPostId { get; set; }
    }
}
