using ECommerceAPI.Data;
using ECommerceAPI.DTO;
using ECommerceAPI.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace ECommerceAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly ProductRepository _productRepository;
        public ProductController(ProductRepository productRepository)
        {
            _productRepository = productRepository;
        }
        // GET: api/product
        [HttpGet]
        public async Task<APIResponse<List<Product>>> GetAllProducts()
        {
            try
            {
                var products = await _productRepository.GetAllProductsAsync();
                return new APIResponse<List<Product>>(products, "Retrieved all products successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<List<Product>>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }
        // GET: api/product/5
        [HttpGet("{id}")]
        public async Task<APIResponse<Product>> GetProductById(int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                {
                    return new APIResponse<Product>(HttpStatusCode.NotFound, "Product not found.");
                }
                return new APIResponse<Product>(product, "Product retrieved successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<Product>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }
        // POST: api/product
        [HttpPost]
        public async Task<APIResponse<ProductResponseDTO>> CreateProduct([FromBody] ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<ProductResponseDTO>(HttpStatusCode.BadRequest, "Invalid data", ModelState);
            }
            try
            {
                var productId = await _productRepository.InsertProductAsync(product);
                var responseDTO = new ProductResponseDTO { ProductId = productId };
                return new APIResponse<ProductResponseDTO>(responseDTO, "Product created successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<ProductResponseDTO>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }
        // PUT: api/product/5
        [HttpPut("{id}")]
        public async Task<APIResponse<bool>> UpdateProduct(int id, [FromBody] ProductDTO product)
        {
            if (!ModelState.IsValid)
            {
                return new APIResponse<bool>(HttpStatusCode.BadRequest, "Invalid data", ModelState);
            }
            if (id != product.ProductId)
            {
                return new APIResponse<bool>(HttpStatusCode.BadRequest, "Mismatched product ID");
            }
            try
            {
                await _productRepository.UpdateProductAsync(product);
                return new APIResponse<bool>(true, "Product updated successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<bool>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }
        // DELETE: api/product/5
        [HttpDelete("{id}")]
        public async Task<APIResponse<bool>> DeleteProduct(int id)
        {
            try
            {
                var product = await _productRepository.GetProductByIdAsync(id);
                if (product == null)
                {
                    return new APIResponse<bool>(HttpStatusCode.NotFound, "Product not found.");
                }
                await _productRepository.DeleteProductAsync(id);
                return new APIResponse<bool>(true, "Product deleted successfully.");
            }
            catch (Exception ex)
            {
                return new APIResponse<bool>(HttpStatusCode.InternalServerError, "Internal server error: " + ex.Message);
            }
        }
    }
}
