using Mapster;
using Tazkara.Application.DTOs.Category;
using Tazkara.Application.DTOs.Event;
using Tazkara.Domain.Entities;

namespace Tazkara.Application.Mappings
{
    public static class MappingProfile
    {
        public static void RegisterMappings()
        {
            TypeAdapterConfig<Category, CategoryDto>.NewConfig();
            
            TypeAdapterConfig<Event, EventDto>.NewConfig()
                .Map(dest => dest.CategoryName, src => src.Category != null ? src.Category.Name : string.Empty)
                .Map(dest => dest.OrganizerName, src => src.Organizer != null ? $"{src.Organizer.FirstName} {src.Organizer.LastName}" : string.Empty)
                .Map(dest => dest.Status, src => src.Status.ToString());
        }
    }
}
