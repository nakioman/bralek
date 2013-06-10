using Castle.MicroKernel.Registration;
using Castle.MicroKernel.SubSystems.Configuration;
using Castle.Windsor;

namespace Treenks.Bralek.Worker.Installers
{
    public class JobsInstaller : IWindsorInstaller
    {
        private const string JobsNamespace = "Treenks.Bralek.Worker.Jobs";

        public void Install(IWindsorContainer container, IConfigurationStore store)
        {
            container.Register(Types.FromThisAssembly()
                .InNamespace(JobsNamespace, true)
                .If(t => t.IsClass)
                .WithServiceDefaultInterfaces()
                .Configure(c => c.LifestyleTransient()));
        }
    }
}