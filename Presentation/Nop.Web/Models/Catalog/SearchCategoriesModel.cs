using Nop.Web.Framework.Mvc;

namespace Nop.Web.Models.Catalog
{
    public class SearchCategoriesModel : BaseNopModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public bool Selected { get; set; }
    }
}