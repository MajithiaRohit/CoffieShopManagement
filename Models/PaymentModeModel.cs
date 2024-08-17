using System.ComponentModel.DataAnnotations;

namespace CoffieShop.Models
{
    public class PaymentModeModel
    {
        public int PaymentModeID { get; set; }

        [Required]
        public string? PaymentModeName { get; set; }
    }
}
