using Nop.Services.Stores;
using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public class StoreProfileController : BasePublicController
    {
        private readonly IStoreService storeService;

        public StoreProfileController(IStoreService storeService)
        {
            this.storeService = storeService;
        }

        // GET: StoreProfile
        public ActionResult Show(string profileKeyword)
        {
            if (string.IsNullOrEmpty(profileKeyword) || profileKeyword.ToLower() == "none")
                return RedirectToAction("Index", "Home");

            var store = storeService.GetStoreByProfileShorUrl(profileKeyword);

            if (store == null)
                return HttpNotFound();

            return View(store);
        }
    }
}