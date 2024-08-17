using System.ComponentModel.DataAnnotations;

namespace CoffieShop.Models
{
    public class OrderDetailModel
    {

        public int? OrderDetailID { get; set; }

        [Required(ErrorMessage = "Order ID is required")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Product ID is required")]
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Amount must be a positive value")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Total amount is required")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Total amount must be a positive value")]
        public decimal TotalAmount { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserID { get; set; }
    }
}
