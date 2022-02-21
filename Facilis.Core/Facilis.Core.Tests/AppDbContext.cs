using Microsoft.EntityFrameworkCore;

namespace Facilis.Core.Tests
{
    public class AppDbContext : DbContext
    {
        #region Constructor(s)

        public AppDbContext(DbContextOptions options) : base(options)
        {
        }

        #endregion Constructor(s)
    }
}