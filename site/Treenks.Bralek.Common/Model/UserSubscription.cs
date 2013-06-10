using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Treenks.Bralek.Common.Model
{
    [Table("UserSubscriptions")]
    public class UserSubscription
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public virtual int Id { get; set; }        

        public virtual User User { get; set; }

        public virtual Subscription Subscription { get; set; }

        public virtual DateTime UpdatedOnUTC { get; set; }

        public virtual DateTime AddedOnUTC { get; set; }

        [InverseProperty("UserSubscription")]
        public virtual ICollection<PostRead> PostsRead { get; set; }

        public int UnreadItems { get; set; }
    }
}