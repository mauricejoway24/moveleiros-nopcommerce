using Nop.Core.Domain.Seo;
using Nop.Core.Infrastructure;
using Nop.Services.Catalog;
using System.Web.Mvc;

namespace Nop.Web.Controllers
{
    public class KeywordsMappingController : BasePublicController
    {
        // GET: KeywordsMapping
        public ActionResult KeywordHandle(KeywordsMapping keyword)
        {
            return RedirectToAction("Search", "Catalog");
        }

        public ActionResult CityHandle(string city)
        {
            var cityService = EngineContext.Current.Resolve<ICityService>();
            var cityObj = cityService.GetCityByUrl(city.ToLower());

            if (cityObj != null)
            {
                var keywordWithCity = new KeywordsMapping
                {
                    CityId = cityObj.Id
                };

                return KeywordHandle(keywordWithCity);
            }

            return RedirectToAction("Search", "Catalog");
        }
    }
}