namespace Marketplace.Dtos
{
    public class ItemDto
    {
        public int ItemId { get; set; }

        public string Title { get; set; } = null!;

        public string Description { get; set; } = null!;

        public string Condition { get; set; } = null!;

        public string TypeName { get; set; } = null!;
        public List<string> Tags { get; set; } = new List<string>();
    }
}
