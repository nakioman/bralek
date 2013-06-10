using System.Linq;
using Treenks.Bralek.Common.Model;

namespace Treenks.Bralek.Common.Services.FeedItems
{
    public interface IFeedItemsService
    {
        IQueryable<SubscriptionPost> Search(string query, string userEmail);
    }
}