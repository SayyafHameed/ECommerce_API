using System.ComponentModel.DataAnnotations;

namespace ECommerceAPI.DTO
{
    public class CustomerDTO
    {
        public int CustomerId { get; set; } // Used only for updates, not for inserts
        [Required]
        public string Name { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        public string Address { get; set; }
    }
}
