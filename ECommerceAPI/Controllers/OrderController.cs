using ECommerceAPI.Data;
using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly OrderRepository _orderRepository;
        public OrderController(OrderRepository orderRepository)
        {
            _orderRepository = orderRepository;
        }
        // GET: api/order
        [HttpGet]
        public async Task<ActionResult<APIResponse<List<Order>>>> GetAllOrders(string Status = "Pending")
        {
            try
            {
                var orders = await _orderRepository.GetAllOrdersAsync(Status);
                return Ok(new APIResponse<List<Order>>(orders, "Retrieved all orders successfully."));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new APIResponse<List<Order>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message));
            }
        }
        // POST: api/order
        [HttpPost]
        public async Task<APIResponse<CreateOrderResponseDTO>> CreateOrder([FromBody] OrderDTO orderDto)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<CreateOrderResponseDTO>(HttpStatusCode.BadRequest, "Invalid data", ModelState);
            }
            try
            {
                var response = await _orderRepository.CreateOrderAsync(orderDto);
                return new APIResponse<CreateOrderResponseDTO>(response, response.Message);
            }
            catch (Exception ex)
            {
                return new APIResponse<CreateOrderResponseDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }
        // GET: api/order/5
        [HttpGet("{id}")]
        public async Task<APIResponse<Order>> GetOrderById(int id)
        {
            try
            {
                var order = await _orderRepository.GetOrderDetailsAsync(id);
                if (order == null)
                {
                    return new APIResponse<Order>(HttpStatusCode.NotFound, "Order not found.");
                }
                return new APIResponse<Order>(order, "Order retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<Order>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }
        // PUT: api/order/5/status
        [HttpPut("{id}/status")]
        public async Task<APIResponse<OrderStatusResponseDTO>> UpdateOrderStatus(int id, [FromBody] OrderStatusDTO status)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<OrderStatusResponseDTO>(HttpStatusCode.BadRequest, "Invalid data", ModelState);
            }
            if (id != status.OrderId)
            {
                return new APIResponse<OrderStatusResponseDTO>(HttpStatusCode.BadRequest, "Mismatched Order ID");
            }
            try
            {
                var response = await _orderRepository.UpdateOrderStatusAsync(id, status.Status);
                return new APIResponse<OrderStatusResponseDTO>(response, response.Message);
            }
            catch (Exception ex)
            {
                return new APIResponse<OrderStatusResponseDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }
        // PUT: api/order/5/confirm
        [HttpPut("{id}/confirm")]
        public async Task<APIResponse<ConfirmOrderResponseDTO>> ConfirmOrder(int id)
        {
            try
            {
                var response = await _orderRepository.ConfirmOrderAsync(id);
                return new APIResponse<ConfirmOrderResponseDTO>(response, response.Message);
            }
            catch (Exception ex)
            {
                return new APIResponse<ConfirmOrderResponseDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }
    }
}
