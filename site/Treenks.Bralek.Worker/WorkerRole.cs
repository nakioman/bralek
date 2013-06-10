using System;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Castle.Windsor;
using Castle.Windsor.Installer;
using Microsoft.WindowsAzure.ServiceRuntime;
using Treenks.Bralek.Worker.Jobs;

namespace Treenks.Bralek.Worker
{
    public class WorkerRole : RoleEntryPoint
    {
        private static IWindsorContainer _container;
        private Task _jobsTask;
        private IEnumerable<IJob> _jobs;
        private ILogger _logger = NullLogger.Instance;

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public override bool OnStart()
        {
            Logger.Info("Worker Starting");

            // Set the maximum number of concurrent connections 
            ServicePointManager.DefaultConnectionLimit = 1;

            try
            {
                BootstrapContainer();
                StartJobs();
                Logger.Info("Worker Started");
                return true;
            }
            catch (Exception ex)
            {
                Logger.Error("Error starting worker", ex);
            }

            return false;
        }

        private void StartJobs()
        {
            _jobs = _container.ResolveAll<IJob>();
            var taskFactory = new TaskFactory(TaskCreationOptions.LongRunning, TaskContinuationOptions.LongRunning);
            _jobsTask = taskFactory.StartNew(() => Parallel.ForEach(_jobs, job => job.Start()));
        }

        private void BootstrapContainer()
        {
            Logger.Info("Entering Bootstrap container");
            _container = new WindsorContainer().Install(FromAssembly.InThisApplication());
            Logger.Info("Exiting Bootstrap container");
        }

        public override void OnStop()
        {
            try
            {
                StopJobs();
                _container.Dispose();
            }
            catch (Exception ex)
            {
                Logger.Error("Error stoping worker", ex);
            }
        }

        private void StopJobs()
        {
            Parallel.ForEach(_jobs, job => job.Stop());
            _jobsTask.Dispose();
        }
    }
}
