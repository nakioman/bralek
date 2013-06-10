using System.Data.Entity;

namespace Treenks.Bralek.Common.Model
{
    public class BralekDbContext : DbContext
    {
        public BralekDbContext(string connectionStringName) : base(connectionStringName) { }

        public IDbSet<User> Users { get; set; }

        public IDbSet<Subscription> Subscriptions { get; set; }

        public IDbSet<UserSubscription> UserSubscriptions { get; set; }

        public IDbSet<SubscriptionPost> SubscriptionPosts { get; set; }

        public IDbSet<PostRead> PostsRead { get; set; }

        public IDbSet<Bookmark> Bookmarks { get; set; } 
    }
}