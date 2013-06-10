using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;
using Microsoft.WindowsAzure;
using Treenks.Bralek.Common.Model;

namespace Treenks.Bralek.Worker.Installers
{
    public class DatabaseContextInstaller : IWindsorInstaller
    {
        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            var connectionString = CloudConfigurationManager.GetSetting("DefaultConnection");
            container.Register(Component.For<BralekDbContext>()
                .DependsOn(Dependency.OnValue<string>(connectionString))
                .LifestyleTransient());
        }
    }
}