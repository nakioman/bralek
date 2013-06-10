using System.Web.Mvc;

namespace Treenks.Bralek.Web.Helpers
{
    public static class HomeUrlHelper
    {
        private const string ControllerName = "Home";

        public static string HomeIndex(this UrlHelper helper)
        {
            return helper.RouteUrl("Default", new {action = "Index", controller = ControllerName});
        }
    }
}