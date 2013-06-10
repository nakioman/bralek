using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Treenks.Bralek.Common.Model
{
    [Table("Subscriptions")]
    public class Subscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        public virtual string SiteUrl { get; set; }

        public virtual string FeedUrl { get; set; }

        public virtual int Hash { get; set; }

        public virtual DateTime AddedOnUTC { get; set; }

        public virtual User AddedBy { get; set; }

        [InverseProperty("Subscription")]
        public virtual ICollection<SubscriptionPost> SubscriptionPosts { get; set; }

        [Column(TypeName = "datetime2")]
        public virtual DateTime? LastFeedUpdatesUTC { get; set; }

        public virtual bool IsWorking { get; set; }

        public virtual string LastError { get; set; }

        [InverseProperty("Subscription")]
        public virtual ICollection<UserSubscription> UserSubscriptions { get; set; }

        public virtual string Title { get; set; }
    }
}