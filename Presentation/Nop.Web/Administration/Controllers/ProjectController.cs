using Nop.Admin.Models.Projects;
using Nop.Core;
using Nop.Core.Caching;
using Nop.Core.Domain.Common;
using Nop.Core.Domain.Projects;
using Nop.Services.Catalog;
using Nop.Services.Configuration;
using Nop.Services.Customers;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Orders;
using Nop.Services.Projects;
using Nop.Services.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public class ProjectController : BaseAdminController
    {
        #region Fields
        private IStoreContext _storeContext;
        private AdminAreaSettings _adminAreaSettings;
        private ISettingService _settingService;
        private IPermissionService _permissionService;
        private ICustomerService _customerService;
        private IReturnRequestService _returnRequestService;
        private IWorkContext _workContext;
        private ICacheManager _cacheManager;
        private readonly IProjectService _projectService;
        private readonly ILocalizationService _localizationService;
        private readonly IPictureService _pictureService;
        private readonly ICategoryService _categoryService;
        private readonly IBrandService _brandService;
        #endregion Fields

        #region Constructors
        public ProjectController(IStoreContext storeContext,
            AdminAreaSettings adminAreaSettings,
            ISettingService settingService,
            IPermissionService permissionService,
            ICustomerService customerService,
            IReturnRequestService returnRequestService,
            IWorkContext workContext,
            ICacheManager cacheManager,
            IProjectService projectService,
            ILocalizationService localizationService,
            IPictureService pictureService,
            ICategoryService categoryService,
            IBrandService brandService)
        {
            this._storeContext = storeContext;
            this._adminAreaSettings = adminAreaSettings;
            this._settingService = settingService;
            this._permissionService = permissionService;
            this._customerService = customerService;
            this._returnRequestService = returnRequestService;
            this._workContext = workContext;
            this._cacheManager = cacheManager;
            this._projectService = projectService;
            this._localizationService = localizationService;
            this._pictureService = pictureService;
            this._categoryService = categoryService;
            this._brandService = brandService;
        }
        #endregion

        #region Actions
        public virtual ActionResult List()
        {
            var projects = _projectService.ListMyProjects(_workContext.CurrentCustomer.Id);
            return View(projects);
        }

        public virtual ActionResult Create()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DesignerCreate))
                return AccessDeniedView();
            GetCategoryList();
            GetBrandList();
            return View(new ProjectModel());
        }

        [HttpPost]
        public virtual ActionResult Create(ProjectModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DesignerCreate))
                return AccessDeniedView();
            if (model.PromobFile != null && Path.GetExtension(model.PromobFile.FileName).ToLower() != ".promob")
                ModelState.AddModelError("PromobFile", "Arquivo Promob está com uma extensão inválida");
            if (model.PromobFile == null)
                ModelState.AddModelError("PromobFile", "Arquivo Promob é obrigatório");
            if (model.Pictures != null)
                foreach (var picture in model.Pictures)
                {
                    if (picture == null)
                        ModelState.AddModelError("Pictures", "As imagens do projeto são obrigatórias");
                    else
                        if (!IsImage(picture))
                            ModelState.AddModelError("Pictures", "Você deve apenas fazer o upload de imagens");
                }
            else
                ModelState.AddModelError("Pictures", "As imagens do projeto são obrigatórias");
            if (model.CategoriesId == null || (!model.CategoriesId?.Any() ?? true))
                ModelState.AddModelError("CategoriesId", "As categorias do projeto são obrigatórias");
            if (ModelState.IsValid)
            {
                model.DesignerId = _workContext.CurrentCustomer.Id;
                var project = model.ToEntity();
                FillCategories(project, model.CategoriesId);

                UploadImages(project, model.Pictures);
                UploadPromob(project, model.PromobFile);
                _projectService.InsertProject(project);
                SuccessNotification(_localizationService.GetResource("Admin.Projects.Added"));
                return RedirectToAction("List");
            }
            GetCategoryList(model.CategoriesId);
            GetBrandList(model.BrandId);
            return View(model);
        }

        public virtual ActionResult Edit(int id)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DesignerCreate))
                return AccessDeniedView();
            var project = _projectService.GetById(id);
            GetCategoryList(project.Categories.Select(a => a.CategoryId).ToList());
            GetBrandList(project.BrandId);
            return View(new ProjectModel(project));
        }

        [HttpPost]
        public virtual ActionResult Edit(ProjectModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.DesignerCreate))
                return AccessDeniedView();

            var projectEntity = _projectService.GetById(model.Id.Value);
            if (model.Pictures != null)
                foreach (var picture in model.Pictures)
                {
                    if (picture == null && model.PicturesId == null)
                        ModelState.AddModelError("Pictures", "As imagens do projeto são obrigatórias");
                    else if (picture != null)
                        if (!IsImage(picture))
                        ModelState.AddModelError("Pictures", "Você deve apenas fazer o upload de imagens");
                }
            else if (model.PicturesId == null)
                ModelState.AddModelError("Pictures", "As imagens do projeto são obrigatórias");
            if (model.CategoriesId == null || (!model.CategoriesId?.Any() ?? true))
                ModelState.AddModelError("CategoriesId", "As categorias do projeto são obrigatórias");
            if (ModelState.IsValid)
            {
                projectEntity.Name = model.Name;
                projectEntity.Description = model.Description;
                model.DesignerId = _workContext.CurrentCustomer.Id;

                foreach (var category in projectEntity.Categories)
                    if (!model.CategoriesId.Any(a => a == category.CategoryId))
                        model.CategoriesId.Remove(category.CategoryId);
                FillCategories(projectEntity, model.CategoriesId);

                var picturesDeleted = projectEntity.Pictures.Where(a => !model.PicturesId?.Any(b => b == a.PictureId) ?? true).ToList();
                _projectService.RemovePictureProject(picturesDeleted);
                if (model.Pictures?.Any() ?? false)
                    UploadImages(projectEntity, model.Pictures);

                if (model.PromobFile != null)
                    UploadPromob(projectEntity, model.PromobFile);
                _projectService.EditProject(projectEntity);
                SuccessNotification(_localizationService.GetResource("Admin.Projects.Edited"));
                return RedirectToAction("List");
            }
            GetCategoryList(model.CategoriesId);
            GetBrandList(model.BrandId);
            model.CategoriesId = projectEntity.Categories.Select(a => a.CategoryId).ToList();
            model.ProjectPictures = projectEntity.Pictures.Select(a => new ProjectPictureModel
            {
                PictureUrl = a.UrlImage,
                PictureId = a.PictureId
            }).ToList();
            return View(model);
        }

        [HttpPost]
        public RedirectToRouteResult Delete()
        {
            var projectEntity = _projectService.GetById(Convert.ToInt32(Request.Form["id"]));
            projectEntity.Active = false;
            _projectService.EditProject(projectEntity);
            SuccessNotification(_localizationService.GetResource("Admin.Projects.Deleted"));
            return RedirectToAction("List");
        }

        public PartialViewResult BestProjectTopics()
        {
            var topics = _projectService.GetBestTopics();
            return PartialView(topics);
        }

        public FileResult DownloadPromob(int id)
        {
            var project = _projectService.GetById(id);
            var path = HostingEnvironment.MapPath("~/App_Data/PromobFiles/" + project.PromobFile);
            var fileStream = new FileStream(path, FileMode.Open);
            var fileName = project.Name + ".promob";

            return File(fileStream, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
        #endregion

        #region Methods
        public void GetCategoryList(List<int> categoriesSelected = null)
        {
            var list = new List<SelectListItem>();
            foreach (var category in _categoryService.GetAllCategories())
            {
                list.Add(new SelectListItem
                {
                    Text = category.Name,
                    Value = category.Id.ToString(),
                    Selected = categoriesSelected?.Any(a => a == category.Id) ?? false
                });
            }
            ViewBag.AvailableCategories = list;
        }

        public void GetBrandList(int? brandSelected = null)
        {
            var list = new List<SelectListItem>();
            foreach (var brand in _brandService.GetAllBrands())
            {
                bool selected = false;
                if (brandSelected.HasValue && brandSelected.Value == brand.Id)
                {
                    selected = true;
                }
                list.Add(new SelectListItem
                {
                    Text = brand.Name,
                    Value = brand.Id.ToString(),
                    Selected = selected
                });
            }
            ViewBag.AvailableBrands = list;
        }

        public void FillCategories(Project project, List<int> categoriesId)
        {
            foreach (var categoryId in categoriesId)
                project.Categories.Add(new ProjectCategory { CategoryId = categoryId });
        }

        public void UploadImages(Project project, HttpPostedFileBase[] pictures)
        {
            foreach (var picture in pictures)
            {
                if (picture != null)
                {
                    Stream stream = picture.InputStream;
                    var pictureName = picture.FileName;
                    var contentType = picture.ContentType;

                    var fileBinary = new byte[stream.Length];
                    stream.Read(fileBinary, 0, fileBinary.Length);

                    var fileExtension = Path.GetExtension(pictureName);
                    if (!string.IsNullOrEmpty(fileExtension))
                        fileExtension = fileExtension.ToLowerInvariant();
                    if (string.IsNullOrEmpty(contentType))
                    {
                        switch (fileExtension)
                        {
                            case ".bmp":
                                contentType = MimeTypes.ImageBmp;
                                break;
                            case ".gif":
                                contentType = MimeTypes.ImageGif;
                                break;
                            case ".jpeg":
                            case ".jpg":
                            case ".jpe":
                            case ".jfif":
                            case ".pjpeg":
                            case ".pjp":
                                contentType = MimeTypes.ImageJpeg;
                                break;
                            case ".png":
                                contentType = MimeTypes.ImagePng;
                                break;
                            case ".tiff":
                            case ".tif":
                                contentType = MimeTypes.ImageTiff;
                                break;
                            default:
                                break;
                        }
                    }
                    var pictureProject = _pictureService.InsertPicture(fileBinary, contentType, null);
                    project.Pictures.Add(new ProjectPicture { PictureId = pictureProject.Id });
                }
            }
        }

        public void UploadPromob(Project project, HttpPostedFileBase promobFile)
        {
            var environment = HostingEnvironment.MapPath("~/App_Data/PromobFiles");
            var extension = Path.GetExtension(promobFile.FileName);
            var promobFileName = Guid.NewGuid().ToString() + extension;
            var path = Path.Combine(environment, promobFileName);
            promobFile.SaveAs(path);
            project.PromobFile = promobFileName;
        }

        private bool IsImage(HttpPostedFileBase file)
        {
            if (file.ContentType.Contains("image"))
                return true;
            string[] formats = new string[] { ".jpg", ".png", ".gif", ".jpeg", ".bmp" };
            return formats.Any(item => file.FileName.EndsWith(item, StringComparison.OrdinalIgnoreCase));
        }
        #endregion
    }
}
