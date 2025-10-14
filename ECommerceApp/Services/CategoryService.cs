using Microsoft.EntityFrameworkCore;
using ECommerceApp.DTOs;
using ECommerceApp.DTOs.CategoryDTO;
using ECommerceApp.Data;
using ECommerceApp.Models;
namespace ECommerceApp.Services
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;

        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ApiResponse<CategoryResponseDTO>> CreateCategoryAsync(CategoryCreateDTO categoryCreateDTO)
        {
            try
            {
               // Check if category name already exists (cas-insenstive)
               if(await _context.Categories.AnyAsync(c => c.Name.ToLower() == categoryCreateDTO.Name.ToLower()))
                {
                    return new ApiResponse<CategoryResponseDTO>(400, "Category name already exists.");
                }

                // Manual mapping from DTO to Model
                var category = new Category
                {
                    Name = categoryCreateDTO.Name,
                    Description = categoryCreateDTO.Description,
                    IsActive = true
                };

                // Add category to the database

                _context.Categories.Add(category);
                await _context.SaveChangesAsync();

                // Map to CategoryResponseDTO
                var categoryResponse = new CategoryResponseDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IsActive = category.IsActive
                };

                return new ApiResponse<CategoryResponseDTO>(200, categoryResponse);
            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryResponseDTO>(500,$"An unexpected error occured while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<CategoryResponseDTO>> GetCategoryByIdAsync(int id)
        {
            try
            {
                var category = await _context.Categories
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == id);

                if(category == null)
                {
                    return new ApiResponse<CategoryResponseDTO>(404, "Category not found.");
                }

                var categoryResponse = new CategoryResponseDTO
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    IsActive = category.IsActive
                };

                return new ApiResponse<CategoryResponseDTO>(200,categoryResponse);

            }
            catch (Exception ex)
            {
                return new ApiResponse<CategoryResponseDTO>(500, $"An unexpected error occured while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> UpdateCategoryAsync(CategoryUpdateDTO categoryUpdateDTO)
        {
            try
            {
                var category = await _context.Categories
                    .FindAsync(categoryUpdateDTO.Id);

                if( category == null )
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Category not found.");
                }


                // Check if the new category name already exists (excluding current category)

                if(await _context.Categories.AnyAsync(c => c.Name.ToLower() == categoryUpdateDTO.Name.ToLower() && c.Id != categoryUpdateDTO.Id ))
                {
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Another category with the same name already exists.");
                }

                // check if the category is deleted or not;

                if (!category.IsActive)
                {
                    return new ApiResponse<ConfirmationResponseDTO>(400, "Cannot update a deleted category.");
                }

                // Update category properties manually

                category.Name = categoryUpdateDTO.Name;
                category.Description = categoryUpdateDTO.Description;
                
                await _context.SaveChangesAsync();

                // Prepare confirmation message

                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Category with Id {categoryUpdateDTO.Id} Updated successfully."
                };

                return new ApiResponse<ConfirmationResponseDTO>(200, confirmationMessage);

                        
            }

            catch (Exception ex)
            {
                 return new ApiResponse<ConfirmationResponseDTO>(500,$"An unexpected error occurred while processing your request, Error: {ex.Message}" );
            }
        }

        public async Task<ApiResponse<ConfirmationResponseDTO>> DeleteCategoryAsync(int id)
        {
            try
            {
                var category = await _context.Categories
                    .Include(c => c.Products)
                    .FirstOrDefaultAsync(c => c.Id == id);

                if( category == null )
                {
                    return new ApiResponse<ConfirmationResponseDTO>(404, "Category not found.");
                }

                // Soft Delete

                category.IsActive = false;
                await _context.SaveChangesAsync();

                // Prepare confirmation Message 

                var confirmationMessage = new ConfirmationResponseDTO
                {
                    Message = $"Category with Id {id} deleted Successfully."
                };

                return new ApiResponse<ConfirmationResponseDTO>( 200, confirmationMessage);
                    
            }
            catch (Exception ex)
            {
                return new ApiResponse<ConfirmationResponseDTO>(500, $"An unexpected error occurred while processing your request, Error: {ex.Message}");
            }
        }

        public async Task<ApiResponse<List<CategoryResponseDTO>>> GetAllCategoriesAsync()
        {
            try
            {
                var categories = await _context.Categories
                    .AsNoTracking()
                    .ToListAsync();

                var categoryList = categories.Select(c => new  CategoryResponseDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    IsActive = c.IsActive
                }).ToList();

                return new ApiResponse<List<CategoryResponseDTO>>( 200, categoryList);
            }
            catch (Exception ex)
            {
                return new ApiResponse<List<CategoryResponseDTO>>(500, $"An unexpected error occured while processing your request, Error: {ex.Message}");
            }
        }
    }
}
