using System;
using System.Linq;
using System.Web.Mvc;
using Nop.Core;
using Nop.Core.Domain.Catalog;
using Nop.Core.Domain.Customers;
using Nop.Core.Domain.Media;
using Nop.Core.Domain.Vendors;
using Nop.Services.Catalog;
using Nop.Services.Common;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Services.Vendors;
using Nop.Web.Factories;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Catalog;
using Nop.Services.Configuration;
using System.Collections.Generic;

namespace Nop.Web.Controllers
{
    public partial class CatalogController : BasePublicController
    {
        #region Fields

        private readonly ICatalogModelFactory _catalogModelFactory;
        private readonly IProductModelFactory _productModelFactory;
        private readonly ICategoryService _categoryService;
        private readonly IManufacturerService _manufacturerService;
        private readonly IProductService _productService;
        private readonly IVendorService _vendorService;
        private readonly IWorkContext _workContext;
        private readonly IStoreContext _storeContext;
        private readonly ILocalizationService _localizationService;
        private readonly IWebHelper _webHelper;
        private readonly IProductTagService _productTagService;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IAclService _aclService;
        private readonly IStoreMappingService _storeMappingService;
        private readonly IPermissionService _permissionService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly MediaSettings _mediaSettings;
        private readonly CatalogSettings _catalogSettings;
        private readonly VendorSettings _vendorSettings;
        private readonly IBrandService _brandService;
        private readonly ISettingService _settingService;

        #endregion

        #region Constructors

        public CatalogController(ICatalogModelFactory catalogModelFactory,
            IProductModelFactory productModelFactory,
            ICategoryService categoryService, 
            IManufacturerService manufacturerService,
            IProductService productService, 
            IVendorService vendorService,
            IWorkContext workContext, 
            IStoreContext storeContext,
            ILocalizationService localizationService,
            IWebHelper webHelper,
            IProductTagService productTagService,
            IGenericAttributeService genericAttributeService,
            IAclService aclService,
            IStoreMappingService storeMappingService,
            IPermissionService permissionService, 
            ICustomerActivityService customerActivityService,
            IBrandService brandService,
            MediaSettings mediaSettings,
            CatalogSettings catalogSettings,
            VendorSettings vendorSettings,
            ISettingService settingService)
        {
            _catalogModelFactory = catalogModelFactory;
            _productModelFactory = productModelFactory;
            _categoryService = categoryService;
            _manufacturerService = manufacturerService;
            _productService = productService;
            _vendorService = vendorService;
            _workContext = workContext;
            _storeContext = storeContext;
            _localizationService = localizationService;
            _webHelper = webHelper;
            _productTagService = productTagService;
            _genericAttributeService = genericAttributeService;
            _aclService = aclService;
            _storeMappingService = storeMappingService;
            _permissionService = permissionService;
            _customerActivityService = customerActivityService;
            _mediaSettings = mediaSettings;
            _catalogSettings = catalogSettings;
            _vendorSettings = vendorSettings;
            _brandService = brandService;
            _settingService = settingService;
        }

        #endregion

        #region Categories
        
