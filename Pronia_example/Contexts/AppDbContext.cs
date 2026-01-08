using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Pronia_example.Models;
using System.Reflection;

namespace Pronia_example.Contexts
{
    public class AppDbContext:IdentityDbContext<AppUser>
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ProniaDb;Trusted_Connection=True;TrustServerCertificate=True");
        //    base.OnConfiguring(optionsBuilder);
        //}

        public AppDbContext(DbContextOptions options):base(options)
        {
            
        }

		protected override void OnModelCreating(ModelBuilder builder)
		{
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
			base.OnModelCreating(builder);
		}

		public DbSet <AppFeature> AppFeatures { get; set; }
        public DbSet <Product> Products { get; set; }
        public DbSet <Productİmage> Productİmages { get; set; }
        public DbSet <Category> Categories { get; set; }

        public DbSet <Tag> Tags { get; set; }
        public DbSet <ProductTag> ProductTags { get; set; }
		public DbSet<BasketItem> BasketItems { get; set; }
	}
}
