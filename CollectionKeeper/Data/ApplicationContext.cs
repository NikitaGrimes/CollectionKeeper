using CollectionKeeper.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CollectionKeeper.Data
{
    public class ApplicationContext : IdentityDbContext<CollectionUser>
    {
        private readonly IConfiguration _configuration;

        public DbSet<Collection> Collections { get; set; }

        public DbSet<CollectionItem> Items { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<Topic> Topics { get; set; }

        public ApplicationContext(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }

        protected override void OnConfiguring(DbContextOptionsBuilder dbContextOptionsBuilder)
        {
            base.OnConfiguring(dbContextOptionsBuilder);
            dbContextOptionsBuilder.UseSqlServer(_configuration.GetConnectionString("ConnectionString"));
        }
    }
}
