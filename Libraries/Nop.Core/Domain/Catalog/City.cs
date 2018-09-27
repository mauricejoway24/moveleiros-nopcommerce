namespace Nop.Core.Domain.Catalog
{
    public class City : BaseEntity
    {
        public string Description { get; set; }
        public string UF { get; set; }
        public string UFDescricao { get; set; }
        public string ShortUrl { get; set; }
    }
}
