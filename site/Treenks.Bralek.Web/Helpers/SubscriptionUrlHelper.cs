using System.Web.Mvc;

namespace Treenks.Bralek.Web.Helpers
{
    public static class SubscriptionUrlHelper
    {
        private const string ControllerName = "Subscription";

        public static string SubscriptionGet(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Default", new { action = "Get", controller = ControllerName });
        }

        public static string SubscriptionMarkAsRead(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Default", new { action = "MarkAsRead", controller = ControllerName });
        }

        public static string SubscriptionAdd(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Default", new { action = "Add", controller = ControllerName });
        }

        public static string SubscriptionMenu(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Default", new { action = "Menu", controller = ControllerName });
        }

        public static string SubscriptionMenuItemsUpdated(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Default", new { action = "MenuItemsUpdated", controller = ControllerName });
        }

        public static string SubscriptionMarkItemsAsRead(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Default", new { action = "MarkItemsAsRead", controller = ControllerName });
        }

        public static string SubscriptionBookmark(this UrlHelper urlHelper)
        {
            return urlHelper.RouteUrl("Default", new { action = "Bookmark", controller = ControllerName });
        }
    }
}