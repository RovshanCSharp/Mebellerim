using System.ComponentModel.DataAnnotations;

namespace Mebeller.Models.Product
{
    public class OrderInvoiceDetails
    {
        [Key]
        public int OrderInvoiceId { get; set; }
       
        [Required]
        public string InvoiceDetailsProductName { get; set; }   
        [Required]
        public decimal InvoiceDetailsTotalPrice { get; set; }
        [Required]
        public int InvoiceDetailsQuantity { get; set; }

        //Navigations Properties

        [Required]
        public Order Order { get; set; }
    }
}