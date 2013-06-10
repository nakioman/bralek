using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Treenks.Bralek.Web.Plumbing
{
    public class RouteHandler<THttpHandler> : MvcRouteHandler
    where THttpHandler : MvcHandler
    {

        public RouteHandler(IControllerFactory controllerFactory)
            : base(controllerFactory) { }

        protected override IHttpHandler GetHttpHandler(
                RequestContext requestContext)
        {
            requestContext.HttpContext
                .SetSessionStateBehavior(
                    GetSessionStateBehavior(requestContext));

            return (IHttpHandler)Activator
                .CreateInstance(typeof(THttpHandler), requestContext);
        }

        /// <summary>
        ///   <para>Assign route handler to all routes passed</para>
        /// </summary>
        /// <param name = "routes">Routes</param>
        public static void Assign(RouteCollection routes)
        {
            using (routes.GetReadLock())
            {

                var routeHandler
                    = new RouteHandler<THttpHandler>(
                        ControllerBuilder.Current.GetControllerFactory());

                foreach (var route in routes
                    .OfType<Route>()
                    .Where(r => (r.RouteHandler is MvcRouteHandler)))
                {

                    route.RouteHandler = routeHandler;
                }
            }
        }
    }
}