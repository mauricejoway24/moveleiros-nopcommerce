using Nop.Core.Domain.Catalog;

namespace Nop.Data.Mapping.Catalog
{
    public class CityMap : NopEntityTypeConfiguration<City>
    {
        public CityMap()
        {
            ToTable("City");
            HasKey(t => t.Id);
            Property(t => t.Description).HasMaxLength(40);
            Property(t => t.UF).HasMaxLength(2);
            Property(t => t.UFDescricao).HasMaxLength(20);
        }
    }
}
