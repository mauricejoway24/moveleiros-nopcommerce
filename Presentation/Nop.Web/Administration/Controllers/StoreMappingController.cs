using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Admin.Extensions;
using Nop.Admin.Models.Stores;
using Nop.Core.Domain.Stores;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using Nop.Services.Customers;
using Nop.Core;

namespace Nop.Admin.Controllers
{
    [AdminAuthorize]
    public partial class StoreMappingController : BaseAdminController
    {
        private readonly ICustomerService _customerService;
        private readonly IStoreService _storeService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly ISettingService _settingService;
        private readonly ILanguageService _languageService;
        private readonly ILocalizationService _localizationService;
        private readonly ILocalizedEntityService _localizedEntityService;
        private readonly IPermissionService _permissionService;
        private readonly IWorkContext _workContext;

        public StoreMappingController(ICustomerService customerService, IStoreService storeService, IStoreMappingService storeMappingService,
            ISettingService settingService,
            ILanguageService languageService,
            ILocalizationService localizationService,
            ILocalizedEntityService localizedEntityService,
            IPermissionService permissionService,
            IWorkContext workContext
            )
        {
            this._customerService = customerService;
            this._storeService = storeService;
            this._storeMappingService = storeMappingService;
            this._settingService = settingService;
            this._languageService = languageService;
            this._localizationService = localizationService;
            this._localizedEntityService = localizedEntityService;
            this._permissionService = permissionService;
            this._workContext = workContext;
        }

        public ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            return View(new StoreMappingModel());
        }

        [HttpPost]
        public ActionResult List(DataSourceRequest command, StoreMappingModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedKendoGridJson();

            var storeModels = _storeMappingService.GetAllPagedStoreMapping(
                model.StoreName,
                "Store",
                command.Page,
                command.PageSize,
                model.RemoveCustomerProfile
            );

            var gridModel = new DataSourceResult
            {
                Data = storeModels.Select(x => new StoreMappingModel()
                {
                    Id = x.Id,
                    UserName = _customerService.GetCustomerById(x.EntityId).Email,
                    EntityId = x.EntityId,
                    EntityName = x.EntityName,
                    StoreName = x.Store.Name,
                    StoreUrl = x.Store.Url
                }),
                Total = storeModels.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var model = new StoreMappingModel();
            //stores
            var allStores = _storeService.GetAllStoresByEntityName(_workContext.CurrentCustomer.Id, "Stores");
            if (allStores.Count <= 0)
            {
                allStores = _storeService.GetAllStores();
                model.AvailableStores.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            }
            foreach (var s in allStores)
                model.AvailableStores.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });

            //Customer
            // TODO: Fix this shit caused by StoreMapping plugin...
            int[] searchCustomerRoleIds = new int[] { 5, 6, 14, 16, 18 };

            foreach (var s in _customerService.GetAllCustomers(customerRoleIds: searchCustomerRoleIds))
                model.AvailableCustomers.Add(new SelectListItem() { Text = s.Email, Value = s.Id.ToString() });

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public ActionResult Create(StoreMappingModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            if (ModelState.IsValid)
            {
                var storeMapping = model.ToEntity();
                _storeMappingService.InsertStoreMapping(storeMapping);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Stores.Added"));
                return continueEditing ? RedirectToAction("Edit", new { id = storeMapping.Id }) : RedirectToAction("List");
            }
            //If we got this far, something failed, redisplay form
            return View(model);
        }
        public ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var storeMapping = _storeMappingService.GetStoreMappingById(id);
            if (storeMapping == null)
                //No store found with the specified id
                return RedirectToAction("List");

            var model = storeMapping.ToModel();

            //stores
            var AllStores = _storeService.GetAllStoresByEntityName(_workContext.CurrentCustomer.Id, "Stores");
            if (AllStores.Count <= 0)
            {
                AllStores = _storeService.GetAllStores();
                model.AvailableStores.Add(new SelectListItem() { Text = _localizationService.GetResource("Admin.Common.All"), Value = "0" });
            }
            foreach (var s in AllStores)
                model.AvailableStores.Add(new SelectListItem() { Text = s.Name, Value = s.Id.ToString() });

            //Customer
            int[] searchCustomerRoleIds = new int[] { 3 };

            foreach (var s in _customerService.GetAllCustomers(customerRoleIds: searchCustomerRoleIds))
                model.AvailableCustomers.Add(new SelectListItem() { Text = s.Email, Value = s.Id.ToString() });

            return View(model);
        }
        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        [FormValueRequired("save", "save-continue")]
        public ActionResult Edit(StoreMappingModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var storeMapping = _storeMappingService.GetStoreMappingById(model.Id);
            if (storeMapping == null)
                //No store found with the specified id
                return RedirectToAction("List");

            if (ModelState.IsValid)
            {
                storeMapping = model.ToEntity(storeMapping);
                _storeMappingService.UpdateStoreMapping(storeMapping);

                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Stores.Updated"));
                return continueEditing ? RedirectToAction("Edit", new { id = storeMapping.Id }) : RedirectToAction("List");
            }

            //If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpPost]
        public ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageStores))
                return AccessDeniedView();

            var storeMapping = _storeMappingService.GetStoreMappingById(id);
            if (storeMapping == null)
                //No store found with the specified id
                return RedirectToAction("List");

            try
            {
                _storeMappingService.DeleteStoreMapping(storeMapping);
                SuccessNotification(_localizationService.GetResource("Admin.Configuration.Stores.Deleted"));
                return RedirectToAction("List");
            }
            catch (Exception exc)
            {
                ErrorNotification(exc);
                return RedirectToAction("Edit", new { id = storeMapping.Id });
            }
        }
    }
}
