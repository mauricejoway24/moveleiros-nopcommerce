using Nop.Core.Domain.Seo;

namespace Nop.Data.Mapping.Seo
{
    public class KeywordsMappingMap : NopEntityTypeConfiguration<KeywordsMapping>
    {
        public KeywordsMappingMap()
        {
            ToTable("KeywordsMapping");
            HasKey(t => t.Id);
            Property(m => m.MetaKeywords).HasMaxLength(400);
            Property(m => m.MetaTitle).HasMaxLength(400);
        }
    }
}
