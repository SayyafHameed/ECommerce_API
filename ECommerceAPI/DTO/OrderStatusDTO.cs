using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTO
{
    public class OrderStatusDTO
    {
        [Required]
        public int OrderId { get; set; }
        [Required]
        public string Status { get; set; }
    }
}
