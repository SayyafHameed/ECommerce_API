using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTO
{
    public class PaymentDTO
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public string PaymentType { get; set; }
    }
}
