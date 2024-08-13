using ECommerceAPI.Data;
using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PaymentController : ControllerBase
    {
        private readonly PaymentRepository _paymentRepository;
        public PaymentController(PaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }
        // POST: api/payment/makepayment
        [HttpPost("MakePayment")]
        public async Task<APIResponse<PaymentResponseDTO>> MakePayment([FromBody] PaymentDTO paymentDto)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<PaymentResponseDTO>(HttpStatusCode.BadRequest, "Invalid Data", ModelState);
            }
            try
            {
                var response = await _paymentRepository.MakePaymentAsync(paymentDto);
                return new APIResponse<PaymentResponseDTO>(response, response.Message);
            }
            catch (Exception ex)
            {
                return new APIResponse<PaymentResponseDTO>(HttpStatusCode.InternalServerError, "Internal Server Error: " + ex.Message);
            }
        }
        // GET: api/payment/paymentdetails/5
        [HttpGet("PaymentDetails/{id}")]
        public async Task<APIResponse<Payment>> GetPaymentDetails(int id)
        {
            try
            {
                var payment = await _paymentRepository.GetPaymentDetailsAsync(id);
                if (payment == null)
                {
                    return new APIResponse<Payment>(HttpStatusCode.NotFound, $"Payment with ID {id} not found.");
                }
                return new APIResponse<Payment>(payment, "Payment retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<Payment>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }
        // PUT: api/payment/updatepaymentstatus/5
        [HttpPut("UpdatePaymentStatus/{id}")]
        public async Task<APIResponse<UpdatePaymentResponseDTO>> UpdatePaymentStatus(int id, [FromBody] PaymentStatusDTO paymentStatusDTO)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<UpdatePaymentResponseDTO>(HttpStatusCode.BadRequest, "Invalid data", ModelState);
            }
                                       
            if (id != paymentStatusDTO.PaymentId)
            {
                return new APIResponse<UpdatePaymentResponseDTO>(HttpStatusCode.BadRequest, "Mismatched Payment ID");
            }
            try
            {
                var response = await _paymentRepository.UpdatePaymentStatusAsync(id, paymentStatusDTO.Status);
                return new APIResponse<UpdatePaymentResponseDTO>(response, response.Message);
            }
            catch (Exception ex)
            {
                return new APIResponse<UpdatePaymentResponseDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }
    }
}
