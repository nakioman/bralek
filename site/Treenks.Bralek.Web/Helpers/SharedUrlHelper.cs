using System.Web.Mvc;

namespace Treenks.Bralek.Web.Helpers
{
    public static class SharedUrlHelper
    {
         public static string Image(this UrlHelper helper, string imageFile)
         {
             return helper.Content(string.Format("~/Content/images/{0}", imageFile));
         }        
    }
}