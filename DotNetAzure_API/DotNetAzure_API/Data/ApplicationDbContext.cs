using DotNetAzure_API.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace DotNetAzure_API.Data
{
    public class ApplicationDbContext: DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
            
        }

        public DbSet<TaskItem> TaskItems { get; set; }
    }
}
