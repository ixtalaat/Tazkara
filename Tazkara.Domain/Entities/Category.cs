namespace Tazkara.Domain.Entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        // Navigation property
        public ICollection<Event> Events { get; set; } = new List<Event>();
    }
}
