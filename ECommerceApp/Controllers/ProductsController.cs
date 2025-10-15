using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.ProductDTOs;
using ECommerceApp.Services;

namespace ECommerceApp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductsController : ControllerBase
    {
        private readonly ProductService _productService;

        // Injecting the ProductService
        public ProductsController(ProductService productService)
        {
            _productService = productService;
        }

        // Create a new product 
        [HttpPost("CreateProduct")]
        public async Task<ActionResult<ApiResponse<ProductResponseDTO>>> CreateProduct([FromBody] ProductCreateDTO productCreateDTO)
        {
            var response = await _productService.CreateProductAsync(productCreateDTO);

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }

        // Retrieves a product by ID 
        [HttpGet("GetProductById/{id}")]
        public async Task<ActionResult<ApiResponse<ProductResponseDTO>>> GetProductById(int id)
        {
            var response = await _productService.GetProductByIdAsync(id);

            if (response.StatusCode != 200) 
            {
                return StatusCode(response.StatusCode,response);    
            }

            return Ok(response);
        }

        // Update an existing Product
        [HttpPut("UpdateProduct")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> UpdateProduct([FromBody] ProductUpdateDTO product)
        {
            var response =  await _productService.UpdateProductAsync(product);

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Deletes a product by ID
        [HttpDelete("DeleteProduct/{id}")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> DeleteProduct(int id)
        {
            var response = await _productService.DeleteProductAsync(id);

            if( response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Retrieves all products.
        [HttpGet("GetAllProducts")]
        public async Task<ActionResult<ApiResponse<List<ProductResponseDTO>>>> GetAllProducts()
        {
            var response = await _productService.GetAllProductsAsync();

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }

            return Ok(response);
        }

        // Retrieves all products by category
        [HttpGet("GetAllProductsByCategory/{categoryId}")]
        public async Task<ActionResult<ApiResponse<List<ProductResponseDTO>>>> GetAllProductsByCategory(int categoryId)
        {
            var response = await _productService.GetAllProductsByCategoryAsync(categoryId);

            if(Response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }

        // Udate Product Status
        [HttpPut("UpdateProductStatus")]
        public async Task<ActionResult<ApiResponse<ConfirmationResponseDTO>>> UpdateProductStatus(ProductStatusUpdateDTO productStatusUpdateDTO)
        {
            var response = await _productService.UpdateProductsStatusAsync(productStatusUpdateDTO);

            if(response.StatusCode != 200)
            {
                return StatusCode(response.StatusCode, response);
            }
            return Ok(response);
        }
        
    }

}
