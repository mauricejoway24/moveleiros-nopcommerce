using System.Web.Mvc;
using System.Web.Routing;
using Nop.Web.Framework.Controllers;

namespace Nop.Web.Actions
{
    public class GlobalKeywordsMappingFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var routeData = filterContext.RouteData;
            var route = (Route)routeData.Route;

            if (route != null && route.Url.StartsWith("perfil/"))
            {
                base.OnActionExecuting(filterContext);
                return;
            }

            // Check keywords
            if (IsKeywordsRoute(route))
            {
                var ctrl = (BaseController)filterContext.Controller;
                filterContext.Result = ctrl.RedirectToAction("Index", "KeywordsMapping", routeData.Values);
            }

            base.OnActionExecuting(filterContext);
        }

        private bool IsKeywordsRoute(Route route)
        {
            var url = route?.Url ?? "";

            if (string.IsNullOrEmpty(url))
                return false;

            var routesPath = url.Split('/');
            var keyword = routesPath[0];

            if (keyword == "teste")
                return true;

            return false;
        }
    }
}