using System.Web;
using System.Web.Mvc;
using Treenks.Bralek.Web.Controllers.ActionFilters;

namespace Treenks.Bralek.Web
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
            filters.Add(new LogErrorAttribute());
        }
    }
}