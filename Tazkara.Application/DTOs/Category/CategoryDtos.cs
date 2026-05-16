namespace Tazkara.Application.DTOs.Category
{
    public class CategoryDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class CreateCategoryRequest
    {
        public string Name { get; set; } = string.Empty;
    }
}
