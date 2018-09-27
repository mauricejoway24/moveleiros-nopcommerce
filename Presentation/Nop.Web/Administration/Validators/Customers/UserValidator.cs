using FluentValidation;
using Nop.Admin.Models.Customers;
using Nop.Core.Domain.Customers;
using Nop.Data;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using System.Collections.Generic;
using System.Linq;

namespace Nop.Admin.Validators.Customers
{
    public class UserValidator : BaseNopValidator<UserModel>
    {
        public UserValidator(ILocalizationService localizationService,
            ICustomerService customerService,
            IDbContext dbContext)
        {
            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress()
                .WithMessage(localizationService.GetResource("Admin.Common.WrongEmail"))
                .When(x => IsRegisteredCustomerRoleChecked(x, customerService));

            RuleFor(x => x.FirstName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Moveleiros.Admin.Users.Fields.FirstName.Required"));

            RuleFor(x => x.LastName)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Moveleiros.Admin.Users.Fields.LastName.Required"));

            RuleFor(x => x.Password)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Moveleiros.Admin.Users.Fields.Password.Required"));

            SetDatabaseValidationRules<Customer>(dbContext);
        }

        private bool IsRegisteredCustomerRoleChecked(UserModel model, ICustomerService customerService)
        {
            var allCustomerRoles = customerService.GetAllCustomerRoles(true);
            var newCustomerRoles = new List<CustomerRole>();

            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);

            bool isInRegisteredRole = newCustomerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Registered) != null;

            return isInRegisteredRole;
        }
    }
}