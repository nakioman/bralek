using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using System.Web.Mvc;

namespace Treenks.Bralek.Web.Installers
{
    /// <summary>
    /// This <see cref="IWindsorInstaller"/> is responsible for registering all ASP.Net MVC controllers in the container.
    /// <para>
    /// A controller is a class which implements <see cref="IController"/> and which name ends with "Controller".
    /// </para>
    /// </summary>
    /// <remarks>
    /// The controllers are registered as Transient.
    /// </remarks>
    public class ControllersInstaller : IWindsorInstaller
    {
        /// <summary>
        /// how to find controllers : (all classes in the current assembly) && (deriving from IController) && (class name ends with 'Controller')
        /// how to configure them   : by default, all controllers are transient
        /// </summary>
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Types.FromThisAssembly()
                                .BasedOn<IController>()
                                .If(t => t.Name.EndsWith("Controller"))
                                .Configure(c => c.LifestyleTransient()));
        }
    }
}