using Microsoft.EntityFrameworkCore;
using Pronia_example.Models;

namespace Pronia_example.Contexts
{
    public class AppDbContext:DbContext
    {
        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    optionsBuilder.UseSqlServer("Server=(localdb)\\MSSQLLocalDB;Database=ProniaDb;Trusted_Connection=True;TrustServerCertificate=True");
        //    base.OnConfiguring(optionsBuilder);
        //}

        public AppDbContext(DbContextOptions options):base(options)
        {
            
        }

        public DbSet <AppFeature> AppFeatures { get; set; }
    }
}