        [NopHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult Category(int categoryId, CatalogPagingFilteringModel command)
        {
            var category = _categoryService.GetCategoryById(categoryId);
            if (category == null || category.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                !category.Published ||
                //ACL (access control list) 
                !_aclService.Authorize(category) ||
                //Store mapping
                !_storeMappingService.Authorize(category);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable && !_permissionService.Authorize(StandardPermissionProvider.ManageCategories))
                return InvokeHttp404();

            var searchCatModel = new List<SearchCategoriesModel>
            {
                new SearchCategoriesModel { Id = categoryId, Selected = true }
            };

            return Search(
                new SearchModel { SelectedCategories = searchCatModel },
                command
            );

            ////'Continue shopping' URL
            //_genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, 
            //    SystemCustomerAttributeNames.LastContinueShoppingPage, 
            //    _webHelper.GetThisPageUrl(false),
            //    _storeContext.CurrentStore.Id);

            ////display "edit" (manage) link
            //if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageCategories))
            //    DisplayEditLink(Url.Action("Edit", "Category", new { id = category.Id, area = "Admin" }));

            ////activity log
            //_customerActivityService.InsertActivity("PublicStore.ViewCategory", _localizationService.GetResource("ActivityLog.PublicStore.ViewCategory"), category.Name);

            ////model
            //var model = _catalogModelFactory.PrepareCategoryModel(category, command);

            ////template
            //var templateViewPath = _catalogModelFactory.PrepareCategoryTemplateViewPath(category.CategoryTemplateId);
            //return View(templateViewPath, model);
        }

        [ChildActionOnly]
        public virtual ActionResult CategoryNavigation(int currentCategoryId, int currentProductId)
        {
            var model = _catalogModelFactory.PrepareCategoryNavigationModel(currentCategoryId, currentProductId);
            return PartialView(model);
        }

        [ChildActionOnly]
        public virtual ActionResult TopMenu()
        {
            var model = _catalogModelFactory.PrepareTopMenuModel();
            return PartialView(model);
        }
        
        [ChildActionOnly]
        public virtual ActionResult HomepageCategories()
        {
            var model = _catalogModelFactory.PrepareHomepageCategoryModels();
            if (!model.Any())
                return Content("");

            return PartialView(model);
        }

        #endregion

        #region Manufacturers

        [NopHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult Manufacturer(int manufacturerId, CatalogPagingFilteringModel command)
        {
            var manufacturer = _manufacturerService.GetManufacturerById(manufacturerId);
            if (manufacturer == null || manufacturer.Deleted)
                return InvokeHttp404();

            var notAvailable =
                //published?
                !manufacturer.Published ||
                //ACL (access control list) 
                !_aclService.Authorize(manufacturer) ||
                //Store mapping
                !_storeMappingService.Authorize(manufacturer);
            //Check whether the current user has a "Manage categories" permission (usually a store owner)
            //We should allows him (her) to use "Preview" functionality
            if (notAvailable && !_permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                return InvokeHttp404();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer, 
                SystemCustomerAttributeNames.LastContinueShoppingPage, 
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);
            
            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageManufacturers))
                DisplayEditLink(Url.Action("Edit", "Manufacturer", new { id = manufacturer.Id, area = "Admin" }));

            //activity log
            _customerActivityService.InsertActivity("PublicStore.ViewManufacturer", _localizationService.GetResource("ActivityLog.PublicStore.ViewManufacturer"), manufacturer.Name);

            //model
            var model = _catalogModelFactory.PrepareManufacturerModel(manufacturer, command);
            
            //template
            var templateViewPath = _catalogModelFactory.PrepareManufacturerTemplateViewPath(manufacturer.ManufacturerTemplateId);
            return View(templateViewPath, model);
        }

        [NopHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult ManufacturerAll()
        {
            var model = _catalogModelFactory.PrepareManufacturerAllModels();
            return View(model);
        }

        [ChildActionOnly]
        public virtual ActionResult ManufacturerNavigation(int currentManufacturerId)
        {
            if (_catalogSettings.ManufacturersBlockItemsToDisplay == 0)
                return Content("");

            var model = _catalogModelFactory.PrepareManufacturerNavigationModel(currentManufacturerId);

            if (!model.Manufacturers.Any())
                return Content("");
            
            return PartialView(model);
        }

        #endregion

        #region Vendors

        [NopHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult Vendor(int vendorId, CatalogPagingFilteringModel command)
        {
            var vendor = _vendorService.GetVendorById(vendorId);
            if (vendor == null || vendor.Deleted || !vendor.Active)
                return InvokeHttp404();

            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);
            
            //display "edit" (manage) link
            if (_permissionService.Authorize(StandardPermissionProvider.AccessAdminPanel) && _permissionService.Authorize(StandardPermissionProvider.ManageVendors))
                DisplayEditLink(Url.Action("Edit", "Vendor", new { id = vendor.Id, area = "Admin" }));

            //model
            var model = _catalogModelFactory.PrepareVendorModel(vendor, command);

            return View(model);
        }

        [NopHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult VendorAll()
        {
            //we don't allow viewing of vendors if "vendors" block is hidden
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return RedirectToRoute("HomePage");

            var model = _catalogModelFactory.PrepareVendorAllModels();
            return View(model);
        }

        [ChildActionOnly]
        public virtual ActionResult VendorNavigation()
        {
            if (_vendorSettings.VendorsBlockItemsToDisplay == 0)
                return Content("");

            var model = _catalogModelFactory.PrepareVendorNavigationModel();
            if (!model.Vendors.Any())
                return Content("");
            
            return PartialView(model);
        }

        #endregion

        #region Product tags
        
        [ChildActionOnly]
        public virtual ActionResult PopularProductTags()
        {
            var model = _catalogModelFactory.PreparePopularProductTagsModel();

            if (!model.Tags.Any())
                return Content("");
            
            return PartialView(model);
        }

        [NopHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult ProductsByTag(int productTagId, CatalogPagingFilteringModel command)
        {
            var productTag = _productTagService.GetProductTagById(productTagId);
            if (productTag == null)
                return InvokeHttp404();

            var model = _catalogModelFactory.PrepareProductsByTagModel(productTag, command);
            return View(model);
        }

        [NopHttpsRequirement(SslRequirement.No)]
        public virtual ActionResult ProductTagsAll()
        {
            var model = _catalogModelFactory.PrepareProductTagsAllModel();
            return View(model);
        }

        #endregion

        #region Moveleiros

        public virtual ActionResult FilterCategories(SearchModel model)
        {
            if (model == null)
                model = new SearchModel();

            model.SelectedCategories = _categoryService
                .GetAllCategories(onlyCategory: true)
                .Select(t => new SearchCategoriesModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Selected = model.selcats?.Contains(t.Id) ?? false
                })
                .ToList();

#if DEBUG
            System.Threading.Thread.Sleep(2000);
#endif

            return View(model);
        }

        public virtual ActionResult FilterSubCategories(SearchModel model)
        {
            if (model == null)
                model = new SearchModel();

            model.SelectedSubCategories = _categoryService
                .GetAllCategories(onlySubCategory: true)
                .Select(t => new SearchCategoriesModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Selected = model.selcats?.Contains(t.Id) ?? false
                })
                .ToList();

#if DEBUG
            System.Threading.Thread.Sleep(2000);
#endif

            return View(model);
        }

        public virtual ActionResult FilterBrands(SearchModel model)
        {
            if (model == null)
                model = new SearchModel();

            model.SelectedBrands = _brandService
                .GetAllBrands()
                .Select(t => new SearchBrandModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Selected = model.selBrands?.Contains(t.Id) ?? false
                })
                .ToList();

#if DEBUG
            System.Threading.Thread.Sleep(2000);
#endif

            return View(model);
        }

