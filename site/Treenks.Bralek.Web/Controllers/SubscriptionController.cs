using System.Data.Objects;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.SessionState;
using Treenks.Bralek.Common.Exceptions;
using Treenks.Bralek.Common.Model;
using Treenks.Bralek.Common.Services.FeedItems;
using Treenks.Bralek.Common.Services.Feeds;
using Treenks.Bralek.Web.ViewModels.Shared;
using Treenks.Bralek.Web.ViewModels.Subscription;
using WebMatrix.WebData;

namespace Treenks.Bralek.Web.Controllers
{
    [Authorize]
    [SessionState(SessionStateBehavior.ReadOnly)]
    public class SubscriptionController : Controller
    {
        private readonly BralekDbContext _dbContext;
        private readonly IFeedsService _feedsService;
        private readonly IFeedItemsService _feedItemsService;
        private const int PageSize = 10;

        public SubscriptionController(BralekDbContext dbContext, IFeedsService feedsService,
            IFeedItemsService feedItemsService)
        {
            _dbContext = dbContext;
            _feedsService = feedsService;
            _feedItemsService = feedItemsService;
        }

        [HttpGet]
        public ActionResult Get(int page = 0, int? subscriptionId = null, bool? orderByOldest = null,
            bool? showAllPosts = null)
        {            
            var startFrom = page * PageSize;
            var user = _dbContext.Users.FirstOrDefault(x => x.Email == WebSecurity.CurrentUserName);
            var owningSubscriptionPosts = _dbContext.SubscriptionPosts.Where(
                x => x.Subscription.UserSubscriptions.Any(y => y.User.Email == WebSecurity.CurrentUserName));

            if (subscriptionId != null && subscriptionId != -1)
            {
                owningSubscriptionPosts = owningSubscriptionPosts.Where(
                    x => x.Subscription.Id == subscriptionId);
            }
            else if (subscriptionId == -1)
            {
                owningSubscriptionPosts =
                    owningSubscriptionPosts.Where(
                        x => x.Subscription.UserSubscriptions.Any(
                            y => y.User.Email == WebSecurity.CurrentUserName &&
                            y.User.Bookmarks.Any(z => z.User.Email == y.User.Email && z.SubscriptionPost.Id == x.Id)));
            }

            if (showAllPosts.HasValue)
            {
                user.ShowAllItems = showAllPosts.Value;
                _dbContext.SaveChanges();
            }
            showAllPosts = showAllPosts ?? user.ShowAllItems;
            showAllPosts = showAllPosts.Value || subscriptionId == -1;
            if (!showAllPosts.Value)
            {
                startFrom = 0;
                owningSubscriptionPosts = owningSubscriptionPosts.Where(
                        x => !x.PostsRead.Any(y => y.UserSubscription.User.Email == WebSecurity.CurrentUserName
                            && y.SubscriptionPost.Id == x.Id));
            }

            if (orderByOldest.HasValue)
            {
                user.OrderByOldest = orderByOldest.Value;
                _dbContext.SaveChanges();
            }
            orderByOldest = orderByOldest ?? user.OrderByOldest;
            owningSubscriptionPosts = orderByOldest.Value
                ? owningSubscriptionPosts.OrderBy(x => x.PublishDateUTC)
                : owningSubscriptionPosts.OrderByDescending(x => x.PublishDateUTC);

            var model = owningSubscriptionPosts.Skip(startFrom).Take(PageSize)
                .Select(x => new SubscriptionItemViewModel
                                {
                                    Author = x.Authors,
                                    Content = x.Content,
                                    OriginalUrl = x.OriginalUrl,
                                    PublishDateUTC = x.PublishDateUTC,
                                    FeedId = x.Subscription.Id,
                                    Title = x.Title,
                                    Id = x.Id,
                                    FeedName = x.Subscription.Title,
                                    AlreadyRead = x.PostsRead.Any(y => y.UserSubscription.User.Email == WebSecurity.CurrentUserName),
                                    InBookmarks = x.Subscription.UserSubscriptions.Any(y => y.User.Email == WebSecurity.CurrentUserName
                                    && y.User.Bookmarks.Any(z => z.User.Email == y.User.Email && z.SubscriptionPost.Id == x.Id))
                                });

            return PartialView("_SubscriptionItems", model);
        }

