using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pronia_example.Configurations
{
	public class ProductConfiguration : IEntityTypeConfiguration<Product>
	{
		public void Configure(EntityTypeBuilder<Product> builder)
		{
			builder.Property(x => x.Name).IsRequired().HasMaxLength(256);
			builder.Property(x=>x.Description).IsRequired(false).HasMaxLength(256);
			builder.Property(x=>x.Price).HasColumnType("decimal(10,2)").IsRequired();
			builder.Property(x => x.CategoryId).IsRequired();

			builder.HasOne(x=>x.Category).WithMany(x=>x.Products).HasForeignKey(x=>x.CategoryId).HasPrincipalKey(x=>x.Id).OnDelete(DeleteBehavior.Restrict);

			builder.HasMany(x => x.Productİmages).WithOne(x => x.product).HasForeignKey(x => x.ProductId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);
			builder.HasMany(x => x.ProductTags).WithOne(x => x.Product).HasForeignKey(x => x.ProductId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);

		}

	}
}
