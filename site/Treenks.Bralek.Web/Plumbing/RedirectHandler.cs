using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Treenks.Bralek.Web.Plumbing
{
    public class RedirectHandler : MvcHandler
    {
        public RedirectHandler(RequestContext requestContext)
            : base(requestContext) { }

        protected override IAsyncResult BeginProcessRequest(
                HttpContext httpContext,
                AsyncCallback callback, object state)
        {

            if (!httpContext.Request.IsLocal &&
                !httpContext.Request.Url.AbsoluteUri.Contains("://www."))
            {

                httpContext.Response.Status = "301 Moved Permanently";
                httpContext.Response.StatusCode = 301;
                httpContext.Response.AppendHeader(
                    "Location",
                    httpContext.Request.Url.AbsoluteUri
                        .Replace("://", "://www.")
                    );
            }

            return base.BeginProcessRequest(
                httpContext, callback, state);
        }
    }
}