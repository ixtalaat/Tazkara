using Tazkara.Application.DTOs.Category;
using Tazkara.Application.Wrappers;

namespace Tazkara.Application.Interfaces
{
    public interface ICategoryService
    {
        Task<ApiResponse<List<CategoryDto>>> GetAllCategoriesAsync();
        Task<ApiResponse<CategoryDto>> CreateCategoryAsync(CreateCategoryRequest request);
    }
}
