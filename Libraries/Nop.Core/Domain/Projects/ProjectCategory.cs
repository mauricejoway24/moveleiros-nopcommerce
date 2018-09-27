using Nop.Core.Domain.Catalog;

namespace Nop.Core.Domain.Projects
{
    public class ProjectCategory : BaseEntity
    {
        // Product_Category_Mapping Fields:
        // IsFeaturedProduct = 0
        // DisplayOrder = 1

        public int ProjectId { get; set; }
        public int CategoryId { get; set; }

        public Project Project { get; set; }
        public Category Category { get; set; }
    }
}