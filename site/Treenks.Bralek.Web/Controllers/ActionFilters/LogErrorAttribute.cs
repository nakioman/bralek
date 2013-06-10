using System.Web.Mvc;
using log4net;

namespace Treenks.Bralek.Web.Controllers.ActionFilters
{
    public class LogErrorAttribute : HandleErrorAttribute
    {
        private readonly ILog _logger;

        public LogErrorAttribute()
        {
            _logger = LogManager.GetLogger(GetType());
        }

        public override void OnException(ExceptionContext filterContext)
        {
            var controller = (string)filterContext.RouteData.Values["controller"];
            var controllerAction = (string)filterContext.RouteData.Values["action"];

            _logger.ErrorFormat("Exception in controller: {0}, action: {1}. The exception is: {2}",
                controller, controllerAction, filterContext.Exception);

            base.OnException(filterContext);
        }
    }
}