        #endregion

        #region Searching

        [NopHttpsRequirement(SslRequirement.No)]
        [ValidateInput(false)]
        public virtual ActionResult Search(SearchModel model, CatalogPagingFilteringModel command)
        {
            //'Continue shopping' URL
            _genericAttributeService.SaveAttribute(_workContext.CurrentCustomer,
                SystemCustomerAttributeNames.LastContinueShoppingPage,
                _webHelper.GetThisPageUrl(false),
                _storeContext.CurrentStore.Id);

            if (model == null)
                model = new SearchModel();

            model = _catalogModelFactory.PrepareSearchModel(model, command);

            model.SelectedCategories = _categoryService
                .GetAllCategories(onlyCategory: true)
                .Select(t => new SearchCategoriesModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Selected = model.selcats?.Contains(t.Id) ?? false
                })
                .Take(7)
                .ToList();

            var selectedCategories = model
                .SelectedCategories
                .Where(t => t.Selected)
                .ToList();

            var selCatIds = selectedCategories.Select(s => s.Id);

            model.SelectedSubCategories = _categoryService
                .GetAllCategories(onlySubCategory: true)
                .Where(t => selCatIds.Contains(t.ParentCategoryId))
                .Select(t => new SearchCategoriesModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Selected = model.selcats?.Contains(t.Id) ?? false
                })
                .Take(6)
                .ToList();

            model.SelectedBrands = _brandService
                .GetAllBrands()
                .Select(t => new SearchBrandModel
                {
                    Id = t.Id,
                    Name = t.Name,
                    Selected = model.selBrands?.Contains(t.Id) ?? false
                })
                .Take(5)
                .ToList();

            ViewBag.RenderFilter = true;
            ViewBag.iCheck = true;
            ViewBag.CtaCustomizationColor = _settingService.GetSettingByKey<string>("moveleiros.mov-cta-customization");
            ViewBag.CtaCustomizationColorHover = _settingService.GetSettingByKey<string>("moveleiros.mov-cta-customization.hover");
            ViewBag.IsAjaxRequest = Request.IsAjaxRequest();

            return View("Search", model);
        }

        [ChildActionOnly]
        public virtual ActionResult SearchBox()
        {
            var model = _catalogModelFactory.PrepareSearchBoxModel();
            return PartialView(model);
        }

        [ValidateInput(false)]
        public virtual ActionResult SearchTermAutoComplete(string term)
        {
            if (String.IsNullOrWhiteSpace(term) || term.Length < _catalogSettings.ProductSearchTermMinimumLength)
                return Content("");

            //products
            var productNumber = _catalogSettings.ProductSearchAutoCompleteNumberOfProducts > 0 ?
                _catalogSettings.ProductSearchAutoCompleteNumberOfProducts : 10;

            var products = _productService.SearchProducts(
                storeId: _storeContext.CurrentStore.Id,
                keywords: term,
                languageId: _workContext.WorkingLanguage.Id,
                visibleIndividuallyOnly: true,
                pageSize: productNumber);

            var models =  _productModelFactory.PrepareProductOverviewModels(products, false, _catalogSettings.ShowProductImagesInSearchAutoComplete, _mediaSettings.AutoCompleteSearchThumbPictureSize).ToList();
            var result = (from p in models
                          select new
                          {
                              label = p.Name,
                              producturl = Url.RouteUrl("Product", new { SeName = p.SeName }),
                              productpictureurl = p.DefaultPictureModel.ImageUrl
                          })
                          .ToList();
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        #endregion
    }
}
