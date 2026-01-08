using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Pronia_example.Configurations
{
	public class BasketItemConfiguration : IEntityTypeConfiguration<BasketItem>
	{
		public void Configure(EntityTypeBuilder<BasketItem> builder)
		{
			builder.Property(x => x.Count).IsRequired();
			builder.ToTable(opt =>
			{
				opt.HasCheckConstraint("CK_Product_Count", "[Count]>0");
			});
			builder.HasOne(x => x.Product).WithMany(x => x.BasketItems).HasForeignKey(x => x.ProductId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);

			builder.HasOne(x => x.AppUser).WithMany(x => x.BasketItems).HasForeignKey(x => x.AppUserId).HasPrincipalKey(x => x.Id).OnDelete(DeleteBehavior.Cascade);

		}
	}
}
