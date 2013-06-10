using System.Threading;
using Castle.Core.Logging;
using Treenks.Bralek.Common.Services.Feeds;

namespace Treenks.Bralek.Worker.Jobs
{
    public class UpdateFeedsJob : IJob
    {
        private readonly IFeedsService _updateFeedsService;
        private const int TenMinutes = 600000;
        private bool _isWorking = true;
        private ILogger _logger = NullLogger.Instance;

        public ILogger Logger
        {
            get { return _logger; }
            set { _logger = value; }
        }

        public UpdateFeedsJob(IFeedsService updateFeedsService)
        {
            _updateFeedsService = updateFeedsService;
        }

        public void Start()
        {
            Logger.Info("Start UpdateFeedsJob");
            while (_isWorking)
            {
                try
                {
                    Logger.Info("Updating feeds");
                    _updateFeedsService.UpdateFeeds();
                    Logger.Info("Finish updating feeds");
                    Thread.Sleep(TenMinutes);
                }
                catch (System.Exception ex)
                {
                    Logger.Error("Error trying to update feeds", ex);
                }
            }
            Logger.Info("Stopped Update feeds job");
        }

        public void Stop()
        {
            Logger.Info("Stoping Update feeds job");
            _isWorking = false;
        }
    }
}