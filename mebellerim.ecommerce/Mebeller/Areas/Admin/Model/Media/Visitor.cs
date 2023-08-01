using System;
using System.ComponentModel.DataAnnotations;

namespace Mebeller.Areas.Admin.Model.Media
{
    public class Visitor
    {
        [Key]
        public int VisitorId { get; set; }

        [MaxLength(50, ErrorMessage = "The IP address can't be longer than 50 characters.")]
        [Required(ErrorMessage = "Please enter the visitor's IP address.")]
        public string VisitorIpAddress { get; init; }

        [Required(ErrorMessage = "Please enter the count of visits.")]
        [Range(1, int.MaxValue, ErrorMessage = "The count of visits must be a positive number.")]
        public int CountOfVisit { get; set; }

        [Required(ErrorMessage = "Please enter the last visit time.")]
        public DateTime LastVisitTime { get; set; } 
    }
}