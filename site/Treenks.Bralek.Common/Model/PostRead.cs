using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Treenks.Bralek.Common.Model
{
    [Table("PostsRead")]
    public class PostRead
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        public virtual DateTime ReadOnUTC { get; set; }

        public virtual SubscriptionPost SubscriptionPost { get; set; }

        public virtual UserSubscription UserSubscription { get; set; }
    }
}