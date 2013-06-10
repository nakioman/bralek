using Treenks.Bralek.Common.Model;
namespace Treenks.Bralek.Common.Services.Feeds
{
    public interface IFeedsService
    {
        void UpdateFeeds();
        UserSubscription AddNewFeed(string feedUrl, User user);
    }
}