using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Tazkara.Application.DTOs.Category;
using Tazkara.Application.Interfaces;

namespace Tazkara.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var response = await _categoryService.GetAllCategoriesAsync();
            return Ok(response);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Create(CreateCategoryRequest request)
        {
            var response = await _categoryService.CreateCategoryAsync(request);
            if (response.Success)
            {
                return Ok(response);
            }
            return BadRequest(response);
        }
    }
}
