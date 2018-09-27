using FluentValidation;
using Nop.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Catalog
{
    public class BrandValidator : BaseNopValidator<BrandModel>
    {
        public BrandValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Moveleiros.Admin.Catalog.Brands.Fields.Name.Required"));

            SetDatabaseValidationRules<Brand>(dbContext);
        }
    }
}