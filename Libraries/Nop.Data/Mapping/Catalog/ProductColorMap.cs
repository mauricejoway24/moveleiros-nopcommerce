using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public class ProductColorMap : NopEntityTypeConfiguration<ProductColor>
    {
        public ProductColorMap()
        {
            ToTable("Product_Color_Mapping");

            HasKey(t => t.Id);

            HasRequired(t => t.Picture)
                .WithMany()
                .HasForeignKey(t => t.PictureId);


            HasRequired(t => t.Product)
                .WithMany(p => p.ProductColors)
                .HasForeignKey(t => t.ProductId);
        }
    }
}
