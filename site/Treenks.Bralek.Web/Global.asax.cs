using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Treenks.Bralek.Web.App_Start;
using Treenks.Bralek.Web.Plumbing;

namespace Treenks.Bralek.Web
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : HttpApplication, IContainerAccessor
    {
        private static IWindsorContainer _container;

        IWindsorContainer IContainerAccessor.Container
        {
            get { return _container; }
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            //RouteHandler<RedirectHandler>.Assign(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();

            BootstrapContainer();
        }

        /// <summary>
        /// Initialize Windsor container
        /// </summary>
        private static void BootstrapContainer()
        {
            //Instanciate the container, and register all components defined by installers class 
            //(implementing IWindsorInstaller) located in the current assembly
            _container = new WindsorContainer().Install(FromAssembly.InThisApplication());

            //Instanciate our Windsor controllers factory (that overrides MVC standard controllers resolution)
            var controllerFactory = new WindsorControllerFactory(_container.Kernel);
            //Attach our custom controllers factory to MVC main engine
            ControllerBuilder.Current.SetControllerFactory(controllerFactory);

        }

        protected void Application_End()
        {
            _container.Dispose();
        }
    }
}