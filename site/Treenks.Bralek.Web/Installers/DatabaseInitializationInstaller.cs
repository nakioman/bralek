using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Treenks.Bralek.Web.Plumbing;

namespace Treenks.Bralek.Web.Installers
{
    public class DatabaseInitializationInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.AddFacility<DatabaseInitializationFacility>();
        }
    }
}