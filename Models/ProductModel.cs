using System.ComponentModel.DataAnnotations;

namespace CoffieShop.Models
{
    public class ProductModel
    {
     
        public int ProductID { get; set; }

        [Required(ErrorMessage = "Product name is required")]
        [StringLength(100, ErrorMessage = "Product name cannot be longer than 100 characters")]
        public string? ProductName { get; set; }

        [Required(ErrorMessage = "Product price is required")]
        [Range(0.01, 9999999999.99, ErrorMessage = "Product price must be a positive value")]
        public decimal ProductPrice { get; set; }

        [Required(ErrorMessage = "Product code is required")]
        [StringLength(100, ErrorMessage = "Product code cannot be longer than 100 characters")]
        public string? ProductCode { get; set; }

        [Required(ErrorMessage = "Description is required")]
        [StringLength(100, ErrorMessage = "Description cannot be longer than 100 characters")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public int UserID { get; set; }
    }
}