        [HttpGet]
        public ActionResult Menu()
        {            
            var user = _dbContext.Users.Single(x => x.Email == WebSecurity.CurrentUserName);
            var subscriptions = user.Subscriptions.OrderBy(x => x.Subscription.Title)
                .Select(x => new UserSubscriptionViewModel
                {
                    Title = x.Subscription.Title,
                    SiteUrl = x.Subscription.SiteUrl,
                    Id = x.Subscription.Id,
                    UnreadItems = x.UnreadItems                    
                });

            return Json(JsonConvert.SerializeObject(subscriptions), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult MenuItemsUpdated()
        {
            var user = _dbContext.Users.Single(x => x.Email == WebSecurity.CurrentUserName);
            var subscriptions = user.Subscriptions
                                    .Where(x => x.Subscription.SubscriptionPosts
                                                    .Any(y => x.Subscription.LastFeedUpdatesUTC > DateTime.UtcNow.AddMinutes(-10)
                                                        && !y.PostsRead.Any(z => z.UserSubscription.User.Email == WebSecurity.CurrentUserName
                                                        && z.SubscriptionPost.Id == y.Id)))
                                    .OrderBy(x => x.Subscription.Title)
            .Select(x => new UserSubscriptionViewModel
                {
                    Title = x.Subscription.Title,
                    SiteUrl = x.Subscription.SiteUrl,
                    Id = x.Subscription.Id,
                    UnreadItems = x.UnreadItems
                });

            return Json(JsonConvert.SerializeObject(subscriptions), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkAsRead(int id)
        {
            var alreadyMarked = _dbContext.PostsRead.Any(x => x.SubscriptionPost.Id == id
                && x.UserSubscription.User.Email == WebSecurity.CurrentUserName);
            if (alreadyMarked)
            {
                return Content("ALREADY_MARKED");
            }
            var subscriptionPost = _dbContext.SubscriptionPosts.FirstOrDefault(x => x.Id == id);
            var userSubscription = subscriptionPost.Subscription.UserSubscriptions
                                                   .FirstOrDefault(x => x.User.Email == WebSecurity.CurrentUserName);
            userSubscription.UnreadItems--;
            var postRead = new PostRead
                               {
                                   ReadOnUTC = DateTime.UtcNow,
                                   SubscriptionPost = subscriptionPost,
                                   UserSubscription = userSubscription,
                               };
            _dbContext.PostsRead.Add(postRead);
            _dbContext.SaveChanges();

            return Content("MARKED");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult MarkItemsAsRead(string timeFrame)
        {            
            IQueryable<SubscriptionPost> subscriptionPostsUnread = _dbContext.SubscriptionPosts
                        .Where(x => x.Subscription.UserSubscriptions
                            .Any(y => y.User.Email == WebSecurity.CurrentUserName)
                            && !x.PostsRead.Any(y => y.UserSubscription.User.Email == WebSecurity.CurrentUserName
                            && y.SubscriptionPost.Id == x.Id));
            switch (timeFrame)
            {
                case "DAY":
                    subscriptionPostsUnread =
                        subscriptionPostsUnread
                        .Where(x => x.PublishDateUTC <= EntityFunctions.AddDays(DateTime.UtcNow, -1));
                    break;
                case "WEEK":
                    subscriptionPostsUnread =
                        subscriptionPostsUnread
                        .Where(x => x.PublishDateUTC <= EntityFunctions.AddDays(DateTime.UtcNow, -7));
                    break;
                case "MONTH":
                    subscriptionPostsUnread =
                        subscriptionPostsUnread
                        .Where(x => x.PublishDateUTC <= EntityFunctions.AddMonths(DateTime.UtcNow, -1));
                    break;
            }

            foreach (var subscriptionPost in subscriptionPostsUnread)
            {
                var userSubscription = subscriptionPost.Subscription.UserSubscriptions
                                                       .Single(x => x.User.Email == WebSecurity.CurrentUserName);
                userSubscription.UnreadItems--;
                var postRead = new PostRead
                                   {
                                       ReadOnUTC = DateTime.UtcNow,
                                       SubscriptionPost = subscriptionPost,
                                       UserSubscription = userSubscription
                                   };
                _dbContext.PostsRead.Add(postRead);
            }
            _dbContext.SaveChanges();
            return Content("MARKED");
        }

        [HttpGet]
        public ActionResult Add()
        {
            return PartialView("_Add");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(AddSubscriptionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = _dbContext.Users.Single(x => x.Email == WebSecurity.CurrentUserName);
                try
                {
                    var userSubscription = _feedsService.AddNewFeed(model.FeedUrl.Trim(), user);
                    var successModel = new UserSubscriptionViewModel
                    {
                        Id = userSubscription.Subscription.Id,
                        SiteUrl = userSubscription.Subscription.SiteUrl,
                        Title = userSubscription.Subscription.Title,
                        UnreadItems = userSubscription.Subscription.SubscriptionPosts.Count(y => y.PostsRead == null
                            || !y.PostsRead
                        .Any(z => z.UserSubscription.User.Email == WebSecurity.CurrentUserName
                        && z.SubscriptionPost.Id == y.Id))
                    };
                    var json = JsonConvert.SerializeObject(successModel);
                    return PartialView("_AddSuccess", json);
                }
                catch (BralekTechnicalException ex)
                {
                    ModelState.AddModelError(String.Empty, ex.Message);
                }
            }
            return PartialView("_Add", model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Bookmark(int id, bool add)
        {
            if (add)
            {
                var subscriptionPost = _dbContext.SubscriptionPosts.FirstOrDefault(x => x.Id == id);
                var user = _dbContext.Users.FirstOrDefault(x => x.Email == WebSecurity.CurrentUserName);
                var bookmark = new Bookmark
                                   {
                                       AddedOnUTC = DateTime.UtcNow,
                                       SubscriptionPost = subscriptionPost,
                                       User = user
                                   };
                _dbContext.Bookmarks.Add(bookmark);
                _dbContext.SaveChanges();
                return Content("BOOKMARKED");
            }
            var bookmarkToDelete = _dbContext.Bookmarks.FirstOrDefault(x => x.User.Email == WebSecurity.CurrentUserName
                                                                            && x.SubscriptionPost.Id == id);
            _dbContext.Bookmarks.Remove(bookmarkToDelete);
            _dbContext.SaveChanges();
            return Content("UNBOOKMARKED");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Search(string query, int fromPage, bool? orderByOldest = null)
        {
            var startFrom = fromPage * PageSize;
            var user = _dbContext.Users.FirstOrDefault(x => x.Email == WebSecurity.CurrentUserName);
            var subscriptionPosts = _feedItemsService.Search(query, WebSecurity.CurrentUserName);
            if (orderByOldest.HasValue)
            {
                user.OrderByOldest = orderByOldest.Value;
                _dbContext.SaveChanges();
            }
            orderByOldest = orderByOldest ?? user.OrderByOldest;
            subscriptionPosts = orderByOldest.Value
                ? subscriptionPosts.OrderBy(x => x.PublishDateUTC)
                : subscriptionPosts.OrderByDescending(x => x.PublishDateUTC);

            var model = subscriptionPosts.Skip(startFrom).Take(PageSize)
                .Select(x => new SubscriptionItemViewModel
                {
                    Author = x.Authors,
                    Content = x.Content,
                    OriginalUrl = x.OriginalUrl,
                    PublishDateUTC = x.PublishDateUTC,
                    FeedId = x.Subscription.Id,
                    Title = x.Title,
                    Id = x.Id,
                    FeedName = x.Subscription.Title,
                    AlreadyRead = x.PostsRead.Any(y => y.UserSubscription.User.Email == WebSecurity.CurrentUserName),
                    InBookmarks = x.Subscription.UserSubscriptions.Any(y => y.User.Email == WebSecurity.CurrentUserName
                    && y.User.Bookmarks.Any(z => z.User.Email == y.User.Email && z.SubscriptionPost.Id == x.Id))
                });

            return PartialView("_SubscriptionItems", model);
        }
    }
}
