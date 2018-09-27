using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public class BrandMap : NopEntityTypeConfiguration<Brand>
    {
        public BrandMap()
        {
            ToTable("Brand");
            HasKey(t => t.Id);
            Property(m => m.Name).IsRequired().HasMaxLength(400);
            Property(m => m.MetaKeywords).HasMaxLength(400);
            Property(m => m.MetaTitle).HasMaxLength(400);
        }
    }
}
