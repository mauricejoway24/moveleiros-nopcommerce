using FluentValidation.Attributes;
using Nop.Admin.Validators.Customers;
using Nop.Web.Framework;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace Nop.Admin.Models.Customers
{
    [Validator(typeof(UserValidator))]
    public class UserModel : BaseNopEntityModel
    {
        [NopResourceDisplayName("Moveleiros.Admin.Settings.Users.Fields.Email")]
        public string Email { get; set; }

        [NoTrim]
        [DataType(DataType.Password)]
        [NopResourceDisplayName("Moveleiros.Admin.Settings.Users.Fields.Password")]
        public string Password { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Settings.Users.Fields.FirstName")]
        public string FirstName { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Settings.Users.Fields.LastName")]
        public string LastName { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Settings.Users.Fields.Active")]
        public bool Active { get; set; }

        [NopResourceDisplayName("Moveleiros.Admin.Settings.Users.Fields.CreatedOn")]
        public DateTime CreatedOn { get; set; }

        #region Roles

        [NopResourceDisplayName("Moveleiros.Admin.Settings.Users.Fields.CustomerRoles")]
        public string CustomerRoleNames { get; set; }

        public List<SelectListItem> AvailableCustomerRoles { get; set; } = new List<SelectListItem>();

        [NopResourceDisplayName("Moveleiros.Admin.Settings.Users.Fields.CustomerRoles")]
        [UIHint("MultiSelect")]
        public IList<int> SelectedCustomerRoleIds { get; set; } = new List<int>();

        #endregion
    }
}