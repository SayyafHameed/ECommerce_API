namespace ECommerceAPI.DTO
{
    public class UpdatePaymentResponseDTO
    {
        public int PaymentId { get; set; }
        public string CurrentStatus { get; set; }
        public string UpdatedStatus { get; set; }
        public string Message { get; set; }
        public bool IsUpdated { get; set; }
    }
}
