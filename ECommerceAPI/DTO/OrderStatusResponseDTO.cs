namespace ECommerceAPI.DTO
{
    public class OrderStatusResponseDTO
    {
        public int OrderId { get; set; }
        public string Status { get; set; }
        public bool IsUpdated { get; set; }
        public string Message { get; set; }
    }
}
