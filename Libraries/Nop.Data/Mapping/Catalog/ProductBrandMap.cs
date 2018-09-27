using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public class ProductBrandMap : NopEntityTypeConfiguration<ProductBrand>
    {
        public ProductBrandMap()
        {
            ToTable("Product_Brand_Mapping");
            HasKey(t => t.Id);

            HasRequired(t => t.Brand)
                .WithMany()
                .HasForeignKey(t => t.BrandId);
        }
    }
}
