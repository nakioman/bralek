using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using Treenks.Bralek.Common.Model;
using Treenks.Bralek.Common.Services.ImportFeeds.OPML;

namespace Treenks.Bralek.Common.Services.ImportFeeds
{
    public class ImportFeedService : IImportFeedService
    {
        private readonly BralekDbContext _dbContext;

        public ImportFeedService(BralekDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void ImportOpml(Stream opmlFile, string userEmail)
        {
            var user = _dbContext.Users.Single(x => x.Email == userEmail);
            using (var textReader = new XmlTextReader(opmlFile))
            {
                const string rss = "rss";
                var xs = new XmlSerializer(typeof(Opml));
                var opmlData = (Opml)xs.Deserialize(textReader);

                foreach (var outline in opmlData.Body.Outline.Where(x => x.Type == rss))
                {
                    var xmlUrlHash = outline.XmlUrl.GetHashCode();
                    var subscription = _dbContext.Subscriptions.SingleOrDefault(x => x.Hash == xmlUrlHash);

                    if (subscription == null)
                    {
                        subscription = new Subscription
                                           {
                                               FeedUrl = outline.XmlUrl,
                                               Hash = xmlUrlHash,
                                               SiteUrl = outline.HtmlUrl,
                                               AddedBy = user,
                                               AddedOnUTC = DateTime.UtcNow,
                                               Title = outline.Title,
                                           };
                        _dbContext.Subscriptions.Add(subscription);
                    }

                    var userSubscription = new UserSubscription
                                               {
                                                   Subscription = subscription,                                                   
                                                   User = user,
                                                   AddedOnUTC = DateTime.UtcNow,
                                                   UpdatedOnUTC = DateTime.UtcNow
                                               };
                    _dbContext.UserSubscriptions.Add(userSubscription);
                }
                _dbContext.SaveChanges();
            }
        }
    }
}