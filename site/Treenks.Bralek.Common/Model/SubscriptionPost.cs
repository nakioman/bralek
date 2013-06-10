using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Treenks.Bralek.Common.Model
{
    [Table("SubscriptionPosts")]
    public class SubscriptionPost
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }

        public virtual string Title { get; set; }

        [Column(TypeName = "datetime2")]
        public virtual DateTime? PublishDateUTC { get; set; }

        public virtual string Authors { get; set; }

        public virtual string Categories { get; set; }

        public virtual Subscription Subscription { get; set; }

        public string OriginalUrl { get; set; }

        public string Content { get; set; }

        [InverseProperty("SubscriptionPost")]
        public virtual ICollection<PostRead> PostsRead { get; set; }
    }
}