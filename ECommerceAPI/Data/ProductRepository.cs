using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using Npgsql;

namespace ECommerceAPI.Data
{
    public class ProductRepository
    {
        private readonly PostgreSqlConnectionFactory _connectionFactory;
        public ProductRepository(PostgreSqlConnectionFactory connectionFactory)
        {
            _connectionFactory = connectionFactory;
        }
        public async Task<List<Product>> GetAllProductsAsync()
        {
            var products = new List<Product>();
            var query = "SELECT ProductId, Name, Price, Quantity, Description, IsDeleted FROM Products WHERE IsDeleted = 0";
            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            products.Add(new Product
                            {
                                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
                            });
                        }
                    }
                }
            }
            return products;
        }
        public async Task<Product?> GetProductByIdAsync(int productId)
        {
            var query = "SELECT ProductId, Name, Price, Quantity, Description, IsDeleted FROM Products WHERE ProductId = @ProductId AND IsDeleted = 0";
            Product? product = null;
            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            product = new Product
                            {
                                ProductId = reader.GetInt32(reader.GetOrdinal("ProductId")),
                                Name = reader.GetString(reader.GetOrdinal("Name")),
                                Price = reader.GetDecimal(reader.GetOrdinal("Price")),
                                Quantity = reader.GetInt32(reader.GetOrdinal("Quantity")),
                                Description = reader.GetString(reader.GetOrdinal("Description")),
                                IsDeleted = reader.GetBoolean(reader.GetOrdinal("IsDeleted"))
                            };
                        }
                    }
                }
            }
            return product;
        }
        public async Task<int> InsertProductAsync(ProductDTO product)
        {
            var query = @"INSERT INTO Products (Name, Price, Quantity, Description, IsDeleted) 
                        VALUES (@Name, @Price, @Quantity, @Description, 0);
                        SELECT CAST(SCOPE_IDENTITY() as int);";
            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@Quantity", product.Quantity);
                    command.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);
                    // ExecuteScalar is used here to return the first column of the first row in the result set
                    int productId = (int)await command.ExecuteScalarAsync();
                    return productId;
                }
            }
        }
        public async Task UpdateProductAsync(ProductDTO product)
        {
            var query = "UPDATE Products SET Name = @Name, Price = @Price, Quantity = @Quantity, Description = @Description WHERE ProductId = @ProductId";
            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", product.ProductId);
                    command.Parameters.AddWithValue("@Name", product.Name);
                    command.Parameters.AddWithValue("@Price", product.Price);
                    command.Parameters.AddWithValue("@Quantity", product.Quantity);
                    command.Parameters.AddWithValue("@Description", product.Description ?? (object)DBNull.Value);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
        public async Task DeleteProductAsync(int productId)
        {
            var query = "UPDATE Products SET IsDeleted = 1 WHERE ProductId = @ProductId";
            using (var connection = _connectionFactory.CreateConnection())
            {
                await connection.OpenAsync();
                using (var command = new NpgsqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@ProductId", productId);
                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
