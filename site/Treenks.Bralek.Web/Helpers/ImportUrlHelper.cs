using System.Web.Mvc;

namespace Treenks.Bralek.Web.Helpers
{
    public static class ImportUrlHelper
    {
        private const string ControllerName = "Import";

        public static string ImportGoogleReader(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Default", new {action = "GoogleReader", controller = ControllerName});
        }
    }
}