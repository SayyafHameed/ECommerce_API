using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTO
{
    public class ProductDTO
    {
        public int ProductId { get; set; }  // Used only for updates, not for inserts
        [Required]
        public string Name { get; set; }
        [Required]
        public decimal Price { get; set; }
        [Required]
        public int Quantity { get; set; }
        public string Description { get; set; }
    }
}
