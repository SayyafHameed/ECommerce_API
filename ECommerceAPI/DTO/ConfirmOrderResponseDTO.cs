namespace ECommerceAPI.DTO
{
    public class ConfirmOrderResponseDTO
    {
        public int OrderId { get; set; }
        public bool IsConfirmed { get; set; }
        public string Message { get; set; }
    }
}
