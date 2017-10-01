using Microsoft.EntityFrameworkCore;

namespace BuildingRestFullApi.Entities
{
    public class LibraryContext : DbContext
    {
        public LibraryContext(DbContextOptions options)
            : base(options)
        {
                
        }

        public DbSet<Author> Authors { get; set; }

        public DbSet<Book> Books { get; set; }
    }
}
