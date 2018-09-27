using Nop.Core;
using Nop.Services.Localization;
using Nop.Services.Logging;
using Nop.Services.Messages;
using Nop.Services.Stores;
using Nop.Web.Framework;
using Nop.Web.Framework.Security;
using Nop.Web.Models.Chat;
using Nop.Web.Models.Common;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public class CustomerChatController : BasePublicController
    {
        private readonly IStoreService storeService;
        private readonly IStoreMappingService storeMappingService;
        private readonly ICustomerActivityService customerActivityService;
        private readonly ILocalizationService localizationService;
        private readonly IWorkflowMessageService workflowMessageService;
        private readonly IWorkContext workContext;

        public CustomerChatController(
            IStoreService storeService,
            IStoreMappingService storeMappingService,
            ICustomerActivityService customerActivityService,
            ILocalizationService localizationService,
            IWorkflowMessageService workflowMessageService,
            IWorkContext workContext)
        {
            this.storeService = storeService;
            this.storeMappingService = storeMappingService;
            this.customerActivityService = customerActivityService;
            this.localizationService = localizationService;
            this.workflowMessageService = workflowMessageService;
            this.workContext = workContext;
        }

        [PublicStoreAllowNavigation(true)]
        public virtual ActionResult Chat(int productId)
        {
            var storeId = storeMappingService
                .GetStoreIdByEntityId(productId, "Product")
                .FirstOrDefault();

            if (storeId == 0)
            {
                customerActivityService.InsertActivity("GetCustomerChat",
                    localizationService.GetResource("Moveleiros.ActivityLog.ChatNotFound"), productId);

                return RedirectToAction("ChatContact", new { productId = productId });
            }

            var chatSnippet = storeService.GetChatByStoreId(storeId);

            if (string.IsNullOrEmpty(chatSnippet))
            {
                customerActivityService.InsertActivity("GetCustomerChat",
                    localizationService.GetResource("Moveleiros.ActivityLog.ChatNotFound"), productId);

                return RedirectToAction("ChatContact", new { productId = productId });
            }

            customerActivityService.InsertActivity("GetCustomerChat",
                localizationService.GetResource("Moveleiros.ActivityLog.GetCustomerChat"), productId);

#if DEBUG
            System.Threading.Thread.Sleep(2000);
#endif

            return View("Chat", model: chatSnippet);
        }

        [HttpGet]
        public virtual ActionResult ChatContact(int productId)
        {
            return View(new ChatContactUsModel
            {
                ProductId = productId,
                Subject = localizationService.GetResource("Moveleiros.EmailMessage.CustomizaAction")
            });
        }

        [HttpPost]
        [PublicAntiForgery]
        public virtual ActionResult ChatContact(ChatContactUsModel contactModel)
        {
            if (!ModelState.IsValid)
                return View(contactModel);

            string subject = contactModel.Subject;
            string body = Core.Html.HtmlHelper.FormatText(
                contactModel.Question + Environment.NewLine + 
                    Environment.NewLine + "Phone: " + contactModel.PhoneNumber, 
                false, 
                true, 
                false, 
                false, 
                false, 
                false
            );

            workflowMessageService.SendContactUsMessage(
                workContext.WorkingLanguage.Id,
                contactModel.Email.Trim(), 
                contactModel.FullName, 
                subject, 
                body
            );

            contactModel.SuccessfullySent = true;
            contactModel.Result = localizationService.GetResource("Moveleiros.ContactUs.ContactSent");

            //activity log
            customerActivityService.InsertActivity("PublicStore.ContactUs", localizationService.GetResource("ActivityLog.PublicStore.ContactUs"));

            return View(contactModel);
        }
    }
}