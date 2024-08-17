using System.ComponentModel.DataAnnotations;

namespace CoffieShop.Models
{
    public class BillsModel
    {
        [Key]
        public int BillID { get; set; }

        [Required(ErrorMessage = "Bill number is required")]
        [StringLength(100, ErrorMessage = "Bill number cannot be longer than 100 characters")]
        public string? BillNumber { get; set; }

        [Required(ErrorMessage = "Bill date is required")]
        public DateTime BillDate { get; set; }

        [Required(ErrorMessage = "Order ID is required")]
        public int OrderID { get; set; }

        [Required(ErrorMessage = "Total amount is required")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Total amount must be a positive value")]
        public decimal TotalAmount { get; set; }


        [Range(0, 9999999999.99, ErrorMessage = "Discount must be a positive value")]
        public decimal? Discount { get; set; } = 0;

        [Required(ErrorMessage = "Net amount is required")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Net amount must be a positive value")]
        public decimal NetAmount { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserID { get; set; }
    }
}
