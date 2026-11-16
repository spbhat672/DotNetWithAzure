using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureFunctionApp.Models
{
    public class TaskItemCreatedEvent
    {
        public required string Title { get; set; }

        public string Description { get; set; } = string.Empty;

        public DateTime CreatedOnUtc { get; set; }

        public DateTime? UpdatedOnUtc { get; set; }
    }
}
