using System.ComponentModel.DataAnnotations;

namespace Mebeller.Areas.Admin.Model.Media;

public class UserDetails
{
    [Key]
    public int UserDetailsId { get; set; }

    [MaxLength(250, ErrorMessage = "The first name must be up to 250 characters")]
    public string FirstName { get; set; }

    [MaxLength(250, ErrorMessage = "The last name must be up to 250 characters")]
    public string LastName { get; set; }

    [MaxLength(250, ErrorMessage = "The province must be up to 250 characters")]
    public string UserProvince { get; set; }

    [MaxLength(250, ErrorMessage = "The city must be up to 250 characters")]
    public string UserCity { get; set; }

    [MaxLength(500, ErrorMessage = "The address must be up to 500 characters")]
    public string UserAddress { get; set; }

    [DataType(DataType.PostalCode)]
    public string UserZipCode { get; set; }
}