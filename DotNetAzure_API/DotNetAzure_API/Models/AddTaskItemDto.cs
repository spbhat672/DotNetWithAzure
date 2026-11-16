namespace DotNetAzure_API.Models
{
    public class AddTaskItemDto
    {
        public required string Title { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }
    }
}
