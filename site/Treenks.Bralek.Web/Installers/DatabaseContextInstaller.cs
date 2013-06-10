using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Treenks.Bralek.Common.Model;

namespace Treenks.Bralek.Web.Installers
{
    public class DatabaseContextInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Component.For<BralekDbContext>()
                .DependsOn(Dependency.OnValue<string>("DefaultConnection"))
                .LifestylePerWebRequest());
        }
    }
}