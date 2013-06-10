using System.Web.Mvc;

namespace Treenks.Bralek.Web.Helpers
{
    public static class AccountUrlHelper
    {
        private const string ControllerName = "Account";

        public static string AccountRegister(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Default", new {action = "Register", controller = ControllerName});
        }

        public static string AccountLogin(this UrlHelper urlHelper, string returnUrl = null)
        {
            return urlHelper.RouteUrl("Default", new { action = "Login", controller = ControllerName, returnUrl = returnUrl });
        }

        public static string AccountRecoverPassword(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Default", new { action = "RecoverPassword", controller = ControllerName});
        }

        public static string AccountChangePassword(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Default", new { action = "ChangePassword", controller = ControllerName });
        }
    }
}