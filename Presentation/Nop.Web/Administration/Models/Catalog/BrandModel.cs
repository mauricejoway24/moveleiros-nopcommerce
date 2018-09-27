using FluentValidation.Attributes;
using Nop.Admin.Validators.Catalog;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Nop.Admin.Models.Catalog
{
    [Validator(typeof(BrandValidator))]
    public class BrandModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Moveleiros.Admin.Catalog.Brands.Fields.Name")]
        public string Name { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Catalog.Brands.Fields.MetaKeywords")]
        public string MetaKeywords { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Catalog.Brands.Fields.MetaDescription")]
        public string MetaDescription { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Catalog.Brands.Fields.MetaTitle")]
        public string MetaTitle { get; set; }

        [UIHint("Picture")]
        [NopResourceDisplayName("Moveleiros.Admin.Catalog.Brands.Fields.Picture")]
        public int PictureId { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Catalog.Brands.Fields.Published")]
        public bool Published { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Catalog.Brands.Fields.Deleted")]
        public bool Deleted { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Catalog.Brands.Fields.DisplayOrder")]
        public int DisplayOrder { get; set; }
    }
}