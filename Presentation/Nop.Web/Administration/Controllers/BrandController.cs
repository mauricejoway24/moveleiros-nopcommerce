using Nop.Admin.Extensions;
using Nop.Admin.Models.Catalog;
using Nop.Core.Domain.Catalog;
using Nop.Services.Catalog;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Media;
using Nop.Services.Security;
using Nop.Services.Seo;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Kendoui;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public class BrandController : BaseAdminController
    {
        private readonly IPermissionService _permissionService;
        private readonly IBrandService _brandService;
        private readonly IPictureService _pictureService;
        private readonly ICustomerActivityService _customerActivityService;
        private readonly ILocalizationService _localizationService;

        public BrandController(
            IPermissionService permissionService,
            IBrandService brandService,
            IPictureService pictureService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService)
        {
            _permissionService = permissionService;
            _brandService = brandService;
            _pictureService = pictureService;
            _customerActivityService = customerActivityService;
            _localizationService = localizationService;
        }

        public virtual ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public virtual ActionResult List()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBrands))
                return AccessDeniedView();

            var model = new BrandListModel();

            return View(model);
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command, BrandListModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBrands))
                return AccessDeniedKendoGridJson();

            var brands = _brandService.GetAllBrands(model.SearchBrandName,
                command.Page - 1, command.PageSize, true);

            var gridModel = new DataSourceResult
            {
                Data = brands.Select(x => x.ToModel()),
                Total = brands.TotalCount
            };

            return Json(gridModel);
        }

        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBrands))
                return AccessDeniedView();

            var nextOrderNumber = _brandService.GetNextOrderDisplay();

            var model = new BrandModel()
            {
                Published = true,
                DisplayOrder = nextOrderNumber
            };

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Create(BrandModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBrands))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return View(model);

            var brand = model.ToEntity();

            brand.CreatedOnUtc = DateTime.UtcNow;
            brand.UpdatedOnUtc = DateTime.UtcNow;

            _brandService.InsertBrand(brand);

            //update picture seo file name
            UpdatePictureSeoNames(brand);

            //activity log
            _customerActivityService.InsertActivity("AddNewBrand", _localizationService.GetResource("ActivityLog.AddNewBrand"), brand.Name);

            SuccessNotification(_localizationService.GetResource("Moveleiros.Admin.Catalog.Brands.Added"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = brand.Id });
            }

            return RedirectToAction("List");
        }

        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBrands))
                return AccessDeniedView();

            var brand = _brandService.GetBrandById(id);
            if (brand == null || brand.Deleted)
                //No manufacturer found with the specified id
                return RedirectToAction("List");

            var model = brand.ToModel();

            return View(model);
        }

        [HttpPost, ParameterBasedOnFormName("save-continue", "continueEditing")]
        public virtual ActionResult Edit(BrandModel model, bool continueEditing)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBrands))
                return AccessDeniedView();

            var brand = _brandService.GetBrandById(model.Id);
            if (brand == null || brand.Deleted)
                //No manufacturer found with the specified id
                return RedirectToAction("List");

            if (!ModelState.IsValid)
                return View(model);

            int prevPictureId = brand.PictureId;
            brand = model.ToEntity(brand);
            brand.UpdatedOnUtc = DateTime.UtcNow;
            _brandService.UpdateBrand(brand);
        
            //delete an old picture (if deleted or updated)
            if (prevPictureId > 0 && prevPictureId != brand.PictureId)
            {
                var prevPicture = _pictureService.GetPictureById(prevPictureId);

                if (prevPicture != null)
                    _pictureService.DeletePicture(prevPicture);
            }

            //update picture seo file name
            UpdatePictureSeoNames(brand);

            //activity log
            _customerActivityService.InsertActivity("EditBrand", _localizationService.GetResource("ActivityLog.EditBrand"), brand.Name);

            SuccessNotification(_localizationService.GetResource("Moveleiros.Admin.Catalog.Brands.Updated"));

            if (continueEditing)
            {
                //selected tab
                SaveSelectedTabName();

                return RedirectToAction("Edit", new { id = brand.Id });
            }
            return RedirectToAction("List");
        }

        [HttpPost]
        public virtual ActionResult Delete(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageBrands))
                return AccessDeniedView();

            var brand = _brandService.GetBrandById(id);
            if (brand == null)
                //No manufacturer found with the specified id
                return RedirectToAction("List");

            _brandService.DeleteBrand(brand);

            //activity log
            _customerActivityService.InsertActivity("DeleteBrand", _localizationService.GetResource("ActivityLog.DeleteBrand"), brand.Name);

            SuccessNotification(_localizationService.GetResource("Moveleiros.Admin.Catalog.Brands.Deleted"));
            return RedirectToAction("List");
        }

        [NonAction]
        protected virtual void UpdatePictureSeoNames(Brand brand)
        {
            var picture = _pictureService.GetPictureById(brand.PictureId);
            if (picture != null)
                _pictureService.SetSeoFilename(picture.Id, _pictureService.GetPictureSeName(brand.Name));
        }
    }
}