using Facilis.Core.Abstractions;
using Facilis.Core.Enums;
using Microsoft.EntityFrameworkCore;

namespace Facilis.ToDoApp
{
    public class AppDbContext : DbContext
    {
        public DbSet<ToDoItem> ToDoItems { get; set; }

        #region Constructor(s)

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion Constructor(s)
    }

    public class ToDoItem : IEntityWithId, IEntityWithStatus
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();

        public StatusTypes Status { get; set; }

        public string Name { get; set; } = string.Empty;
    }
}