namespace DotNetAzure_API.Models.Entities
{
    public class TaskItem
    {
        public Guid Id { get; set; }

        public required string Title { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }
    }
}
