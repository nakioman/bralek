using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Treenks.Bralek.Common.Installers
{
    public class ServicesInstallers : IWindsorInstaller
    {
        private const string ServiceNamespace = "Treenks.Bralek.Common.Services";

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Types.FromAssemblyInThisApplication()                
                .InNamespace(ServiceNamespace, true)
                .If(t => t.IsClass && t.Name.EndsWith("Service"))
                .WithServiceDefaultInterfaces()
                .Configure(c => c.LifestyleTransient()));
        }
    }
}