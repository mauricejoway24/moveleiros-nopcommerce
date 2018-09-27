using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;

namespace Nop.Admin.Models.Catalog
{
    public class BrandListModel : BaseNopModel
    {
        [NopResourceDisplayName("Moveleiros.Admin.Catalog.Brands.List.SearchBrandName")]
        public string SearchBrandName { get; set; }
    }
}