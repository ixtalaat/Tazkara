using Mapster;
using Tazkara.Application.DTOs.Category;
using Tazkara.Application.Interfaces;
using Tazkara.Application.Wrappers;
using Tazkara.Domain.Entities;

namespace Tazkara.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.ListAllAsync();
            var dtos = categories.Adapt<List<CategoryDto>>();
            return ApiResponse<List<CategoryDto>>.SuccessResponse(dtos);
        }

        public async Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Name))
            {
                return ApiResponse<CategoryDto>.ErrorResponse("Category name is required.");
            }

            var exists = await _categoryRepository.CategoryExistsAsync(request.Name);
            if (exists)
            {
                return ApiResponse<CategoryDto>.ErrorResponse("Category already exists.");
            }

            var category = new Category { Name = request.Name };
            await _categoryRepository.AddAsync(category);

            return ApiResponse<CategoryDto>.SuccessResponse(category.Adapt<CategoryDto>());
        }
    }
}
