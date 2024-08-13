using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTO
{
    public class UpdaePaymentDTO
    {
        [Required]
        public int PaymentId { get; set; }
        [Required]
        public string? Status { get; set; }
    }
}
