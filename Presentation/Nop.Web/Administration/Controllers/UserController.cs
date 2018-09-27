using Nop.Admin.Models.Customers;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Common;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Web.Framework.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public class UserController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly ICustomerService _customerService;
        private readonly IWorkContext _workContext;
        private readonly ILocalizationService _localizationService;
        private readonly IStoreService _storeService;
        private readonly CustomerSettings _customerSettings;
        private readonly IStoreContext _storeContext;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly ICustomerRegistrationService _customerRegistrationService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly IStoreMappingService _storeMappingService;

        public UserController(ICustomerService customerService, 
            IPermissionService permissionService,
            IWorkContext workContext,
            ILocalizationService localizationService,
            IStoreService storeService,
            CustomerSettings customerSettings,
            IStoreContext storeContext,
            IGenericAttributeService genericAttributeService,
            ICustomerActivityService customerActivityService,
            ICustomerRegistrationService customerRegistrationService,
            IStoreMappingService storeMappingService)
        {
            _permissionService = permissionService;
            _customerService = customerService;
            _workContext = workContext;
            _localizationService = localizationService;
            _storeService = storeService;
            _storeContext = storeContext;
            _genericAttributeService = genericAttributeService;
            _customerRegistrationService = customerRegistrationService;
            _customerActivityService = customerActivityService;
            _customerSettings = customerSettings;
            _storeMappingService = storeMappingService;
        }

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            //load registered customers by default
            var defaultRoleIds = new List<int> {
                _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Stores).Id,
                _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Vendors).Id,
            };

            var model = new UserListModel
            {
                SearchCustomerRoleIds = defaultRoleIds,
            };

            var allRoles = _customerService.GetAllCustomerRoles(true);

            foreach (var role in allRoles)
            {
                if (!_workContext.CurrentCustomer.IsInCustomerRole(SystemCustomerRoleNames.Administrators) &&
                    !defaultRoleIds.Contains(role.Id))
                    continue;

                model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = defaultRoleIds.Any(x => x == role.Id)
                });
            }

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult UserList(DataSourceRequest command, UserListModel model,
            [ModelBinder(typeof(CommaSeparatedModelBinder))] int[] searchCustomerRoleIds)
        {
            // we use own binder for searchCustomerRoleIds property 
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedKendoGridJson();

            if (searchCustomerRoleIds.Length == 0)
                searchCustomerRoleIds = new int[] { _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Vendors).Id };

            var customers = _customerService.GetAllCustomers(
                customerRoleIds: searchCustomerRoleIds,
                email: model.SearchEmail,
                firstName: model.SearchFirstName,
                lastName: model.SearchLastName,
                loadOnlyWithShoppingCart: false,
                pageIndex: command.Page - 1,
                pageSize: command.PageSize);

            var gridModel = new DataSourceResult
            {
                Data = customers.Select(PrepareCustomerModelForList),
                Total = customers.TotalCount
            };

            return Json(gridModel);
        }

        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            var model = new UserModel();

            PrepareCustomerModel(model, null, false);

            //default value
            model.Active = true;

            return View(model);
        }

        [ValidateInput(false)]
        [FormValueRequired("save", "save-continue")]
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(UserModel model, bool continueEditing, FormCollection form)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageUsers))
                return AccessDeniedView();

            if (!String.IsNullOrWhiteSpace(model.Email))
            {
                var cust2 = _customerService.GetCustomerByEmail(model.Email);

                if (cust2 != null)
                    ModelState.AddModelError("", "Email is already registered");
            }

            //validate customer roles
            var allCustomerRoles = _customerService.GetAllCustomerRoles(true);
            var newCustomerRoles = new List<CustomerRole>();

            foreach (var customerRole in allCustomerRoles)
                if (model.SelectedCustomerRoleIds.Contains(customerRole.Id))
                    newCustomerRoles.Add(customerRole);

            // Trick to include this customer always as Registered :)
            newCustomerRoles.Add(
                allCustomerRoles.Where(t => t.SystemName == SystemCustomerRoleNames.Registered).FirstOrDefault()
            );

            var customerRolesError = ValidateCustomerRoles(newCustomerRoles);

            if (!String.IsNullOrEmpty(customerRolesError))
            {
                ModelState.AddModelError("", customerRolesError);
                ErrorNotification(customerRolesError, false);
            }

            // Ensure that valid email address is entered if Registered role is checked to avoid registered customers with empty email address
            if (newCustomerRoles.Any() && 
                newCustomerRoles.FirstOrDefault(c => c.SystemName == SystemCustomerRoleNames.Registered) != null && 
                !CommonHelper.IsValidEmail(model.Email))
            {
                ModelState.AddModelError("", _localizationService.GetResource("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"));
                ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.ValidEmailRequiredRegisteredRole"), false);
            }

            if (!ModelState.IsValid)
            {
                PrepareCustomerModel(model, null, true);
                return View(model);
            }

            var customer = new Customer
            {
                CustomerGuid = Guid.NewGuid(),
                Email = model.Email,
                Active = model.Active,
                CreatedOnUtc = DateTime.UtcNow,
                LastActivityDateUtc = DateTime.UtcNow,
                RegisteredInStoreId = _storeContext.CurrentStore.Id,
                VendorId = 1 // Default Moveleiros Loja (URGENT: CHANGE IT)
            };
            _customerService.InsertCustomer(customer);

            // Map user automatically with 
            _storeMappingService.InsertStoreMapping(
                customerId: customer.Id,
                entityName: SystemCustomerRoleNames.Stores,
                storeId: _storeContext.CurrentStore.Id
            );

            //form fields
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.FirstName, model.FirstName);
            _genericAttributeService.SaveAttribute(customer, SystemCustomerAttributeNames.LastName, model.LastName);

            //password
            if (!String.IsNullOrWhiteSpace(model.Password))
            {
                var changePassRequest = new ChangePasswordRequest(model.Email, 
                    false, 
                    _customerSettings.DefaultPasswordFormat, 
                    model.Password
                );

                var changePassResult = _customerRegistrationService.ChangePassword(changePassRequest);

                if (!changePassResult.Success)
                {
                    foreach (var changePassError in changePassResult.Errors)
                        ErrorNotification(changePassError);
                }
            }

            //customer roles
            foreach (var customerRole in newCustomerRoles)
            {
                //ensure that the current customer cannot add to "Administrators" system role if he's not an admin himself
                if (customerRole.SystemName == SystemCustomerRoleNames.Administrators &&
                    !_workContext.CurrentCustomer.IsAdmin())
                    continue;

                customer.CustomerRoles.Add(customerRole);
            }
            _customerService.UpdateCustomer(customer);

            //ensure that a customer with a vendor associated is not in "Administrators" role
            //otherwise, he won't have access to other functionality in admin area
            if (customer.IsAdmin() && customer.VendorId > 0)
            {
                customer.VendorId = 0;
                _customerService.UpdateCustomer(customer);
                ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.AdminCouldNotbeVendor"));
            }

            //ensure that a customer in the Vendors role has a vendor account associated.
            //otherwise, he will have access to ALL products
            if (customer.IsVendor() && customer.VendorId == 0)
            {
                var vendorRole = customer
                    .CustomerRoles
                    .FirstOrDefault(x => x.SystemName == SystemCustomerRoleNames.Vendors);

                customer.CustomerRoles.Remove(vendorRole);
                _customerService.UpdateCustomer(customer);
                ErrorNotification(_localizationService.GetResource("Admin.Customers.Customers.CannotBeInVendoRoleWithoutVendorAssociated"));
            }

            //activity log
            _customerActivityService.InsertActivity("AddNewCustomer", _localizationService.GetResource("ActivityLog.AddNewCustomer"), customer.Id);

            SuccessNotification(_localizationService.GetResource("Moveleiros.Admin.Settings.Users.Added"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = customer.Id });
            }

            return RedirectToAction("List");
        }

        #region Utils

        [NonAction]
        protected virtual void PrepareCustomerModel(UserModel model, Customer customer, bool excludeProperties)
        {
            var allStores = _storeService.GetAllStores();

            if (customer != null)
            {
                model.Id = customer.Id;

                if (!excludeProperties)
                {
                    model.Email = customer.Email;
                    model.Active = customer.Active;

                    model.SelectedCustomerRoleIds = customer.CustomerRoles.Select(cr => cr.Id).ToList();

                    //form fields
                    model.FirstName = customer.GetAttribute<string>(SystemCustomerAttributeNames.FirstName);
                    model.LastName = customer.GetAttribute<string>(SystemCustomerAttributeNames.LastName);
                }
            }

            //customer roles
            var allRoles = _customerService.GetAllCustomerRoles(true);
            var adminRole = allRoles.FirstOrDefault(c => c.SystemName == SystemCustomerRoleNames.Registered);

            //precheck Registered Role as a default role while creating a new customer through admin
            if (customer == null && adminRole != null)
            {
                model.SelectedCustomerRoleIds.Add(adminRole.Id);
            }

            var defaultRoleIds = new List<int> {
                _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Stores).Id,
                _customerService.GetCustomerRoleBySystemName(SystemCustomerRoleNames.Vendors).Id,
            };

            foreach (var role in allRoles)
            {
                if (_workContext.CurrentCustomer.IsInCustomerRole(SystemCustomerRoleNames.Stores) &&
                    !defaultRoleIds.Contains(role.Id))
                    continue;

                model.AvailableCustomerRoles.Add(new SelectListItem
                {
                    Text = role.Name,
                    Value = role.Id.ToString(),
                    Selected = model.SelectedCustomerRoleIds.Contains(role.Id)
                });
            }
        }

        [NonAction]
        protected virtual CustomerModel PrepareCustomerModelForList(Customer customer)
        {
            return new CustomerModel
            {
                Id = customer.Id,
                Email = customer.IsRegistered() ? customer.Email : _localizationService.GetResource("Admin.Customers.Guest"),
                CustomerRoleNames = string.Join(", ", customer.CustomerRoles.Select(t => t.Name)),
                FullName = customer.GetFullName(),
                Active = customer.Active
            };
        }

        [NonAction]
        protected virtual string ValidateCustomerRoles(IList<CustomerRole> customerRoles)
        {
            if (customerRoles == null)
                throw new ArgumentNullException("customerRoles");

            //ensure a customer is not added to both 'Guests' and 'Registered' customer roles
            //ensure that a customer is in at least one required role ('Guests' and 'Registered')
            bool isInGuestsRole = customerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Guests) != null;
            bool isInRegisteredRole = customerRoles.FirstOrDefault(cr => cr.SystemName == SystemCustomerRoleNames.Registered) != null;

            if (isInGuestsRole && isInRegisteredRole)
                return _localizationService.GetResource("Admin.Customers.Customers.GuestsAndRegisteredRolesError");
            if (!isInGuestsRole && !isInRegisteredRole)
                return _localizationService.GetResource("Admin.Customers.Customers.AddCustomerToGuestsOrRegisteredRoleError");

            //no errors
            return "";
        }

        #endregion
    }
}