using System;
using System.Linq;
using System.Net;
using System.Xml;
using Argotic.Common;
using Argotic.Extensions.Core;
using Argotic.Syndication;
using Treenks.Bralek.Common.Model;
using Treenks.Bralek.Common.Exceptions;

namespace Treenks.Bralek.Common.Services.Feeds
{
    public class FeedsService : IFeedsService
    {
        private readonly BralekDbContext _dbContext;

        public FeedsService(BralekDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public void UpdateFeeds()
        {
            var subscriptions = _dbContext.Subscriptions;

            foreach (var subscription in subscriptions)
            {
                try
                {
                    var feed = GenericSyndicationFeed.Create(new Uri(subscription.FeedUrl));
                    var itemsAdded = 0;
                    switch (feed.Format)
                    {
                        case SyndicationContentFormat.Atom:
                            itemsAdded = GetAtomFeedItems((AtomFeed)feed.Resource, subscription);
                            break;
                        case SyndicationContentFormat.Rss:
                            itemsAdded = GetRssFeedItems((RssFeed)feed.Resource, subscription);
                            break;
                        case SyndicationContentFormat.None:
                            subscription.IsWorking = false;
                            subscription.LastError = "Feed url is not a recognized format";
                            break;
                        default:
                            throw new Exception(String.Format("Format {0}, not supported for feed items", feed.Format));
                    }
                    foreach (var userSubscription in subscription.UserSubscriptions)
                    {
                        userSubscription.UnreadItems += itemsAdded;
                    }
                    subscription.IsWorking = true;
                    subscription.LastFeedUpdatesUTC = DateTime.UtcNow;
                }
                catch (WebException webException)
                {
                    subscription.IsWorking = false;
                    subscription.LastError = String.Format("WebException {0}", webException.Message);
                }
                catch (XmlException xmlException)
                {
                    subscription.IsWorking = false;
                    subscription.LastError = String.Format("XmlException {0}", xmlException.Message);
                }
            }

            _dbContext.SaveChanges();
        }

        public UserSubscription AddNewFeed(string feedUrl, User user)
        {
            var feedHashCode = feedUrl.GetHashCode();
            var subscription = _dbContext.Subscriptions.SingleOrDefault(x => x.Hash == feedHashCode);
            if (subscription == null)
            {
                subscription = AddSubscription(feedUrl, user);
            }
            else
            {
                var alreadySubscribed = subscription.UserSubscriptions.Any(x => x.User.Email == user.Email);
                if (alreadySubscribed)
                {
                    throw new BralekTechnicalException("ALREADY_SUBSCRIBED", null);
                }
            }
            var userSubscription = new UserSubscription
            {
                AddedOnUTC = DateTime.UtcNow,
                Subscription = subscription,
                UpdatedOnUTC = DateTime.UtcNow,
                User = user,
                UnreadItems = subscription.SubscriptionPosts.Count()
            };
            _dbContext.UserSubscriptions.Add(userSubscription);
            _dbContext.SaveChanges();

            return userSubscription;
        }

        private static DateTime? GetGoodDateTime(DateTime date)
        {
            return date > new DateTime(1980, 1, 1) ? date : (DateTime?)null;
        }

        private Subscription AddSubscription(string feedUrl, User user)
        {
            try
            {
                var uri = new Uri(feedUrl);
                var feed = GenericSyndicationFeed.Create(uri,
                    new SyndicationResourceLoadSettings { AutoDetectExtensions = true, Timeout = new TimeSpan(0, 0, 3) });
                var feedHashCode = feedUrl.GetHashCode();
                var subscription = new Subscription
                {
                    AddedOnUTC = DateTime.UtcNow,
                    AddedBy = user,
                    FeedUrl = feedUrl,
                    Hash = feedHashCode,
                    Title = feed.Title,
                };

                switch (feed.Format)
                {
                    case SyndicationContentFormat.Atom:
                        var atom = (AtomFeed)feed.Resource;
                        subscription.Title = GetAtomSubscriptionTitle(atom);
                        GetAtomFeedItems(atom, subscription);
                        break;
                    case SyndicationContentFormat.Rss:
                        var rss = (RssFeed)feed.Resource;
                        subscription.SiteUrl = rss.Channel.Link.ToString();
                        GetRssFeedItems(rss, subscription);
                        break;
                    case SyndicationContentFormat.None:
                        throw new BralekTechnicalException("NO_RSS_FORMAT", null);
                    default:
                        throw new BralekTechnicalException("RSS_UNKNOWN_FORMAT", null);
                }

                _dbContext.Subscriptions.Add(subscription);
                return subscription;
            }
            catch (UriFormatException ex)
            {
                throw new BralekTechnicalException("ADD_FEED_URI_ERROR", ex);
            }
            catch (WebException ex)
            {
                throw new BralekTechnicalException("FEED_READ_ERROR", ex);
            }
            catch (XmlException ex)
            {
                throw new BralekTechnicalException("FEED_XML_ERROR", ex);
            }
        }

        private static string GetAtomSubscriptionTitle(AtomFeed atom)
        {
            if (atom.BaseUri == null)
            {
                var link = atom.Links.SingleOrDefault(x => x.Relation == String.Empty);
                if (link != null)
                {
                    return link.Uri.ToString();
                }
                var alternateUrl = atom.Links.SingleOrDefault(x => x.Relation == "alternate");
                if (alternateUrl != null)
                {
                    return alternateUrl.Uri.ToString();
                }
            }
            return atom.BaseUri.ToString();
        }

        private int GetRssFeedItems(RssFeed feed, Subscription subscription)
        {
            var itemsAdded = 0;
            foreach (var rssItem in feed.Channel.Items.Where(x => subscription.LastFeedUpdatesUTC == null
                || x.PublicationDate.ToUniversalTime() > subscription.LastFeedUpdatesUTC))
            {
                var categories = rssItem.Categories.Aggregate(String.Empty, (current, category) => current + (category.Value + ",")).TrimEnd(',');
                var subscriptionPost = new SubscriptionPost
                                           {
                                               Categories = categories,
                                               OriginalUrl = rssItem.Link.ToString(),
                                               PublishDateUTC = GetGoodDateTime(rssItem.PublicationDate.ToUniversalTime()),
                                               Subscription = subscription,
                                               Title = rssItem.Title,
                                               Content = GetRssContent(rssItem),
                                               Authors = GetRssAuthors(rssItem)
                                           };
                itemsAdded++;
                _dbContext.SubscriptionPosts.Add(subscriptionPost);
                subscription.SubscriptionPosts.Add(subscriptionPost);
            }
            return itemsAdded;
        }

        private int GetAtomFeedItems(AtomFeed feed, Subscription subscription)
        {
            var itemsAdded = 0;
            foreach (var entry in feed.Entries.Where(x => subscription.LastFeedUpdatesUTC == null
                || x.PublishedOn.ToUniversalTime() > subscription.LastFeedUpdatesUTC))
            {
                var categories = entry.Categories.Aggregate(String.Empty, (current, category) => current + (category.Label + ",")).TrimEnd(',');
                var authors = entry.Authors.Aggregate(String.Empty, (current, author) => current + (author.Name + ",")).TrimEnd(',');
                var subscriptionPost = new SubscriptionPost
                                           {
                                               Authors = authors,
                                               Categories = categories,
                                               Content = entry.Content != null ? entry.Content.Content : (entry.Summary != null) ? entry.Summary.Content : String.Empty,
                                               OriginalUrl = GetOriginalUrl(entry),
                                               PublishDateUTC = GetGoodDateTime(entry.PublishedOn.ToUniversalTime()),
                                               Subscription = subscription,
                                               Title = entry.Title.Content
                                           };
                itemsAdded++;
                _dbContext.SubscriptionPosts.Add(subscriptionPost);
                subscription.SubscriptionPosts.Add(subscriptionPost);
            }
            return itemsAdded;
        }

        private static string GetOriginalUrl(AtomEntry entry)
        {
            var originalUrl = entry.Links.SingleOrDefault(x => String.IsNullOrWhiteSpace(x.Relation));
            var alternateUrl = entry.Links.SingleOrDefault(x => x.Relation == "alternate");

            var url = originalUrl ?? alternateUrl;

            return url.Uri.ToString();
        }

        private string GetRssAuthors(RssItem item)
        {
            var dcExtension = item.FindExtension<DublinCoreElementSetSyndicationExtension>();
            return dcExtension != null ? dcExtension.Context.Creator : String.Empty;
        }

        private static string GetRssContent(RssItem item)
        {
            var contentExtension = item.FindExtension<SiteSummaryContentSyndicationExtension>();
            return contentExtension != null ? contentExtension.Context.Encoded : item.Description;
        }
    }
}