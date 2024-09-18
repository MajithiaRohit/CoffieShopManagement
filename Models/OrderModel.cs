using System.ComponentModel.DataAnnotations;

namespace CoffieShop.Models
{
    public class OrderModel
    {
        [Key]
        public int? OrderID { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        public DateTime OrderDate { get; set; }

      
        public int CustomerID { get; set; }

       
        public int PaymentModeID { get; set; }

        [Required(ErrorMessage = "Total amount is required")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Total amount must be a positive value")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "Shipping address is required")]
        [StringLength(100, ErrorMessage = "Shipping address cannot be longer than 100 characters")]
        public string? ShippingAddress { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserID { get; set; }
    }
}
