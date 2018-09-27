using System;

namespace Nop.Core.Domain.Seo
{
    public class KeywordsMapping : BaseEntity
    {
        /// <summary>
        /// Gets or sets the Keyword
        /// </summary>
        public string Keyword { get; set; }
        /// <summary>
        /// Gets or sets the KeywordNormalized
        /// </summary>
        public string KeywordNormalized { get; set; }

        /// <summary>
        /// Gets or sets the meta keywords
        /// </summary>
        public string MetaKeywords { get; set; }
        /// <summary>
        /// Gets or sets the meta description
        /// </summary>
        public string MetaDescription { get; set; }
        /// <summary>
        /// Gets or sets the meta title
        /// </summary>
        public string MetaTitle { get; set; }

        #region Filter properties
        public int? CityId { get; set; }
        public int? StoreId { get; set; }
        public int? BrandId { get; set; }
        public int? CategoryId { get; set; }
        public decimal? FromArea { get; set; }
        public decimal? ToArea { get; set; }
        public decimal? FromPrice { get; set; }
        public decimal? ToPrice { get; set; }
        #endregion

        /// <summary>
        /// Gets or sets a value indicating whether the entity has been deleted
        /// </summary>
        public bool Deleted { get; set; }
        /// <summary>
        /// Gets or sets the date and time of product creation
        /// </summary>
        public DateTime CreatedOnUtc { get; set; }
        /// <summary>
        /// Gets or sets the date and time of product update
        /// </summary>
        public DateTime UpdatedOnUtc { get; set; }
    }
}
