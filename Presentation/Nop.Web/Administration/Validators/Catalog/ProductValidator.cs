using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Catalog
{
    public partial class ProductValidator : BaseNopValidator<ProductModel>
    {
        public ProductValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Catalog.Products.Fields.Name.Required"));

            //RuleFor(x => x.ProjectDescription)
            //    .NotEmpty()
            //    .WithMessage(localizationService.GetResource("Moveleiros.Admin.Catalog.Products.Fields.ProjectDescription.Required"));

            RuleFor(x => x.ProjectContains)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Moveleiros.Admin.Catalog.Products.Fields.ProjectContains.Required"));

            //RuleFor(x => x.DimensionImageId)
            //    .NotEmpty()
            //    .WithMessage(localizationService.GetResource("Moveleiros.Admin.Catalog.Products.Fields.DimensionImageId.Required"));

            SetDatabaseValidationRules<Product>(dbContext);
        }
    }
}