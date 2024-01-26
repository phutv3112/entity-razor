using Microsoft.EntityFrameworkCore;

namespace razorweb.models
{
    public class BlogContext : DbContext
    {
        public DbSet<Article> articles { set; get; } //==> table name
        public BlogContext(DbContextOptions<BlogContext> options) : base(options)
        {
        }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}