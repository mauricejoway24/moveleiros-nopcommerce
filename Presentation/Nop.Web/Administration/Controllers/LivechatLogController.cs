using Nop.Admin.Models.Livechat;
using Nop.Core;
using Nop.Core.Domain.Customers;
using Nop.Services.Livechat;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Security;
using Nop.Services.Stores;
using Nop.Web.Framework.Kendoui;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Admin.Controllers
{
    public class LivechatLogController : BaseAdminController
    {
        private readonly IPermissionService permissionService;
        private readonly ILivechatService livechatService;
        private readonly IStoreMappingService storeMappingService;
        private readonly ICustomerActivityService customerActivityService;
        private readonly IWorkContext workContext;
        private readonly ILocalizationService localizationService;

        public LivechatLogController(
            IPermissionService permissionService,
            ILivechatService livechatService,
            IStoreMappingService storeMappingService,
            ICustomerActivityService customerActivityService,
            IWorkContext workContext,
            ILocalizationService localizationService)
        {
            this.permissionService = permissionService;
            this.livechatService = livechatService;
            this.storeMappingService = storeMappingService;
            this.customerActivityService = customerActivityService;
            this.workContext = workContext;
            this.localizationService = localizationService;
        }

        public ActionResult Index()
        {
            return RedirectToActionPermanent(nameof(LivechatLogController.List));
        }

        public virtual ActionResult List()
        {
            if (!permissionService.Authorize(StandardPermissionProvider.ManageLivechat))
                return AccessDeniedView();

            // var model = new LivechatLogListModel();

            return View();
        }

        [HttpPost]
        public virtual ActionResult List(DataSourceRequest command, LivechatLogListModel model)
        {
            if (!permissionService.Authorize(StandardPermissionProvider.ManageLivechat))
                return AccessDeniedKendoGridJson();

            var customerId = UserHasAllCustomerPermission() ? 0 : workContext.CurrentCustomer.Id;

            var conversationList = livechatService.GetSummarizedConversationList(
                storeMappingService.CurrentStore(), 
                command.Page, 
                command.PageSize,
                currentUser: customerId
            );

            var gridModel = new DataSourceResult
            {
                Data = conversationList.Select(x => x),
                Total = conversationList.TotalCount
            };

            return Json(gridModel);
        }

        public ActionResult Show(string id)
        {
            if (!permissionService.Authorize(StandardPermissionProvider.ManageLivechat))
                return AccessDeniedView();

            var messages = livechatService.GetChannelMessagesById(id);
            if (messages == null || messages.Count == 0)
                //No log found with the specified id
                return RedirectToAction("List");

            // Check permission


            customerActivityService.InsertActivity(
                workContext.CurrentCustomer, 
                "ShowChat", 
                localizationService.GetResource("Moveleiros.ActivityLog.ShowChat")
            );

            return View(messages);
        }

        private bool UserHasAllCustomerPermission()
        {
            var hasAllCustomerPermission = workContext.CurrentCustomer.IsInCustomerRole(SystemCustomerRoleNames.LivechatAdmin) ||
                workContext.IsSupremeAdmin;

            return hasAllCustomerPermission;
        }
    }
}