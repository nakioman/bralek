using System.Web.Optimization;

namespace Treenks.Bralek.Web.App_Start
{
    public class BundleConfig
    {
        // For more information on Bundling, visit http://go.microsoft.com/fwlink/?LinkId=254725
        public static void RegisterBundles(BundleCollection bundles)
        {
            bundles.Add(new ScriptBundle("~/bundles/jquery").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/bootstrap.js"
                        ));

            bundles.Add(new ScriptBundle("~/bundles/accountscripts").Include(
                "~/Scripts/jquery-{version}.js",
                "~/Scripts/bootstrap.js",
                "~/Scripts/jquery.unobtrusive*",
                "~/Scripts/jquery.validate*",
                "~/Scripts/jquery.validate.inline.js",
                "~/Scripts/Shared/common.js"
                ));


            bundles.Add(new ScriptBundle("~/bundles/sitescripts").Include(
                        "~/Scripts/jquery-{version}.js",
                        "~/Scripts/knockout-{version}.js",
                        "~/Scripts/bootstrap.js",
                        "~/Scripts/bootstrap-fileupload.js",
                        "~/Scripts/jquery.unobtrusive*",
                        "~/Scripts/jquery.validate*",
                        "~/Scripts/jquery.validate.inline.js",
                        "~/Scripts/jquery.viewport.js",
                        "~/Scripts/jquery.unveil.js",
                        "~/Scripts/Shared/home.js",
                        "~/Scripts/Shared/view.js",
                        "~/Scripts/Shared/common.js"
                        ));

            bundles.Add(new StyleBundle("~/Content/landing").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-responsive.css",
                "~/Content/common.css",
                "~/Content/common-responsive.css",
                "~/Content/landing.css",
                "~/Content/font-awesome.css"
                ));

            bundles.Add(new StyleBundle("~/Content/account").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-responsive.css",
                "~/Content/common.css",
                "~/Content/common-responsive.css",
                "~/Content/account.css",
                "~/Content/account-responsive.css",
                "~/Content/font-awesome.css",
                "~/Content/validationEngine.jquery.css"
                ));

            bundles.Add(new StyleBundle("~/Content/site").Include(
                "~/Content/bootstrap.css",
                "~/Content/bootstrap-fileupload.css",
                "~/Content/bootstrap-responsive.css",
                "~/Content/common.css",
                "~/Content/common-responsive.css",
                "~/Content/font-awesome.css",
                "~/Content/validationEngine.jquery.css"
                ));
        }
    }
}