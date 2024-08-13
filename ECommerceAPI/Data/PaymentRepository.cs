using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using Npgsql;

namespace ECommerceAPI.Data
{
    public class PaymentRepository
    {
        private readonly PostgreSqlConnectionFactory _connectionFactory;
        public PaymentRepository(PostgreSqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<PaymentResponseDTO> MakePaymentAsync(PaymentDTO paymentDto)
        {
            var orderValidationQuery = "SELECT TotalAmount FROM Orders WHERE OrderId = @OrderId AND Status = 'Pending'";
            var insertPaymentQuery = "INSERT INTO Payments (OrderId, Amount, Status, PaymentType, PaymentDate) OUTPUT INSERTED.PaymentId VALUES (@OrderId, @Amount, 'Pending', @PaymentType, @PaymentDate)";
            var updatePaymentStatusQuery = "UPDATE Payments SET Status = @Status WHERE PaymentId = @PaymentId";
            PaymentResponseDTO paymentResponseDTO = new PaymentResponseDTO();
            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Validate order amount and status
                        decimal orderAmount = 0m;
                        using (var validationCommand = new NpgsqlCommand(orderValidationQuery, connection, transaction))
                        {
                            validationCommand.Parameters.AddWithValue("@OrderId", paymentDto.OrderId);
                            var result = await validationCommand.ExecuteScalarAsync();
                            if (result == null)
                            {
                                paymentResponseDTO.Message = "Order either does not exist or is not in a pending state.";
                                return paymentResponseDTO;
                            }
                            orderAmount = (decimal)result;
                        }
                        if (orderAmount != paymentDto.Amount)
                        {
                            paymentResponseDTO.Message = "Payment amount does not match the order total.";
                            return paymentResponseDTO;
                        }
                        // Insert initial payment record with 'Pending' status
                        int paymentId;
                        using (var insertCommand = new NpgsqlCommand(insertPaymentQuery, connection, transaction))
                        {
                            insertCommand.Parameters.AddWithValue("@OrderId", paymentDto.OrderId);
                            insertCommand.Parameters.AddWithValue("@Amount", paymentDto.Amount);
                            insertCommand.Parameters.AddWithValue("@PaymentType", paymentDto.PaymentType);
                            insertCommand.Parameters.AddWithValue("@PaymentDate", DateTime.Now);
                            paymentId = (int)await insertCommand.ExecuteScalarAsync();
                        }
                        // Simulate interaction with a payment gateway
                        string paymentStatus = SimulatePaymentGatewayInteraction(paymentDto);
                        // Update the payment status after receiving the gateway response
                        using (var updateCommand = new NpgsqlCommand(updatePaymentStatusQuery, connection, transaction))
                        {
                            updateCommand.Parameters.AddWithValue("@Status", paymentStatus);
                            updateCommand.Parameters.AddWithValue("@PaymentId", paymentId);
                            await updateCommand.ExecuteNonQueryAsync();
                            paymentResponseDTO.IsCreated = true;
                            paymentResponseDTO.Status = paymentStatus;
                            paymentResponseDTO.PaymentId = paymentId;
                            paymentResponseDTO.Message = $"Payment Processed with Status {paymentStatus}";
                        }
                        transaction.Commit();
                        return paymentResponseDTO;
                    }
                    catch (Exception)
                    {
                        transaction.Rollback();
                        throw; // Re-throw to handle the exception further up the call stack
                    }
                }
            }
        }
        private string SimulatePaymentGatewayInteraction(PaymentDTO paymentDto)
        {
            // Simulate different responses based on the payment type or other logic
            switch (paymentDto.PaymentType)
            {
                case "COD":
                    return "Completed";  // COD is usually confirmed immediately if used
                case "CC":
                    return "Completed";  // Assuming credit card payments are processed immediately
                case "DC":
                    return "Failed";     // Simulating a failure for demonstration purposes
                default:
                    return "Completed";  // Default to completed for simplicity in this example
            }
        }
        public async Task<UpdatePaymentResponseDTO> UpdatePaymentStatusAsync(int paymentId, string newStatus)
        {
            // Queries to fetch related order and current payment details
            var paymentDetailsQuery = "SELECT p.OrderId, p.Amount, p.Status, o.Status AS OrderStatus FROM Payments p INNER JOIN Orders o ON p.OrderId = o.OrderId WHERE p.PaymentId = @PaymentId";
            var updatePaymentStatusQuery = "UPDATE Payments SET Status = @Status WHERE PaymentId = @PaymentId";
            UpdatePaymentResponseDTO updatePaymentResponseDTO = new UpdatePaymentResponseDTO()
            {
                PaymentId = paymentId
            };
            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                int orderId;
                decimal paymentAmount;
                string currentPaymentStatus, orderStatus;
                // Retrieve current payment and order status
                using (var command = new NpgsqlCommand(paymentDetailsQuery, connection))
                {
                    command.Parameters.AddWithValue("@PaymentId", paymentId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (!reader.Read())
                        {
                            throw new Exception("Payment record not found.");
                        }
                        orderId = reader.GetInt32(reader.GetOrdinal("OrderId"));
                        paymentAmount = reader.GetDecimal(reader.GetOrdinal("Amount"));
                        currentPaymentStatus = reader.GetString(reader.GetOrdinal("Status"));
                        orderStatus = reader.GetString(reader.GetOrdinal("OrderStatus"));
                        //Also set the CurrentStatus in updatePaymentResponseDTO
                        updatePaymentResponseDTO.CurrentStatus = currentPaymentStatus;
                    }
                }
                // Validate the new status change
                if (!IsValidStatusTransition(currentPaymentStatus, newStatus, orderStatus))
                {
                    updatePaymentResponseDTO.IsUpdated = false;
                    updatePaymentResponseDTO.Message = $"Invalid status transition from {currentPaymentStatus} to {newStatus} for order status {orderStatus}.";
                    return updatePaymentResponseDTO;
                }
                // Update the payment status
                using (var updateCommand = new NpgsqlCommand(updatePaymentStatusQuery, connection))
                {
                    updateCommand.Parameters.AddWithValue("@PaymentId", paymentId);
                    updateCommand.Parameters.AddWithValue("@Status", newStatus);
                    await updateCommand.ExecuteNonQueryAsync();
                    updatePaymentResponseDTO.IsUpdated = true;
                    updatePaymentResponseDTO.UpdatedStatus = newStatus;
                    updatePaymentResponseDTO.Message = $"Payment Status Updated from {currentPaymentStatus} to {newStatus}";
                    return updatePaymentResponseDTO;
                }
            }
        }
        public async Task<Payment?> GetPaymentDetailsAsync(int paymentId)
        {
            var query = "SELECT PaymentId, OrderId, Amount, Status, PaymentType, PaymentDate FROM Payments WHERE PaymentId = @PaymentId";
            Payment? payment = null;
            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@PaymentId", paymentId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            payment = new Payment
                            {
                                PaymentId = reader.GetInt32(reader.GetOrdinal("PaymentId")),
                                OrderId = reader.GetInt32(reader.GetOrdinal("OrderId")),
                                Amount = reader.GetDecimal(reader.GetOrdinal("Amount")),
                                Status = reader.GetString(reader.GetOrdinal("Status")),
                                PaymentType = reader.GetString(reader.GetOrdinal("PaymentType")),
                                PaymentDate = reader.GetDateTime(reader.GetOrdinal("PaymentDate"))
                            };
                        }
                    }
                }
            }
            return payment;
        }
        private bool IsValidStatusTransition(string currentStatus, string newStatus, string orderStatus)
        {
            // Completed payments cannot be modified unless it's a refund for a returned order.
            if (currentStatus == "Completed" && newStatus != "Refund")
            {
                return false;
            }
            // Only pending payments can be cancelled.
            if (currentStatus == "Pending" && newStatus == "Cancelled")
            {
                return true;
            }
            // Refunds should only be processed for returned orders.
            if (currentStatus == "Completed" && newStatus == "Refund" && orderStatus != "Returned")
            {
                return false;
            }
            // Payments should only be marked as failed if they are not completed or cancelled.
            if (newStatus == "Failed" && (currentStatus == "Completed" || currentStatus == "Cancelled"))
            {
                return false;
            }
            // Assuming 'Pending' payments become 'Completed' when the order is shipped or services are rendered.
            if (currentStatus == "Pending" && newStatus == "Completed" && (orderStatus == "Shipped" || orderStatus == "Confirmed"))
            {
                return true;
            }
            // Handle other generic cases or add more specific business rule checks
            return true;
        }
    }
}
