using FluentValidation;
using Nop.Admin.Models.Stores;
using Nop.Core.Domain.Stores;
using Nop.Data;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

namespace Nop.Admin.Validators.Stores
{
    public partial class StoreValidator : BaseNopValidator<StoreModel>
    {
        public StoreValidator(ILocalizationService localizationService, IDbContext dbContext)
        {
            RuleFor(x => x.Name).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Name.Required"));
            RuleFor(x => x.CompanyName).NotEmpty()
                .WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.CompanyName.Required"));
            // RuleFor(x => x.Url).NotEmpty().WithMessage(localizationService.GetResource("Admin.Configuration.Stores.Fields.Url.Required"));
            RuleFor(t => t.LojistaId).GreaterThan(0).Unless(t => t.Id == 1)
                .WithMessage(localizationService.GetResource("Moveleiros.Admin.Configuration.Stores.Fields.LojistaId.Required"));
            RuleFor(t => t.CityId).GreaterThan(0)
                .WithMessage(localizationService.GetResource("Moveleiros.Admin.Configuration.Stores.Fields.CityId.Required"));

            SetDatabaseValidationRules<Store>(dbContext);
        }
    }
}