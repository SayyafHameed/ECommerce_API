namespace ECommerceAPI.DTO
{
    public class PaymentResponseDTO
    {
        public int PaymentId { get; set; }
        public string Status { get; set; }
        public string Message { get; set; }
        public bool IsCreated { get; set; }
    }
}
