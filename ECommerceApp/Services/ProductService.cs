using Microsoft.EntityFrameworkCore;
using ECommerceApp.Data;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.ProductDTOs;
using ECommerceApp.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.Formats.Asn1;
using System.Xml.Linq;
namespace ECommerceApp.Services
{
    public class ProductService
    {
        private readonly ApplicationDbContext _context;

        public ProductService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<ProductResponseDTO>> CreateProductAsync(ProductCreateDTO productCreateDTO)
        {
            try
            {
                // Check if the product name already exists (case-insensitive)
                if(await  _context.Products.AnyAsync(p => p.Name.ToLower() == productCreateDTO.Name.ToLower()))
                {
                    return new ApiResponse<ProductResponseDTO>(400, "Product Name already exists.");
                }

                // Check if Category exists
                if(!await _context.Categories.AnyAsync(cat => cat.Id == productCreateDTO.CategoryId))
                {
                    return new ApiResponse<ProductResponseDTO>(400, "Specified category doesn't exists.");
                }

                // Manual mapping from DTO To Model

                var product = new Product
                {
                    Name = productCreateDTO.Name,
                    Description = productCreateDTO.Description,
                    Price = productCreateDTO.Price,
                    StockQuantity = productCreateDTO.StockQuantity,
                    ImageUrl = productCreateDTO.ImageUrl,
                    DiscountPercentage = productCreateDTO.DiscountPercentage,
                    CategoryId = productCreateDTO.CategoryId,
                    IsAvailable = true

                };

                // Add Product to the database

                _context.Products.Add(product);
                await _context.SaveChangesAsync();

                // Map to ProductResponseDTO

                var productResponse = new ProductResponseDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    StockQuantity = product.StockQuantity,
                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    DiscountPercentage = product.DiscountPercentage,
                    CategoryId = product.CategoryId,
                    IsAvailable = product.IsAvailable
                };

                return new ApiResponse<ProductResponseDTO>(200, productResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductResponseDTO>(500, $"An unexpected error occured while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ProductResponseDTO>> GetProductByIdAsync (int id)
        {
            try
            {
                var product = await _context.Products
                    .AsNoTracking()
                    .FirstOrDefaultAsync(p => p.Id == id);

                if(product == null)
                {
                    return new ApiResponse<ProductResponseDTO>(404, "Product not found.");
                }

                // Map to ProductResponseDTO

                var productResponse = new ProductResponseDTO
                {
                    Id = product.Id,
                    Name = product.Name,
                    Description = product.Description,
                    StockQuantity = product.StockQuantity,
                    ImageUrl = product.ImageUrl,
                    Price = product.Price,
                    DiscountPercentage = product.DiscountPercentage,
                    CategoryId = product.CategoryId,
                    IsAvailable = product.IsAvailable
                };

                return new ApiResponse<ProductResponseDTO>(200,productResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ProductResponseDTO>(500,$"An unexpeected error occured while processing your request, Error: {ex.Message}" );    
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateProductAsync(ProductUpdateDTO productUpdateDTO)
        {
            try
            {
                var product = await _context.Products.FindAsync(productUpdateDTO.Id);

                if(product == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Prdoduct not found.");
                }

                //// Check if the product deleted
                //if(!product.IsAvailable)
                //{
                //    return new ApiResponse<ConfirmationResponseDTO>(400, "Can't update a deleted Product.");
                //}

                // Check if the new product name is already exists (case-insensetive). excluding the current product
                if(await  _context.Products.AnyAsync(p => p.Name.ToLower() == productUpdateDTO.Name.ToLower() && p.Id == productUpdateDTO.Id))
                {
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Another product with the same name already exists.");
                }

                // Check if the category exists
                if(!await _context.Categories.AnyAsync(cat => cat.Id == productUpdateDTO.CategoryId))
                {
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Specified category doesn't exists.");
                }

                // Update product properties manually

                product.Name = productUpdateDTO.Name;
                product.Description = productUpdateDTO.Description;
                product.Price = productUpdateDTO.Price;
                product.StockQuantity = productUpdateDTO.StockQuantity;
                product.ImageUrl = productUpdateDTO.ImageUrl;
                product.DiscountPercentage = productUpdateDTO.DiscountPercentage;
                product.CategoryId = productUpdateDTO.CategoryId;

               await _context.SaveChangesAsync();

                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Product with Id {productUpdateDTO.Id} updated successfully."
                };

                return new ApiResponse<ConfirmationResponseDTO>(200, confirmationMessage);

            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occured while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteProductAsync(int id)
        {
            try
            {
                var product = await _context.Products.FirstOrDefaultAsync(p => p.Id == id);

                if(product == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Product not found.");
                }

                //  Implemeting soft delete

                product.IsAvailable = false;

                await _context.SaveChangesAsync();

                // Prepare confirmtaion message 

                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Product with Id {id} deleted successfully."
                };

                return new ApiResponse<ConfirmationResponseDTO>(200, confirmationMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500,$"An unexpected error occured while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<ProductResponseDTO>>> GetAllProductsAsync()
        {
            try
            {
                var products = await _context.Products
                    .AsNoTracking()
                    .ToListAsync();

                var productList = products.Select(p => new ProductResponseDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ImageUrl = p.ImageUrl,
                    DiscountPercentage = p.DiscountPercentage,
                    CategoryId = p.CategoryId,
                    IsAvailable = p.IsAvailable,
                }).ToList();

                return new ApiResponse<List<ProductResponseDTO>>(200, productList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<ProductResponseDTO>>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<ProductResponseDTO>>> GetAllProductsByCategoryAsync(int categoryId)
        {
            try
            {
                // Retrieve products associated with the specified category

                var products = await _context.Products
                    .AsNoTracking()
                    .Include(p => p.Category)
                    .Where(p => p.CategoryId == categoryId && p.IsAvailable)
                    .ToListAsync();

                if (products == null || products.Count == 0)
                {

                    return new ApiResponse<List<ProductResponseDTO>>(404, "Products not found.");
                    
                }

                var productList = products.Select(p => new ProductResponseDTO
                {
                    Id = p.Id,
                    Name = p.Name,
                    Price = p.Price,
                    StockQuantity = p.StockQuantity,
                    ImageUrl = p.ImageUrl,
                    Description = p.Description,
                    DiscountPercentage = p.DiscountPercentage,
                    CategoryId = categoryId,
                    IsAvailable = p.IsAvailable,

                }).ToList();

                return new ApiResponse<List<ProductResponseDTO>>(200, productList);
            }
            catch (Exception ex)
            {
               return new ApiResponse<List<ProductResponseDTO>>(500,$"An unexpected error occured while processing your request, Error: {ex.Message}" );
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateProductsStatusAsync(ProductStatusUpdateDTO productStatusUpdateDTO)
        {
            try
            {
                var product = await _context.Products
                    .FirstOrDefaultAsync(p => p.Id == productStatusUpdateDTO.ProductId);

                if(product == null)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Product not found.");
                }

                product.IsAvailable = productStatusUpdateDTO.IsAvailable;

                await _context.SaveChangesAsync();

                // Prepare confirmation message 

                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Product with Id {productStatusUpdateDTO.ProductId} Status Updated Sucessfully."
                };

                return new ApiResponse<ConfirmationResponseDTO>(200,confirmationMessage);
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500,$"An unexpected error occurred while processing your request, Error: {ex.Message}" );
            }
        }
    }
}
