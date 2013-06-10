using System;

namespace Treenks.Bralek.Web.ViewModels.Subscription
{
    public class SubscriptionItemViewModel
    {
        public int Id { get; set; }

        public string Title { get; set; }

        public string Content { get; set; }

        public DateTime? PublishDateUTC { get; set; }

        public string FeedName { get; set; }

        public int FeedId { get; set; }

        public string Author { get; set; }

        public string OriginalUrl { get; set; }

        public bool AlreadyRead { get; set; }

        public bool InBookmarks { get; set; }
    }
}