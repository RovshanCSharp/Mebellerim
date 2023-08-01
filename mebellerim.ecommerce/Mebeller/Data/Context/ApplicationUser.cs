using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Mebeller.Areas.Admin.Model.Media;
using Mebeller.Models.Media;
using Mebeller.Models.Product;
using Microsoft.AspNetCore.Identity;

namespace Mebeller.Data.Context;

public class ApplicationUser : IdentityUser
{
    
    public DateTime RegisterTime { get; set; } = DateTime.Now;

    // Navigation Properties
    public UserDetails UserDetails { get; set; }
    public ICollection<Product> FavoriteProducts { get; set; } = new List<Product>();
    public ICollection<Order> UserOrders { get; set; }
    public byte[] Picture { get; set; }
    public ICollection<Comment> UserComments { get; set; }
    [DataType(DataType.PhoneNumber)]
    public string MobileNumber { get; set; }

    public string FirstName { get; set; }
    public string LastName { get; set; }
}